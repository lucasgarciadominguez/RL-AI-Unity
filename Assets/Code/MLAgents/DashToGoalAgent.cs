/*
 * Dash To Goal Agent - RL AI Navigation Script
 * Author: Lucas García
 * Description: Controls an AI agent that navigates between platforms and uses dash mechanics 
 * to reach targets. Rewards are given for approaching targets and penalized for inefficient actions.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
/// <summary>
/// Class that heritates from MLAgents and goes throgh a level reaching several points and getting rewards.
/// </summary>
public class DashToGoalAgent : Agent
{
    // Player controller reference
    private Player_controler playerController;

    [SerializeField]
    private Transform enviromentTransform;

    // List of target platforms
    [SerializeField]
    private List<Transform> targetTransform = new List<Transform>();

    [SerializeField]
    private Material winMaterial, loseMaterial;

    [SerializeField]
    private MeshRenderer floorMeshRenderer;

    [SerializeField]
    private Vector3 initialPosition;

    private float previousDistanceToTarget;

    // Tracks the current target index
    [SerializeField]
    private int currentTargetIndex = 0;
    /// <summary>
    /// 
    /// </summary>
    /// <see cref="IsInPlatform"/>
    public bool isInPlatform { private get; set; } = false;

    public bool IsInPlatform
    {

        get => isInPlatform;
        set => isInPlatform = value;
    }
    public delegate void ResetGoalsReach();
    ResetGoalsReach resetGoalsReach;

    private void Awake()
    {
        playerController = gameObject.transform.GetComponent<Player_controler>();
        foreach (var item in targetTransform)
        {
            resetGoalsReach += item.GetComponent<Goal>().ResetReachGoals;

        }
    }

    public override void OnEpisodeBegin()
    {
        // Reset agent's position and target index
        transform.localPosition = initialPosition;
        currentTargetIndex = 0;

        resetGoalsReach();

    }

    // Collects direction to the current target
    public override void CollectObservations(VectorSensor sensor)
    {
        if (currentTargetIndex < targetTransform.Count)
        {
            Vector3 dirToTarget = (targetTransform[currentTargetIndex].localPosition - transform.localPosition).normalized;
            sensor.AddObservation(dirToTarget.x);
            sensor.AddObservation(dirToTarget.z);
        }
        else
        {
            sensor.AddObservation(0);
            sensor.AddObservation(0);
        }
    }

    // Handles agent actions and rewards
    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];
        bool dashAction = actions.DiscreteActions[0] == 1;
        Vector3 moveDirection = new Vector3(moveX, 0, moveZ).normalized;

        playerController.moveDirection = moveDirection;

        if (currentTargetIndex < targetTransform.Count)
        {
            float distanceToTarget = Vector3.Distance(transform.localPosition, targetTransform[currentTargetIndex].localPosition);

            if (distanceToTarget < previousDistanceToTarget)
                AddReward(0.35f);  // Reward for moving closer
            else
                AddReward(-0.15f);  // Penalize for moving away

            previousDistanceToTarget = distanceToTarget;

            if (dashAction)
            {
                playerController.ActivateDash();
                AddReward(0.5f);
            }

            if (distanceToTarget < 2.0f)
            {
                AddReward(1.0f);  // Reward for reaching the target

                if (currentTargetIndex >= targetTransform.Count)
                {
                    SetReward(2.0f);  // Final reward for completing all targets
                    floorMeshRenderer.material = winMaterial;
                    EndEpisode();
                }
            }
        }

        AddReward(-1f / MaxStep);  // Small penalty to encourage faster progress
    }
    private (string, string, int) GetInfo()
    {
        return ("hey", "hey", 1);
    }
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continousActions = actionsOut.ContinuousActions;
        continousActions[0] = Input.GetAxisRaw("Horizontal");
        continousActions[1] = Input.GetAxisRaw("Vertical");

        ActionSegment<int> discreteActions = actionsOut.DiscreteActions;
        discreteActions[0] = Input.GetButtonDown("Dash") ? 1 : 0;
    }

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
                currentTargetIndex++;
                AddReward(goal.GetValue());
                goal.SetReach(true);
                playerController.ActivateDash();
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