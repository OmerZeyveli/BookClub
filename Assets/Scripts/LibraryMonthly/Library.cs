using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.Runtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Library : MonoBehaviour
{
    [SerializeField] private Transform bookContainer; // Parent object for book objects
    [SerializeField] private GameObject bookPrefab; // Prefab for a single book object
    private AmazonDynamoDBClient client;
    private string userID;

    private async void Start()
    {
        userID = SaveSystem.LoadEmail();

        // Initialize DynamoDB client.
        var credentials = new BasicAWSCredentials("AWS DATABASE KEYS", "AWS DATABASE KEYS");
        client = new AmazonDynamoDBClient(credentials, RegionEndpoint.EUNorth1);

        // Load books
        await LoadBooks();
    }

    private async Task LoadBooks()
    {
        if (client == null)
        {
            Debug.LogError("DynamoDB client is not initialized.");
            return;
        }

        try
        {
            // Load Books and UserBooks tables
            Table booksTable = Table.LoadTable(client, "Books");
            Table userBooksTable = Table.LoadTable(client, "UserBooks");

            if (booksTable == null)
            {
                Debug.LogError("Failed to load Books table.");
                return;
            }

            if (userBooksTable == null)
            {
                Debug.LogError("Failed to load UserBooks table.");
                return;
            }

            // Scan Books table for all books
            var bookSearch = booksTable.Scan(new ScanFilter());
            var bookDocuments = await bookSearch.GetNextSetAsync();

            if (bookDocuments == null || bookDocuments.Count == 0)
            {
                Debug.LogWarning("No books found in the Books table.");
                return;
            }

            // Filter out books with Status == "Next" and sort by ActiveDate (newest to oldest)
            var books = bookDocuments
                .Where(doc => doc.ContainsKey("Status") && doc["Status"].AsString() != "Next")
                .OrderByDescending(doc => doc.ContainsKey("ActiveDate") ? doc["ActiveDate"].AsDateTime() : System.DateTime.MinValue)
                .ToList();

            // Fetch UserBooks for the current user
            var userBookSearch = userBooksTable.Query(userID, new QueryFilter());
            var userBookDocuments = await userBookSearch.GetNextSetAsync();

            Dictionary<string, Document> userBooks = new Dictionary<string, Document>();
            if (userBookDocuments != null)
            {
                userBooks = userBookDocuments.ToDictionary(doc => doc["BookID"].AsString());
            }

            // Populate book objects in Unity
            if (bookPrefab == null)
            {
                Debug.LogError("Book prefab is not assigned.");
                return;
            }

            if (bookContainer == null)
            {
                Debug.LogError("Book container is not assigned.");
                return;
            }

            for (int i = 0; i < books.Count; i++)
            {
                Document book = books[i];
                string bookID = book["BookID"].AsString();
                string title = book.ContainsKey("Title") ? book["Title"].AsString() : "Unknown Title";
                string activeDate = book.ContainsKey("ActiveDate") ? book["ActiveDate"].AsString() : "???";

                // Instantiate a book object
                GameObject bookObject = Instantiate(bookPrefab, bookContainer);
                if (bookObject == null)
                {
                    Debug.LogError("Failed to instantiate book object.");
                    continue;
                }

                // Access child components safely
                Image bookImage = bookObject.GetComponent<Image>();
                TextMeshProUGUI bookName = null;
                TextMeshProUGUI bookDate = null;

                if (bookObject.transform.childCount >= 2)
                {
                    bookName = bookObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
                    bookDate = bookObject.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
                }

                if (bookImage == null)
                {
                    Debug.LogError("Image component is missing on the book object.");
                    continue;
                }

                if (bookName == null || bookDate == null)
                {
                    Debug.LogError("Child components (BookName or BookDate) are missing or not assigned.");
                    continue;
                }

                // Set properties
                bookImage.color = GetBookColor(userBooks, bookID);
                bookName.text = title; // Set book name
                bookDate.text = activeDate; // Set book date
            }

        }
        catch (System.Exception ex)
        {
            Debug.LogError("Error loading books: " + ex.Message);
        }
    }

    private Color GetBookColor(Dictionary<string, Document> userBooks, string bookID)
    {
        if (userBooks.ContainsKey(bookID))
        {
            string status = userBooks[bookID]["Status"].AsString();
            switch (status)
            {
                case "Have Read":
                    return new Color(0.5f, 1, 0.5f, 1); // Green
                case "Didn't Read":
                    return new Color(1, 0.5f, 0.5f, 1); // Red
                case "Not Picked":
                    return new Color(0.5f, 0.5f, 0.5f, 1); // Gray
                /* case "Reading":
                    return new Color(0, 0, 0, 0); // Transparent */
            }
        }
        return new Color(0.5f, 0.5f, 0.5f, 1); // Default to Gray if no record
    }
}
