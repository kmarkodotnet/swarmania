using System;
using UnityEngine;

public class StrategyHandler : MonoBehaviour
{
    private IStrategy _strategy;

    [SerializeField]
    private string PlayerId;

    [SerializeField]
    private Power Power;

    [SerializeField]
    private float decisionRequestTimeInterval = 10000;

    private DateTime? nextDecisionRequestTime = null;
    private bool _isNewBorn;

    System.Random _r;
    StrategyEnum strategyType;


    private void Start()
    {
        PlayerId = gameObject.GetComponentInParent<Player>().GetId();
        nextDecisionRequestTime = DateTime.Now.AddMilliseconds(decisionRequestTimeInterval);
        //_strategy = gameObject.AddComponent<StrategyAi>();
        _isNewBorn = false;
        _strategy = gameObject.AddComponent<StrategyAlgo>();

        _r = new System.Random();
    }

    public Power GetPower()
    {
        return Power;
    }

    void Update()
    {
        RequestStrategyIfNeeded();
        if (IsNewBornBug())
        {
            SwapBugStrategies();
        }
    }

    private bool IsNewBornBug()
    {
        return _isNewBorn;
    }
    public void SetNewBornBug()
    {
        _isNewBorn = true;
    }

    private void RequestStrategyIfNeeded()
    {
        if (nextDecisionRequestTime.HasValue && DateTime.Now > nextDecisionRequestTime.Value)
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

            nextDecisionRequestTime = DateTime.Now.AddMilliseconds(decisionRequestTimeInterval);
        }
    }


    private void SwapBugStrategies()
    {
        
    }

    public void BugCreatedByCastle(Bug bug)
    {
        bug.SetBugStrategy(strategyType);
    }


    public void StrategyResponse(StrategyEnum response)
    {
        strategyType = response;
    }
}
