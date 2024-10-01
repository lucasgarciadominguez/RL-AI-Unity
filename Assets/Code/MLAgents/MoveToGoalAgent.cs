/*
 * Move To Goal Agent - RL AI Navigation Script
 * Author: Lucas García
 * Description: This script controls an AI agent that moves towards goals while being rewarded for 
 * efficient movement and penalized for inefficiency. The agent receives inputs about target direction 
 * and adjusts its behavior to reach objectives.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using System;
using Random = UnityEngine.Random;
using System.Runtime.InteropServices.WindowsRuntime;
using static MoveToGoalAgent;
public class MoveToGoalAgent : Agent
{
    // Reference to the player's controller
    private Player_controler playerController;

    [SerializeField]
    private Transform enviromentTransform;

    // List of target positions
    [SerializeField]
    private List<Transform> targetTransform = new List<Transform>();

    [SerializeField]
    private Transform targetTransform2;

    [SerializeField]
    private Material winMaterial, loseMaterial;

    [SerializeField]
    private MeshRenderer floorMeshRenderer;

    [SerializeField]
    private Vector3 initialPosition;

    private float previousDistanceToTarget;
    private float angleChange;
    private Vector3 previousDirection, currentDirection;

    [SerializeField]
    private float threshold;

    // Enable randomness for agent's starting position
    [SerializeField]
    bool allowRandomness = false;

    // Track current target index
    [SerializeField]
    private int currentTargetIndex = 0;

    private float distanceToTarget;
    private Vector3 moveDirection;
    private float moveX;
    private float moveZ;
    private Vector3 dirToTarget;

    public delegate void ResetGoalsReach();
    ResetGoalsReach resetGoalsReach;

    private void Awake()
    {
        playerController = transform.GetComponent<Player_controler>();
        string name = "lucas";
        string newName = "";
        for (int i = name.Length; i > 0; i--)
        {
            newName+=name[i];
        }
        foreach (var item in targetTransform)
        {
            resetGoalsReach+= item.GetComponent<Goal>().ResetReachGoals;
        }
    }
    public override void OnEpisodeBegin()
    {
        if (allowRandomness)
        {
            // Set random positions for agent and targets
            transform.localPosition = new Vector3(Random.Range(-13f, 13f), 0f, Random.Range(-13f, 13f));
            targetTransform[0].localPosition = new Vector3(Random.Range(-13f, 13f), 0f, Random.Range(-13f, 13f));
            foreach (var item in targetTransform)
            {
                item.localPosition = new Vector3(Random.Range(-13f, 13f), 0f, Random.Range(-13f, 13f));
            }
            enviromentTransform.rotation = Quaternion.Euler(0, Random.Range(1.5f, 3.0f), 0);
        }
        else
        {
            transform.localPosition = initialPosition;
        }

        resetGoalsReach();
        currentTargetIndex = 0;
    }

    // Observations: Provide direction to current target
    public override void CollectObservations(VectorSensor sensor)
    {
        if (currentTargetIndex < targetTransform.Count)
        {
            dirToTarget = (targetTransform[currentTargetIndex].localPosition - transform.localPosition).normalized;
            sensor.AddObservation(dirToTarget.x);
            sensor.AddObservation(dirToTarget.z);
        }
        else
        {
            // No more targets to observe
            sensor.AddObservation(0);
            sensor.AddObservation(0);
        }
    }

    // Actions and rewards
    public override void OnActionReceived(ActionBuffers actions)
    {
        moveX = actions.ContinuousActions[0];
        moveZ = actions.ContinuousActions[1];
        moveDirection = new Vector3(moveX, 0, moveZ).normalized;

        // Apply movement to player
        playerController.moveDirection = moveDirection;

        if (currentTargetIndex < targetTransform.Count)
        {
            distanceToTarget = Vector3.Distance(transform.localPosition, targetTransform[currentTargetIndex].localPosition);

            if (distanceToTarget < previousDistanceToTarget)
                AddReward(0.05f);  // Reward for getting closer
            else
                AddReward(-0.05f);  // Penalize for moving away

            previousDistanceToTarget = distanceToTarget;

            // Additional reward for maintaining a constant direction
            angleChange = Vector3.Angle(previousDirection, currentDirection);
            if (angleChange < threshold)
                AddReward(0.15f);  // Reward for consistency in movement direction

            previousDirection = currentDirection;
        }

        AddReward(-1f / MaxStep);  // Small penalty for each step to encourage faster actions
    }

    // Manual controls for testing
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continousActions = actionsOut.ContinuousActions;
        continousActions[0] = Input.GetAxisRaw("Horizontal");
        continousActions[1] = Input.GetAxisRaw("Vertical");
    }

    // Handle collisions with goals or walls
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Goal>(out Goal goal))
        {
            if (goal.GetIsFinalReward())
            {
                SetReward(1);
                floorMeshRenderer.material = winMaterial;
                EndEpisode();
            }
            else if (!goal.reach)
            {
                currentTargetIndex++;  // Move to the next target
                AddReward(goal.GetValue());
                goal.SetReach(true);
            }
        }
        else if (other.TryGetComponent<Wall>(out Wall wall))
        {
            SetReward(-1f);
            floorMeshRenderer.material = loseMaterial;
            EndEpisode();
        }
    }
}