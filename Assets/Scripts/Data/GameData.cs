using UnityEngine;

[System.Serializable]
public class GameData
{
    public int Kills { get; set; }
    public int Deaths { get; set; }

    public GameData()
    {
        Kills = 0;
        Deaths = 0;
    }
}