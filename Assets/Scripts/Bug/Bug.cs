using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bug : MonoBehaviour
{
    int current = 0;

    [SerializeField] int bugId;
    [SerializeField] ResourceTypeEnum bugCreationResoucreType;
    [SerializeField] float bugCreationResourceAmmount = 10;
    [SerializeField] float bugCreationTime = 10;
    [SerializeField] GameObject birthCastle;

    private static int _nextId;
    private static int NextId
    {
        get
        {
            var nextId = _nextId;
            _nextId++;
            return nextId;
        }
    }
    private void Awake()
    {
        bugId = NextId;
    }

    void Start()
    {

    }

    private void Update()
    {
        current++;

        if (current > 150)
        {
            //Debug.LogWarning(bugId);
        }
    }

    public int GetId()
    {
        return bugId;
    }

    public ResourceTypeEnum GetBugCreationResoucreType()
    {
        return bugCreationResoucreType;
    }
    public float GetBugCreationResourceAmmount()
    {
        return bugCreationResourceAmmount;
    }
    public float GetBugCreationTime()
    {
        return bugCreationTime;
    }

    public GameObject GetBirthCastle()
    {
        return birthCastle;
    }
}
