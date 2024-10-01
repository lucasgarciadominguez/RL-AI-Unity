using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{
    [SerializeField]
    float value = 1;
    [SerializeField]
    bool isFinalReward =false;
    [SerializeField]
    public bool reach { get; private set; } = false;

    public void ResetReachGoals() => reach = false;
    public float GetValue() => value;
    public bool GetIsFinalReward() => isFinalReward;
    public void SetReach(bool _reach) => reach = _reach;
}
