using System;
using UnityEngine;

public class StrategyAlgo : MonoBehaviour, IStrategy
{
    public void RequestStrategyDecision(GameObject go, Power myPower, Power enemyPower)
    {
        var myNormalizedPower = GetNormalizedPower(myPower);
        var enemyNormalizedPower = GetNormalizedPower(enemyPower);
        var strategy = CalculateStrategy(myNormalizedPower, enemyNormalizedPower);
        go.GetComponent<StrategyHandler>().StrategyResponse(strategy);
    }

    private StrategyEnum CalculateStrategy(float myNormalizedPower, float enemyNormalizedPower)
    {
        var delta = 1.4f;
        if(myNormalizedPower > enemyNormalizedPower * delta)
        {
            return StrategyEnum.Scout;
        }else if (myNormalizedPower * delta < enemyNormalizedPower)
        {
            return StrategyEnum.Defensive;
        }
        else
        {
            return StrategyEnum.Offensive;
        }
    }

    private float GetNormalizedPower(Power p)
    {
        return
            p.CastleNum * 100 +
            p.Level1Bug * 10 +
            p.Level2Bug * 15 +
            p.Level3Bug * 20 +
            p.TotalResource;
    }
}

