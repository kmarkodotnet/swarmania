using System;
using UnityEngine;

public class StrategyHandler : MonoBehaviour
{
    private IStrategy _strategy;

    [SerializeField]
    private string PlayerId;

    [SerializeField]
    private Power Power;

    private void Start()
    {
        _strategy = gameObject.AddComponent<StrategyAi>();
        //_strategy = gameObject.AddComponent<StrategyAlgo>();
    }

    public Power GetPower()
    {
        return Power;
    }

    void Update()
    {
        RequestStrategyIfNeeded();
        SelectNextBugTypeIfNeeded();
        GetNewbornBugStrategy();
        SwapBugStrategies();
    }

    private void RequestStrategyIfNeeded()
    {
        var enemyIds = FindObjectOfType<PlayerManager>().GetEnemyIds(PlayerId);
        var enemiesPower = new Power();

        foreach (var enemyId in enemyIds)
        {
            var enemyPower = FindObjectOfType<ObjectCollector>().GetPower(enemyId);
            enemiesPower.Add(enemyPower);
        }

        var myPower = FindObjectOfType<ObjectCollector>().GetPower(PlayerId);
        _strategy.RequestStrategyDecision(gameObject, myPower, enemiesPower);
    }

    public void RequestStrategyResponse()
    {

    }

    private void SwapBugStrategies()
    {
        
    }

    private void GetNewbornBugStrategy()
    {
        
    }

    private void SelectNextBugTypeIfNeeded()
    {
        
    }


    public void StrategyResponse(StrategyEnum response)
    {
        Debug.Log("Response");
    }
}
