using UnityEngine;

public interface IStrategy
{
    void RequestStrategyDecision(GameObject go, Power myPower, Power enemyPower);
}
