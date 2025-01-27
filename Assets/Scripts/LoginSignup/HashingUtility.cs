using System.Security.Cryptography;
using System.Text;

public static class HashingUtility
{
    public static string GetMD5(string input)
    {
        // Step 1: Convert the input string to a byte array
        byte[] inputBytes = Encoding.UTF8.GetBytes(input);

        // Step 2: Compute the MD5 hash
        using (MD5 md5 = MD5.Create())
        {
            byte[] hashBytes = md5.ComputeHash(inputBytes);

            // Step 3: Convert hash byte array to a hexadecimal string
            StringBuilder sb = new StringBuilder();
            foreach (byte b in hashBytes)
            {
                sb.Append(b.ToString("x2")); // "x2" formats to a two-character hexadecimal
            }

            return sb.ToString();
        }
    }
}