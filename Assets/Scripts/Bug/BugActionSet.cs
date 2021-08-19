using System.Linq;
using UnityEngine;

public class BugActionSet : MonoBehaviour
{

    GameObject nearestEnemy;
    GameObject nearestResource;

    void Start()
    {
        GetComponent<BugMovement>().SetMovementTargetVector(gameObject.transform.position);
    }

    private void Update()
    {
        var unit = gameObject.transform;

        GetComponent<BugMovement>().SetUnitPositions(unit);

        GetComponent<BugStateHandler>().HandleStates();
        
    }

    public void RunAway(GameObject nearestEnemy)
    {
        var enemyPos = nearestEnemy.transform.position;
        var myPos = gameObject.transform.position;
        var diff = myPos - enemyPos;
        var max = Mathf.Max(Mathf.Abs(diff.x), Mathf.Abs(diff.y));
        var normalizedDiff = new Vector3(diff.x / max, diff.y / max, diff.z / max);
        var correctedDiff = normalizedDiff * 3;

        BaseRunAway(myPos + correctedDiff);

    }
    public void RunAway(Vector3 directionVector)
    {
        var myPos = gameObject.transform.position;
        var max = Mathf.Max(Mathf.Abs(directionVector.x), Mathf.Abs(directionVector.y));
        var normalizedDiff = new Vector3(directionVector.x / max, directionVector.y / max, directionVector.z);
        var correctedDiff = normalizedDiff * GetComponent<LivingObject>().GetLookDistance() * 1.5f;
        BaseRunAway(myPos + correctedDiff);
    }

    public GameObject FindNearestEnemy()
    {
        nearestEnemy = null;
        nearestEnemy = new CommonService().FindNearestEnemy(gameObject);
        return nearestEnemy;
    }

    public GameObject FindNearestResource()
    {
        nearestResource = null;
        nearestResource = new CommonService().FindNearestResource(gameObject);
        return nearestResource;
    }

    public bool IsEnemy(GameObject otherGameObject)
    {
        return new CommonService().AreEnemies(gameObject, otherGameObject);
    }
    public bool IsAnyResource(GameObject targetResource)
    {
        return new CommonService().IsAnyResource(targetResource);
    }

    public void Attacked()
    {
        GetComponent<BugStateHandler>().SetState(BugStateEnum.Attacked);
    }

    public void Dead()
    {
        //if (GetComponent<BugStateHandler>().GetState() != BugStateEnum.Dead)
        {
            GetComponent<BugStateHandler>().SetState(BugStateEnum.Dead);
            GetComponent<BugAppearance>().SetHidden();
        }
    }

    public void Idle()
    {
        GetComponent<BugStateHandler>().SetState(BugStateEnum.Idle);
    }

    public void Attack(GameObject localTargetToAttack = null)
    {
        if (localTargetToAttack)
        {
            GetComponent<BugStateHandler>().SetTargetToAttack(localTargetToAttack);
        }
        GetComponent<BugStateHandler>().SetState(BugStateEnum.Attack);
    }

    public void UnloadResourceToCastle(GameObject castle)
    {
        if (castle)
        {
            var carriedResourceAmmount = GetComponent<ResourceHandler>().EmptyCarriedResourceAmmount();
            var carriedResourceType = GetComponent<ResourceHandler>().GetCarriedResourceType();
            castle.GetComponent<CastleStateHandler>().AddResource(carriedResourceType, carriedResourceAmmount);
        }
    }

    public void MoveToHarvest(GameObject targetToHarcest)
    {
        //Debug.Log($"{gameObject.GetComponent<SelectableObject>().GetOwnerId()}:{gameObject.GetComponent<Bug>().GetId()} - move attack - {targetToAttack.GetComponent<Bug>().GetId()}");
        GetComponent<BugStateHandler>().SetTargetToHarvest(targetToHarcest);
        if (targetToHarcest && new CommonService().IsAnyResource(targetToHarcest))
        {
            var p = targetToHarcest.transform.position;
            var moveVector = new Vector3(p.x, p.y, p.z);
            GetComponent<BugStateHandler>().SetState(BugStateEnum.MoveToHarvest);
            GetComponent<BugMovement>().Move(moveVector);
        }
        else
        {
            Idle();
        }
    }

    public void MoveToAttack(GameObject targetToAttack)
    {
        GetComponent<BugStateHandler>().SetTargetToAttack(targetToAttack);
        if (targetToAttack && new CommonService().IsAlive(targetToAttack))
        {
            var p = targetToAttack.transform.position;
            var moveVector = new Vector3(p.x, p.y, p.z);
            GetComponent<BugStateHandler>().SetState(BugStateEnum.MoveToAttack);
            GetComponent<BugMovement>().Move(moveVector);
        }
        else
        {
            Idle();
        }
    }
    public void MoveToCastle(GameObject castle)
    {
        if (castle)
        {
            var p = castle.transform.position;
            var moveVector = new Vector3(p.x, p.y, p.z);
            GetComponent<BugStateHandler>().SetState(BugStateEnum.MoveToCastle);
            GetComponent<BugMovement>().Move(moveVector);
        }
        else
        {
            Idle();
        }
    }

    public void BaseRunAway(Vector2 target)
    {
        GetComponent<BugStateHandler>().SetState(BugStateEnum.MoveRunAway);
        GetComponent<BugMovement>().Move(target);
    }
    public void Move(Vector2 target)
    {
        GetComponent<BugStateHandler>().SetState(BugStateEnum.Move);
        GetComponent<BugMovement>().Move(target);
    }

    internal void Harvest()
    {
        GetComponent<BugStateHandler>().SetState(BugStateEnum.Harvest);
    }

    internal void ApplyStrategy(StrategyEnum strategy, Vector3 currentPosition)
    {
        float distance = GetDistanceBasedOnStrategy(strategy);
        var a = Random.Range(0, 360);
        var xp = Mathf.Sin(a) * distance;
        var yp = Mathf.Cos(a) * distance;
        var xn = currentPosition.x + xp;
        var yn = currentPosition.y + yp;
        this.Move(new Vector2() { x = xn, y = yn });
    }

    private float GetDistanceBasedOnStrategy(StrategyEnum strategy)
    {
        switch (strategy)
        {
            case StrategyEnum.Defensive:
                return 5f;
            case StrategyEnum.Offensive:
                return 10f;
            case StrategyEnum.Scout:
                return 15f;
            default:
                throw new System.Exception("No strategy");
        }
    }
}