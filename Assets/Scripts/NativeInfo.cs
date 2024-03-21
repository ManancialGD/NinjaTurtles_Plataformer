using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NativeInfo : MonoBehaviour
{
    int currentPlayerID = 1;
    int history_PlayerID = 1;
    public GameObject[] playerObj;
    public float[] playerGroundAcceleration;
    public float[] PlayerGroundFriction;
    public float[] playerAirAcceleration;
    public float[] playerSlideSpeed;
    public int[] playerDamage;
    public int[] playerGroundSlamDamage;
    public Vector2[] playerAttackForce;
    public Vector2[] playerAttackVelocityBoost;
    public float[] playerJumpForce;
    public Vector2[] playerWallJumpForce;
    public float[] PLAYER_MAX_GROUND_VELOCITY;
    public float[] PLAYER_MAX_AIR_VELOCITY;

    public GameObject GetPlayerObj(int playerID)
    {
        if (playerObj[playerID - 1] != null) return playerObj[playerID - 1];
        else
        {
            Debug.Log("NATIVE ERROR 869412 - Invalid <playerID> input. (" + (playerID - 1) + ")");
            return null;
        }
    }

    public int GetPlayerIndexByObject(GameObject obj)
    {
        int index = 1;
        foreach (GameObject p in playerObj)
        {
            if (obj == p) return index;
            index++;
        }
        return -1;
    }

}
