using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] string id;

    [SerializeField] string name;

    [SerializeField] bool isHuman;

    public string GetId()
    {
        return id;
    }
    public string GetName()
    {
        return name;
    }

    public void RegisterBug(Bug bug)
    {
        if (!isHuman)
        {
            var strategyHandler = gameObject.transform.Find("StrategyHandler");
            var sh = strategyHandler.GetComponent<StrategyHandler>();
            sh.BugCreatedByCastle(bug);
        }
    }
}
