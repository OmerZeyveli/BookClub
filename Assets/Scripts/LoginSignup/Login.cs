using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.Runtime;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class Login : MonoBehaviour
{
    [SerializeField] Button loginButton;
    [SerializeField] TMP_InputField mailInput;
    [SerializeField] TMP_InputField passwordInput;

    AmazonDynamoDBClient client;
    MainMenu menu;

    void Awake()
    {
        if(!string.IsNullOrWhiteSpace(SaveSystem.LoadEmail()))
        {
            SceneManager.LoadScene(1);
        }
    }

    private void Start()
    {
        // Initialize DynamoDB client.
        var credentials = new BasicAWSCredentials("AKIA4VDBMGBDPKBZ3PHN", "4G7g0X9TCDj+PSvH5M3ocTu7pcje0ceZsXacxnEJ");
        client = new AmazonDynamoDBClient(credentials, RegionEndpoint.EUNorth1); // Change region as needed

        menu = this.GetComponent<MainMenu>();
    }

    public void MakeLoginInteractable()
    {

        // Nullcheck inputfields.
        // if (mailInput == null || passwordInput == null) { return; }

        // Check if inputfield texts are empty to make login button interactable.
        if (string.IsNullOrWhiteSpace(mailInput.text) || string.IsNullOrWhiteSpace(passwordInput.text))
        {
            loginButton.interactable = false;
        }
        else
        {
            loginButton.interactable = true;
        }
    }

    public async void StartLogin()
    {
        if (client == null) return;

        // Check for internet connectivity
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            StartCoroutine(menu.ShowErrorPanel("No internet connection."));
            return;
        }

        // Get email and password.
        string email = mailInput.text;
        string password = passwordInput.text;

        // Use MD5 encryiption with salt.
        // Salt is used for rainbow table attacks.
        string salt = "tuz1!";
        password = HashingUtility.GetMD5(salt + password);

        // Exception handling incase of database problems.
        try
        {
            // Connect to DynamoDB table.
            Table userTable = Table.LoadTable(client, "Users");

            // Fetch user data by email.
            Document userDocument = await userTable.GetItemAsync(email);

            // Since its noSQL check if fetched user has a password.
            if (userDocument != null && userDocument.Contains("Password"))
            {
                // User's password and active status from DynamoDB
                string storedPassword = userDocument["Password"].AsString();
                bool isActive = userDocument["IsActive"].AsBoolean();

                if (!isActive)
                {
                    StartCoroutine(menu.ShowErrorPanel("User account is inactive."));
                    return;
                }

                if (storedPassword == password)
                {
                    Debug.Log("Login successful!");
                    SaveSystem.SaveEmail(email);
                    SceneManager.LoadScene(1);
                }
                else
                {
                    StartCoroutine(menu.ShowErrorPanel("Incorrect mail or password."));
                }
            }
            else
            {
                StartCoroutine(menu.ShowErrorPanel("Incorrect mail or password."));
            }
        }
        catch (System.Exception ex)
        {
            StartCoroutine(menu.ShowErrorPanel("Error accessing DataBase :("));
            Debug.LogError("Error accessing DynamoDB: " + ex.Message);
        }
    }
}