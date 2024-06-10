using UnityEngine;

[System.Serializable]
public class GameData
{
    public int Kills;
    public int Deaths;
    public bool Dash_DoubleTap;
    public string Language;

    public GameData()
    {
        Kills = 0;
        Deaths = 0;
        Dash_DoubleTap = true;
        Language = "English";
    }
}