using UnityEngine;
using System.IO;
using Unity.VisualScripting;

public class DataSystem : MonoBehaviour
{
    public void SaveData(GameData data)
    {
        string json = JsonUtility.ToJson(data);
        string path = Path.Combine(Application.persistentDataPath, "gamedata.json");
        File.WriteAllText(path, json);
        Debug.Log("Game saved to " + path);
    }

    public GameData LoadData()
    {
        string path = Path.Combine(Application.persistentDataPath, "gamedata.json");
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            GameData data = JsonUtility.FromJson<GameData>(json);
            Debug.Log("Game loaded from " + path);
            return data;
        }
        else
        {
            Debug.LogWarning("Save file not found in " + path);
            return null;
        }
    }

    void OnEnable()
    {
        GameData data = LoadData();
        if (data == null || !ValidateData(data))
        {
            data = new GameData();
            SaveData(data);
            Debug.Log("New game data generated");
        }
    }

    bool ValidateData(GameData data)
    {
        return true; // alterar para detetar se a GameData está desatualizada (falta algum parametro)
    }
}