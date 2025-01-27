using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.Runtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SignUp : MonoBehaviour
{
    [SerializeField] TMP_InputField emailInput;
    [SerializeField] TMP_InputField passwordInput;
    [SerializeField] TMP_InputField nameInput;
    [SerializeField] TMP_InputField universityInput;
    [SerializeField] TMP_InputField departmentInput;
    [SerializeField] Button signUpButton;

    AmazonDynamoDBClient client;

    MainMenu menu;

    private void Start()
    {
        // Initialize DynamoDB client
        var credentials = new BasicAWSCredentials("AKIA4VDBMGBDPKBZ3PHN", "4G7g0X9TCDj+PSvH5M3ocTu7pcje0ceZsXacxnEJ");
        client = new AmazonDynamoDBClient(credentials, Amazon.RegionEndpoint.EUNorth1);

        menu = this.GetComponent<MainMenu>();
    }

    public void MakeSignupInteractable()
    {
        if (string.IsNullOrEmpty(emailInput.text) || string.IsNullOrEmpty(passwordInput.text) || string.IsNullOrEmpty(nameInput.text) ||
            string.IsNullOrEmpty(universityInput.text) || string.IsNullOrEmpty(departmentInput.text))
        {
            signUpButton.interactable = false;
        }
        else
        {
            signUpButton.interactable = true;
        }
    }

    public async void StartSignup()
    {
        string email = emailInput.text.Trim();
        string password = passwordInput.text.Trim();
        string name = nameInput.text.Trim();
        string university = universityInput.text.Trim();
        string department = departmentInput.text.Trim();

        if (!IsValidEmail(email))
        {
            StartCoroutine(menu.ShowErrorPanel("Invalid email address."));
            return;
        }

        if (!IsValidPassword(password))
        {
            StartCoroutine(menu.ShowErrorPanel("At least 8 characters; 1 uppercase, lowercase, number, special character."));
            return;
        }

        // Check if user already exists
        if (await UserExists(email))
        {
            StartCoroutine(menu.ShowErrorPanel("Email already registered."));
            return;
        }

        // Hash the password with MD5 and a salt
        string salt = "tuz1!";
        string hashedPassword = HashingUtility.GetMD5(salt + password);;

        // Create user in DynamoDB
        await CreateUser(email, hashedPassword, name, university, department);

        ResetInputFields();
        menu.MainMenuButton();

        StartCoroutine(menu.ShowErrorPanel("Sign-up successful! Please wait for admin approval."));
    }

    private void ResetInputFields()
    {
        emailInput.text = "";
        passwordInput.text = "";
        nameInput.text = "";
        universityInput.text = "";
        departmentInput.text = "";
    }

    private bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

    private bool IsValidPassword(string password)
    {
        string pattern = "^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*\\.\\-]).{8,}$";
        return System.Text.RegularExpressions.Regex.IsMatch(password, pattern);
    }

    private async Task<bool> UserExists(string email)
    {
        Table userTable = Table.LoadTable(client, "Users");
        Document userDoc = await userTable.GetItemAsync(email);
        return userDoc != null;
    }

    private async Task CreateUser(string email, string hashedPassword, string name, string university, string department)
    {
        try
        {
            Table userTable = Table.LoadTable(client, "Users");

            Document newUser = new Document
            {
                ["UserID"] = email,
                ["CreatedAt"] = DateTime.UtcNow.ToString("o"), // ISO 8601 format
                ["IsActive"] = new DynamoDBBool(false), // Explicitly save as a boolean
                ["Name"] = name,
                ["Password"] = hashedPassword,
                ["University"] = university,
                ["Department"] = department
            };

            await userTable.PutItemAsync(newUser);
        }
        catch (Exception ex)
        {
            Debug.LogError("Error creating user: " + ex.Message);
            StartCoroutine(menu.ShowErrorPanel("An error occurred during sign-up. Error_101"));
        }
    }
}