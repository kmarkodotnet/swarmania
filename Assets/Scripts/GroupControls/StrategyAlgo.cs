using System;
using UnityEngine;

public class StrategyAlgo : MonoBehaviour, IStrategy
{
    public void RequestStrategyDecision(GameObject go, Power myPower, Power enemyPower)
    {
        Debug.Log("Algo");
    }

}

