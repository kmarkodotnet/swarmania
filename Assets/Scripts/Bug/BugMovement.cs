using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BugMovement : MonoBehaviour
{
    [SerializeField] float MovementSpeed = 1f;
    private Vector3 movementTargetVector;
    private static List<BugStateEnum> BugMoveStates = new List<BugStateEnum> { BugStateEnum.Move, BugStateEnum.MoveRunAway, BugStateEnum.MoveToAttack, BugStateEnum.MoveToHarvest, BugStateEnum.MoveToCastle };
    public void SetMovementTargetVector(Vector3 vector)
    {
        movementTargetVector = vector;
    }

    public void Stop()
    {
        var p = gameObject.transform.position;
        var target = new Vector2(p.x, p.y);
        Move(target);
        GetComponent<BugStateHandler>().SetState(BugStateEnum.Idle);
    }

    public void Move(Vector2 target)
    {
        var s = GetComponent<BugStateHandler>().GetState();
        if (BugMoveStates.Contains(s))
        {
            this.movementTargetVector = target;
            GetComponent<BugMovement>().SetTargetPosition(movementTargetVector);
            GetComponent<BugMovement>().TurnUnitToCorrectDirection(gameObject.transform);
        }
    }

    public bool IsArrivedToTargetVector()
    {
        Vector3 myPos = Camera.main.WorldToScreenPoint(gameObject.transform.position);
        var targetPos = Camera.main.WorldToScreenPoint(movementTargetVector);
        //Debug.Log($"BUG {GetComponent<Bug>().GetId()}  - tr.pos {gameObject.transform.position} movementTargetVec {movementTargetVector} myPos {myPos} targetPos {targetPos}");
        var diff = targetPos - myPos;
        var attackDistance = GetComponent<Attack>().GetAttackRadius();
        return Mathf.Abs(diff.x) < attackDistance && Mathf.Abs(diff.y) < attackDistance;
    }

    public void SetUnitPositions(Transform unit)
    {
        var state = GetComponent<BugStateHandler>().GetState();
        if (BugMoveStates.Contains(state))
        {
            if (unit != null && unit.position != null && movementTargetVector != null)
            {
                var currentSpeed = (Constants.TimeScale * MovementSpeed * FindObjectOfType<Config>().GetMovementScale() / 7) * Time.deltaTime;
                var diff = unit.position - movementTargetVector;
                
                if (diff.magnitude > Mathf.Epsilon)
                {
                    unit.position = Vector3.MoveTowards(unit.position, movementTargetVector, currentSpeed);
                    GetComponent<SelectableBug>().SetPosition(unit.position);
                    TurnUnitToCorrectDirection(gameObject.transform);
                }
            }
        }
    }

    private void TurnUnitToCorrectDirection(Transform unit)
    {
        Vector3 myPos = Camera.main.WorldToScreenPoint(unit.position);
        var enemyPos = Camera.main.WorldToScreenPoint(movementTargetVector);

        Vector3 dir = enemyPos - myPos;
        
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        var currentSpeed = (Constants.TimeScale * MovementSpeed * FindObjectOfType<Config>().GetMovementScale()) * Time.deltaTime;

        unit.rotation = Quaternion.Lerp(unit.rotation, Quaternion.AngleAxis(angle, Vector3.forward), currentSpeed);
            //AngleAxis(angle, Vector3.forward);
    }

    public void SetTargetPosition(Vector3 targetVector)
    {
        targetVector.z = gameObject.transform.position.z;
    }

}
