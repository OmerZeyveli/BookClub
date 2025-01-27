using System;
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

public class Monthly : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI thisMonthBookTitle;
    [SerializeField] private TextMeshProUGUI nextMonthBookTitle;
    [SerializeField] private Button willReadButton;
    [SerializeField] private Button wontReadButton;
    [SerializeField] private Button haveReadButton;

    private AmazonDynamoDBClient client;
    private string userID;

    private string currentBookID;
    private string nextBookID;

    private Document currentUserBookRelation;
    private Document nextUserBookRelation;

    private async void Start()
    {
        userID = SaveSystem.LoadEmail();

        // Initialize DynamoDB client.
        var credentials = new BasicAWSCredentials("AKIA4VDBMGBDPKBZ3PHN", "4G7g0X9TCDj+PSvH5M3ocTu7pcje0ceZsXacxnEJ");
        client = new AmazonDynamoDBClient(credentials, RegionEndpoint.EUNorth1);

        await LoadMonthlyBooks();
    }

    private async Task LoadMonthlyBooks()
    {
        try
        {
            Table booksTable = Table.LoadTable(client, "Books");
            Table userBooksTable = Table.LoadTable(client, "UserBooks");

            // Fetch current and next month's books
            var booksSearch = booksTable.Scan(new ScanFilter());
            var bookDocuments = await booksSearch.GetNextSetAsync();

            if (bookDocuments == null || bookDocuments.Count == 0)
            {
                Debug.LogWarning("No books found.");
                return;
            }

            var currentBook = bookDocuments.FirstOrDefault(b => b["Status"].AsString() == "Current");
            var nextBook = bookDocuments.FirstOrDefault(b => b["Status"].AsString() == "Next");

            if (currentBook == null || nextBook == null)
            {
                Debug.LogWarning("Current or Next month book not found.");
                return;
            }

            currentBookID = currentBook["BookID"].AsString();
            nextBookID = nextBook["BookID"].AsString();

            thisMonthBookTitle.text = $"{currentBook["Title"]} \"{currentBook["Author"]}\"";
            nextMonthBookTitle.text = $"{nextBook["Title"]} \"{nextBook["Author"]}\"";

            // Fetch user relationships with books
            currentUserBookRelation = await userBooksTable.GetItemAsync(userID, currentBookID);
            nextUserBookRelation = await userBooksTable.GetItemAsync(userID, nextBookID);

            UpdateButtonStates();
        }
        catch (Exception ex)
        {
            Debug.LogError("Error loading monthly books: " + ex.Message);
        }
    }

    private void UpdateButtonStates()
    {
        // Configure buttons based on relationships
        if (nextUserBookRelation == null)
        {
            willReadButton.interactable = true;
            wontReadButton.interactable = true;
        }
        else
        {
            willReadButton.interactable = false;
            wontReadButton.interactable = false;
        }

        if (currentUserBookRelation != null && currentUserBookRelation["Status"].AsString() == "Didn't Read")
        {
            haveReadButton.interactable = true;
        }
        else
        {
            haveReadButton.interactable = false;
        }
    }

    public async void OnWillReadButtonClicked()
    {
        try
        {
            Table userBooksTable = Table.LoadTable(client, "UserBooks");

            Document newRelation = new Document
            {
                ["UserID"] = userID,
                ["BookID"] = nextBookID,
                ["Status"] = "Didn't Read",
                ["SelectedDate"] = DateTime.UtcNow.ToString("yyyy-MM-dd")
            };

            await userBooksTable.PutItemAsync(newRelation);

            willReadButton.interactable = false;
            wontReadButton.interactable = false;

            Debug.Log("Will Read status set successfully.");
        }
        catch (Exception ex)
        {
            Debug.LogError("Error setting Will Read status: " + ex.Message);
        }
    }

    public async void OnWontReadButtonClicked()
    {
        try
        {
            Table userBooksTable = Table.LoadTable(client, "UserBooks");

            Document newRelation = new Document
            {
                ["UserID"] = userID,
                ["BookID"] = nextBookID,
                ["Status"] = "Not Picked",
                ["SelectedDate"] = DateTime.UtcNow.ToString("yyyy-MM-dd")
            };

            await userBooksTable.PutItemAsync(newRelation);

            willReadButton.interactable = false;
            wontReadButton.interactable = false;

            Debug.Log("Won't Read status set successfully.");
        }
        catch (Exception ex)
        {
            Debug.LogError("Error setting Won't Read status: " + ex.Message);
        }
    }

    public async void OnHaveReadButtonClicked()
    {
        try
        {
            if (currentUserBookRelation == null || currentUserBookRelation["Status"].AsString() != "Didn't Read")
            {
                Debug.LogWarning("Cannot mark book as read: invalid current status.");
                return;
            }

            Table userBooksTable = Table.LoadTable(client, "UserBooks");

            currentUserBookRelation["Status"] = "Have Read";
            currentUserBookRelation["CompletionDate"] = DateTime.UtcNow.ToString("yyyy-MM-dd");

            await userBooksTable.UpdateItemAsync(currentUserBookRelation);

            haveReadButton.interactable = false;

            Debug.Log("Book marked as read successfully.");
        }
        catch (Exception ex)
        {
            Debug.LogError("Error marking book as read: " + ex.Message);
        }
    }
}