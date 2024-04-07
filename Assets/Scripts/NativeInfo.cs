using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class NativeInfo : MonoBehaviour
{
    public int currentPlayerID = 1;
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

    public float npcGroundAcceleration;
    public float NPC_MAX_GROUND_VELOCITY;
    private GameObject[] CheckpointCircle;
    [SerializeField] public Sprite BallSprite;

    public LayerMask playerLayerMask;
    public Transform enemiesLayer;
    public PhysicsMaterial2D rockMaterial;
    private Vector2[] targetCheckPoints;

    private LampScript[] lampScripts;
    public float staminaUse_GroundSlam;

    void Start()
    {
        lampScripts = FindObjectsOfType<LampScript>();
        ResetTargetCheckpoints(false);
    }

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

    void Update()
    {

        OnChangePlayer();

    }

    public LayerMask GetPlayerLayerMask() => playerLayerMask;
    private int OnChangePlayer()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) && currentPlayerID != 1)
        {
            currentPlayerID = 1;
            ResetTargetCheckpoints(true);
            return 1;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2) && currentPlayerID != 2)
        {
            currentPlayerID = 2;
            ResetTargetCheckpoints(true);
            return 2;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3) && currentPlayerID != 3)
        {
            currentPlayerID = 3;
            ResetTargetCheckpoints(true);
            return 3;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4) && currentPlayerID != 4)
        {
            currentPlayerID = 4;
            ResetTargetCheckpoints(true);
            return 4;
        }

        return 0;
    }

    public Vector2 GetSelectedPlayerPosition() => playerObj[currentPlayerID - 1].transform.position;


    public void CreateTargetCheckpoint(Vector2 newPosition)
    {

        Vector2[] debugPositions = new Vector2[targetCheckPoints.Length + 1];

        for (int i = 0; i < targetCheckPoints.Length; i++)
        {
            debugPositions[i] = targetCheckPoints[i];
        }
        debugPositions[targetCheckPoints.Length] = newPosition;

        targetCheckPoints = debugPositions;

        HideCheckpoints();
        ShowCheckpoints();

        return;
    }

    public void DeleteTargetCheckpoint(int index)
    {
        Vector2[] debugPositions = new Vector2[targetCheckPoints.Length - 1];

        int debug_index = 0;

        for (int i = 0; i < targetCheckPoints.Length; i++)
        {
            if (i != index)
            {
                debugPositions[debug_index] = targetCheckPoints[i];
                debug_index++;
            }
        }

        NPC[] npcFile = new NPC[playerObj.Length];
        for (int i = 0; i < playerObj.Length; i++)
        {
            npcFile[i] = playerObj[i].GetComponent<NPC>();
            npcFile[i].followingCheckpointID -= 1;
        }

        targetCheckPoints = debugPositions;

        HideCheckpoints();
        ShowCheckpoints();
        return;
    }

    private void ResetTargetCheckpoints(bool updatePosition)
    {
        targetCheckPoints = new Vector2[0];
        if (updatePosition) CreateTargetCheckpoint(playerObj[currentPlayerID - 1].transform.position);
        return;
    }

    public (Vector2, int) GetDistanceFromNearestCheckpoint(Vector2 playerPosition)
    {
        bool executed = false;
        Vector2 min_distance = new Vector2(1000000f, 1000000f);
        int checkpointID = 0;
        for (int i = 0; i < targetCheckPoints.Length; i++)
        {
            Vector2 distance = new Vector2(playerPosition.x - targetCheckPoints[i].x, playerPosition.y - targetCheckPoints[i].y);
            if (Mathf.Abs(distance.x) + Mathf.Abs(distance.y) < Mathf.Abs(min_distance.x) + Mathf.Abs(min_distance.y))
            {
                min_distance = distance;
                checkpointID = i;
                executed = true;
            }
        }
        if (!executed)
        {
            Debug.Log("ERRO 96941 - Nenhum checkpoint encontrado");
            return (new Vector2(0f, 0f), -1);
        }
        return (min_distance, checkpointID);
    }

    public int GetCheckpointsAmount() => targetCheckPoints.Length;

    public int GetLastCheckpointBeingFollowed()
    {
        NPC[] npcFile = new NPC[playerObj.Length];
        bool executed = false;
        int lastCheckpointID = 0;
        for (int i = 0; i < playerObj.Length; i++)
        {
            npcFile[i] = playerObj[i].GetComponent<NPC>();
            if (npcFile[i].followingCheckpointID < lastCheckpointID || !executed)
            {
                lastCheckpointID = npcFile[i].followingCheckpointID;
                executed = true;
            }
        }
        return lastCheckpointID;
    }

    public Vector2[] GetCheckpoints() => targetCheckPoints;

    public void ShowCheckpoints()
    {
        CheckpointCircle = new GameObject[targetCheckPoints.Length];
        for (int i = 0; i < targetCheckPoints.Length; i++)
        {

            CheckpointCircle[i] = new GameObject("Checkpoint_" + i);
            CheckpointCircle[i].transform.SetParent(GetComponent<Transform>());
            SpriteRenderer spriteRenderer = CheckpointCircle[i].AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = BallSprite;
            CheckpointCircle[i].transform.position = targetCheckPoints[i];
            CheckpointCircle[i].transform.localScale = new Vector2(0.3f, 0.3f);

        }
        return;
    }

    public void HideCheckpoints()
    {
        foreach (GameObject obj in CheckpointCircle)
        {
            Destroy(obj);
        }
        return;
    }

    public List<(GameObject, float)> GetEnemiesDistances(Vector2 position)
    {
        List<(GameObject, float)> enemiesList = new List<(GameObject, float)>();

        foreach (Transform enemy in enemiesLayer)
        {
            float distance = GetDistance(position, enemy.transform.position).Item2;
            enemiesList.Add((enemy.gameObject, distance));
        }

        return enemiesList;
    }

    public (Vector2, float) GetDistance(Vector2 pos1, Vector2 pos2)
    {

        Vector2 distanceVector = pos2 - pos1;
        float magnitude = Mathf.Abs(distanceVector.x) + Mathf.Abs(distanceVector.y);

        return (distanceVector, magnitude);

    }

    public bool isPlayerAttachToAnyLamp()
    {

        foreach (LampScript lamp in lampScripts)
        {
            if (lamp.isPlayerAttached()) return true;
        }

        return false;

    }

    public RaycastHit2D MakeLinecast(Vector2 position, Vector2 direction, float distance, LayerMask contactLayers) => Physics2D.Linecast(position, position + direction * distance, contactLayers);

}
