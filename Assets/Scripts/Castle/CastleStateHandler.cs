using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class CastleStateHandler : MonoBehaviour
{
    [SerializeField] CastleTypeEnum castleType;
    [SerializeField] CastleStateEnum castleState;
    [SerializeField] CastleJobEnum castleJob;
    [SerializeField] float[] ownedResourceAmmount;
    [SerializeField] ResourceTypeEnum preferedResourceType;
    [SerializeField] float startValueOfPreferedResourceType = 100;
    [SerializeField] BugTypeEnum?[] bugCreationQueue;
    [SerializeField] GameObject[] bugPrefabs = new GameObject[bugPrefabCount];
    [SerializeField] BugTypeEnum[] bugPrefabKeys = new BugTypeEnum[bugPrefabCount];
    [SerializeField] GameObject targetToAttack;

    [SerializeField] static int healthIncreasingValue = 10;


    [SerializeField] static float defaultScale = 1;

    const int bugPrefabCount = 3;

    float bugCreationPeriod = 0.1f;
    float bugCreationTimeLeft;
    System.DateTime start;

    private void Start()
    {
        bugCreationQueue = new BugTypeEnum?[5];

        start = System.DateTime.UtcNow;
        InitializeResourceAmmount();
        AddNewBugToQueue(bugPrefabKeys[0]);
        //bugCreationTimeLeft = GetCreationTimeOfBug(bugCreationQueue[0].Value);
        castleJob = CastleJobEnum.CreatingUnit;
    }


    private void Update()
    {
        var utcNow = System.DateTime.UtcNow;
        if (utcNow > start.AddSeconds(defaultScale) && castleState != CastleStateEnum.Destroyed)
        {
            Debug.Log("tick");
            DoCastleJob();
            HandleCastleState();
            start = utcNow;
        }
    }

    public void AddNewBugToQueue(BugTypeEnum bugTypeEnum)
    {
        Debug.Log("AddNewBugToQueue");
        var firstNullIndex = bugCreationQueue.ToList().IndexOf(null);
        if(firstNullIndex >= 0 && firstNullIndex < bugCreationQueue.Length)
        {
            bugCreationQueue[firstNullIndex] = bugTypeEnum;
            Debug.Log("added");
        }
    }

    List<ResourceTypeEnum> GetResourceTypes()
    {
        var rts = new List<ResourceTypeEnum>();
        var resourceTypes = System.Enum.GetValues(typeof(ResourceTypeEnum));
        foreach (ResourceTypeEnum resourceType in resourceTypes)
        {
            rts.Add(resourceType);
        }
        return rts;
    }
    private void InitializeResourceAmmount()
    {
        var resourceTypes = GetResourceTypes();
        ownedResourceAmmount = new float[resourceTypes.Count];
        int x = 0;
        foreach (ResourceTypeEnum resourceType in resourceTypes)
        {
            ownedResourceAmmount[x] = 15;
            x++;
        }
        ownedResourceAmmount[resourceTypes.IndexOf(preferedResourceType)] += startValueOfPreferedResourceType;
    }

    public float GetResourcesSum()
    {
        return ownedResourceAmmount.Sum();
    }

    internal GameObject[] GetBugPrefabs()
    {
        return this.bugPrefabs;
    }

    private void SetState(CastleStateEnum mewCastleState)
    {
        castleState = mewCastleState;
    }

    private void HandleCastleState()
    {
        if(castleState != CastleStateEnum.Destroyed && gameObject.GetComponent<Health>().GetHealth() <= 0)
        {
            castleState = CastleStateEnum.Destroyed;
        }
        if(castleState == CastleStateEnum.Attacked)
        {
            FindAttacker();
        }else if(castleState == CastleStateEnum.Attack)
        {
            Attack();
        }
        else if(castleState == CastleStateEnum.Idle)
        {
            //Do nothing
        }
        else
        {
            Debug.LogError($"unhandled castlestate: {castleState}");
        }
    }

    private void Attack()
    {
        Debug.LogWarning($"castle can not attack");
    }

    private void FindAttacker()
    {
        var enemy = new CommonService().FindNearestEnemy(gameObject);
        targetToAttack = enemy;
        SetState(CastleStateEnum.Attack);

    }

    private void DoCastleJob()
    {
        if(castleJob == CastleJobEnum.CreatingUnit && IsAnyBugInQueue())
        {
            CreateUnit();
        }else if(castleJob == CastleJobEnum.Healthing)
        {
            IncreaseHealth();
        }
        else if (castleJob == CastleJobEnum.Idle)
        {

        }
        else
        {
            //Debug.LogError($"unhandled job: {castleJob}");
        }
    }

    private bool IsAnyBugInQueue()
    {
        if (bugCreationQueue == null || bugCreationQueue.Length <= 0)
            return false;
        return bugCreationQueue[0] != null;
    }

    private void IncreaseHealth()
    {
        if(GetComponent<Health>().GetHealth() + healthIncreasingValue <= GetComponent<Health>().GetMaxHealth())
        {
            GetComponent<Health>().AddHealth(healthIncreasingValue);
        }
    }

    private void CreateUnit()
    {
        Debug.Log($"bugCreationTimeLeft: {bugCreationTimeLeft} - bugCreationPeriod: {bugCreationPeriod}");
        if (bugCreationTimeLeft - bugCreationPeriod <= Mathf.Epsilon)
        {
            if(BugCreationInProgress)
            {
                CreateBug();
                BugCreationInProgress = false;
            }
            SetNewCreationUnit();
        }
        bugCreationTimeLeft -= bugCreationPeriod;
    }
    public bool BugCreationInProgress { get; set; }
    private void SetNewCreationUnit()
    {
        //Debug.Log($"bugCreationQueue[0] != nul: {bugCreationQueue[0] != null} - HasEnoughResource(bugCreationQueue[0].Value): {HasEnoughResource(bugCreationQueue[0].Value)}");
        if (bugCreationQueue[0] != null && HasEnoughResource(bugCreationQueue[0].Value))
        {
            Debug.Log("Create");
            TakeResourceForCreation(bugCreationQueue[0].Value);
            bugCreationTimeLeft = GetCreationTimeOfBug(bugCreationQueue[0].Value);
            BugCreationInProgress = true;
        }
        else
        {
            Debug.Log("NO Create");
            bugCreationTimeLeft = -1;
        }    
    }

    private void TakeResourceForCreation(BugTypeEnum bugType)
    {
        var resourceTypeNeeded = bugPrefabs[bugPrefabKeys.ToList().IndexOf(bugType)].GetComponent<Bug>().GetBugCreationResoucreType();
        var resourceAmmountNeeded = bugPrefabs[bugPrefabKeys.ToList().IndexOf(bugType)].GetComponent<Bug>().GetBugCreationResourceAmmount();

        var resourceTypes = GetResourceTypes();
        ownedResourceAmmount[resourceTypes.IndexOf(resourceTypeNeeded)] -= resourceAmmountNeeded;
    }

    public void AddResource(ResourceTypeEnum resourceType, float ammount)
    {
        var resourceTypes = GetResourceTypes();
        ownedResourceAmmount[resourceTypes.IndexOf(resourceType)] += ammount;
    }

    private bool HasEnoughResource(BugTypeEnum bugType)
    {
        var resourceTypeNeeded = bugPrefabs[bugPrefabKeys.ToList().IndexOf(bugType)].GetComponent<Bug>().GetBugCreationResoucreType();
        var resourceAmmountNeeded = bugPrefabs[bugPrefabKeys.ToList().IndexOf(bugType)].GetComponent<Bug>().GetBugCreationResourceAmmount();

        var result = false;
        var resourceTypes = GetResourceTypes();
        for (int i = 0; i < ownedResourceAmmount.Length; i++)
        {
            if (resourceTypes[i]== resourceTypeNeeded && ownedResourceAmmount[i] - resourceAmmountNeeded >= 0)
            {
                return true;
            }
        }
        return result;
    }

    private float GetCreationTimeOfBug(BugTypeEnum bugType)
    {
        return bugPrefabs[bugPrefabKeys.ToList().IndexOf(bugType)].GetComponent<Bug>().GetBugCreationTime();
    }

    private void CreateBug()
    {
        var bugType = bugCreationQueue[0];
        GameObject bug = CreateBug(bugType.Value);
        ShiftQueue();
    }

    private void ShiftQueue()
    {
        for (int i = 0; i < bugCreationQueue.Length - 1; i++)
        {
            bugCreationQueue[i] = bugCreationQueue[i + 1];
        }
        bugCreationQueue[bugCreationQueue.Length - 1] = null;
    }

    private GameObject CreateBug(BugTypeEnum bugType)
    {
        if (castleState != CastleStateEnum.Destroyed)
        {
            var bug = Instantiate(bugPrefabs[bugPrefabKeys.ToList().IndexOf(bugType)], gameObject.transform.position - new Vector3(2, 0, 0), Quaternion.identity);
            bug.GetComponent<SelectableObject>().SetOwnerId(GetComponent<SelectableObject>().GetOwnerId());
            return bug;
        }
        return null;
    }

    public CastleStateEnum GetState()
    {
        return castleState;
    }
}
