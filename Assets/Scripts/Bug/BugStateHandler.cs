using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BugStateHandler : MonoBehaviour
{
    [SerializeField] BugStateEnum state = BugStateEnum.Idle;
    bool? decideAttack;
    [SerializeField] GameObject targetToAttack;
    [SerializeField] GameObject targetToHarvest;
    [SerializeField] GameObject targetCastle;
    [SerializeField] float carriedResource;
    bool requested = false;

    public BugStateEnum GetState()
    {
        return state;
    }
    public void InitState(BugStateEnum value)
    {
        state = BugStateEnum.Idle;
    }
    public void SetState(BugStateEnum value)
    {
        if (state != BugStateEnum.Dead)
        {
            if(value == BugStateEnum.Idle)
            {
                //Debug.Log($"BUG {GetComponent<Bug>().GetId()} CHANGE STATE {state} => {value}");
            }
            else
            {
                //Debug.Log($"BUG {GetComponent<Bug>().GetId()} CHANGE STATE {state} => {value}");
            }            
            state = value;
        }
    }

    public void SetDecideAttack(bool decideAttack)
    {
        this.decideAttack = decideAttack;
        requested = false;
    }
    public void SetNoAttackTarget(GameObject targetToDelete)
    {
        if (targetToDelete && targetToAttack && targetToDelete.GetComponent<Bug>().GetId() == targetToAttack.GetComponent<Bug>().GetId())
        {
            this.targetToAttack = null;
        }
    }
    public void SetTargetToAttack(GameObject targetToAttack)
    {
        this.targetToAttack = targetToAttack;
    }

    public void SetNoTargetToHarvest(GameObject targetToDelete)
    {
        if (targetToDelete && targetToHarvest)
        {
            this.targetToHarvest = null;
        }
    }
    public void SetTargetToHarvest(GameObject targetToHarvest)
    {
        this.targetToHarvest = targetToHarvest;
    }
    public void HandleStates()
    {
        //Debug.Log(state);
        if(GetComponent<Health>().GetHealth() <= 0)
        {
            SetState(BugStateEnum.Dead);
        }

        if (state == BugStateEnum.Move)
        {
            if (GetComponent<BugMovement>().IsArrivedToTargetVector())
            {
                GetComponent<BugActionSet>().Idle();
            }
        }
        else if (state == BugStateEnum.MoveToHarvest)
        {
            if (this.targetToHarvest && new CommonService().IsAnyResource(targetToHarvest))
            {
                var p = this.targetToHarvest.transform.position;
                GetComponent<BugMovement>().Move(new Vector3(p.x, p.y, p.z));

                var myCollider = GetComponent<CapsuleCollider2D>();
                var otherCollider = targetToHarvest.GetComponent<PolygonCollider2D>();
                var currentDistance = Physics2D.Distance(myCollider, otherCollider);
                var harvestDistance = 1;
                if (currentDistance.distance <= harvestDistance)
                {
                    GetComponent<BugActionSet>().Harvest();
                }
            }
            else
            {
                SetState(BugStateEnum.Idle);
            }
        }
        else if (state == BugStateEnum.Harvest)
        {
            if (targetToHarvest && GetComponent<BugActionSet>().IsAnyResource(targetToHarvest))
            {
                GetComponent<ResourceHandler>().HarvestTarget(targetToHarvest);        
            }
            else
            {
                GetComponent<BugActionSet>().Idle();
            }
        }
        else if (state == BugStateEnum.MoveToCastle)
        {
            if (this.targetCastle) //TODO: && castle is still owned
            {
                var p = this.targetCastle.transform.position;
                GetComponent<BugMovement>().Move(new Vector3(p.x, p.y, p.z));

                var myCollider = GetComponent<CapsuleCollider2D>();
                var otherCollider = targetCastle.GetComponent<CapsuleCollider2D>();
                var currentDistance = Physics2D.Distance(myCollider, otherCollider);

                if (currentDistance.distance <= 1)
                {
                    GetComponent<BugActionSet>().UnloadResourceToCastle(targetCastle);
                    GetComponent<BugActionSet>().MoveToHarvest(targetToHarvest);
                }
            }
            else
            {
                SetState(BugStateEnum.Idle);
            }
        }
        else if (state == BugStateEnum.MoveRunAway)
        {
            
            if (GetComponent<BugMovement>().IsArrivedToTargetVector())
            {
                var enemy = GetComponent<BugActionSet>().FindNearestEnemy();
                if (enemy)
                {
                    targetToAttack = enemy;
                    GetComponent<BugActionSet>().RunAway(targetToAttack);
                }
                else
                {
                    GetComponent<BugActionSet>().Idle();
                }
                
            }

        }
        else if(state == BugStateEnum.MoveToAttack)
        {
            if (this.targetToAttack && new CommonService().IsAlive(targetToAttack))
            {
                var p = this.targetToAttack.transform.position;
                GetComponent<BugMovement>().Move(new Vector3(p.x, p.y, p.z));

                var myCollider = GetComponent<CapsuleCollider2D>();
                var otherCollider = targetToAttack.GetComponent<CapsuleCollider2D>();
                var currentDistance = Physics2D.Distance(myCollider, otherCollider);
                var attackDistance = GetComponent<Attack>().GetAttackRadius();

                if (currentDistance.distance <= attackDistance)
                {
                    //Debug.Log($"{gameObject.GetComponent<SelectableObject>().GetOwnerId()}:{gameObject.GetComponent<Bug>().GetId()} - set attack state - {targetToAttack.GetComponent<Bug>().GetId()}");
                    GetComponent<BugActionSet>().Attack();
                }
            }
            else
            {
                SetState(BugStateEnum.Idle);
            }
        }
        else if (state == BugStateEnum.Attack)
        {
            if (targetToAttack && GetComponent<BugActionSet>().IsEnemy(targetToAttack))
            {
                float dmg = 0; 
                var alive = GetComponent<Attack>().AttackTarget(targetToAttack, out dmg);
                if (!alive)
                {                    
                    targetToAttack = null;
                    GetComponent<BugActionSet>().Idle();
                }
            }
            else
            {
                GetComponent<BugActionSet>().Idle();
            }
        }
        else if (state == BugStateEnum.Dead)
        {
        }
        else if (state == BugStateEnum.Attacked)
        {
        }
        else if (state == BugStateEnum.Idle)
        {
            var attackRatherThanHarvest = false;
            if (attackRatherThanHarvest)
            {
                var enemy = GetComponent<BugActionSet>().FindNearestEnemy();
                if (enemy)
                {
                    targetToAttack = enemy;
                    GetComponent<BugActionSet>().MoveToAttack(targetToAttack);
                }
            }
            else
            {
                var resource = GetComponent<BugActionSet>().FindNearestResource();
                if (resource)
                {
                    targetToHarvest = resource;
                    var state = GetComponent<BugStateHandler>().GetState();
                    GetComponent<BugActionSet>().MoveToHarvest(targetToHarvest);
                }
            }            
        }
    }

    internal void SetTargetCastle(GameObject birthCastle)
    {
        targetCastle = birthCastle;
    }
}
