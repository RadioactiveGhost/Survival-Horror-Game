using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveSystem
{
    public static void Save(SaveData data)
    {
        BinaryFormatter formatter = new BinaryFormatter();

        string path = Path.Combine(Application.persistentDataPath, "SaveData.data");

        FileStream stream = new FileStream(path, FileMode.Create);

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static SaveData Load()
    {
        BinaryFormatter formatter = new BinaryFormatter();

        string path = Path.Combine(Application.persistentDataPath, "SaveData.data");
        if (File.Exists(path))
        {
            FileStream stream = new FileStream(path, FileMode.Open);
            SaveData data = null;

            try
            {
                data = formatter.Deserialize(stream) as SaveData;
                stream.Close();
            }
            catch (System.Exception e)
            {
                stream.Close();
                Debug.LogError("Failed to load data, " + e.Message);
            }

            return data;
        }
        else
        {
            Debug.Log("File doesn't exist");
            return null;
        }
    }
}