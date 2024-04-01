using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{

    NativeInfo native;
    public int thisPlayerID;
    public int followingCheckpointID = 0;
    Rigidbody2D rb;
    Collision coll;
    Player playerScript;

    void Start()
    {
        playerScript = GetComponent<Player>();
        thisPlayerID = playerScript.thisPlayerID;
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collision>();
        native = FindObjectOfType<NativeInfo>();
    }

    void Update()
    {
        if (thisPlayerID == native.currentPlayerID) return; // Este player é o selecionado
        UpdateCheckpoints();
    }

    void UpdateCheckpoints()
    {
        if (native.GetCheckpointsAmount() <= 0) return; //no checkpoins


        // Catch Checkpoint

        Vector2 nearestCheckpoint;
        int nearestCheckpointID;

        (nearestCheckpoint, nearestCheckpointID) = native.GetDistanceFromNearestCheckpoint(new Vector2(transform.position.x, transform.position.y));

        if (Mathf.Abs(nearestCheckpoint.x) + Mathf.Abs(nearestCheckpoint.y) < 1f && native.GetLastCheckpointBeingFollowed() > nearestCheckpointID) // near the following Checkpoint
        { // Check if there is no NPC following behind checkpoints;
            native.DeleteTargetCheckpoint(nearestCheckpointID);
        }


        // Follow Checkpoint

        Vector2[] checkPoints = native.GetCheckpoints();
        Vector2 distance = new Vector2(transform.position.x - checkPoints[followingCheckpointID].x, transform.position.y - checkPoints[followingCheckpointID].y);

        if (distance.x > 0) //backwards
        {
            if (rb.velocity.x - native.npcGroundAcceleration > -native.NPC_MAX_GROUND_VELOCITY) rb.velocity = new Vector2(rb.velocity.x - native.npcGroundAcceleration, rb.velocity.y);
            else rb.velocity = new Vector2(-native.NPC_MAX_GROUND_VELOCITY, rb.velocity.y);
        }
        else if (distance.x < 0) //forward
        {
            if (rb.velocity.x + native.npcGroundAcceleration < native.NPC_MAX_GROUND_VELOCITY) rb.velocity = new Vector2(rb.velocity.x + native.npcGroundAcceleration, rb.velocity.y);
            else rb.velocity = new Vector2(native.NPC_MAX_GROUND_VELOCITY, rb.velocity.y);
        }

    }
}
