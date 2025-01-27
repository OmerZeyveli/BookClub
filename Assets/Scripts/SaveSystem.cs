using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveSystem
{
    public static void SaveEmail(string email)
    {
        string path = Application.persistentDataPath + "/BookClub.riive";
        FileStream stream = new(path, FileMode.Create);

        BinaryFormatter formatter = new();

        formatter.Serialize(stream, email);
        stream.Close();
    }

    public static string LoadEmail()
    {
        string path = Application.persistentDataPath + "/BookClub.riive";
        if (File.Exists(path))
        {
            FileStream stream = new(path, FileMode.Open);

            BinaryFormatter formatter = new();
            string email = formatter.Deserialize(stream) as string;
            stream.Close();

            return email;
        }
        else
        {
            Debug.Log("Save file not found in " + path);
            return null;
        }
    }

    public static void DeleteEmail()
    {
        string path = Application.persistentDataPath + "/BookClub.riive";
        if (File.Exists(path))
        {
            File.Delete(path);
            Debug.Log("Save file deleted.");
        }
        else
        {
            Debug.Log("No save file found to delete in " + path);
        }
    }
}
