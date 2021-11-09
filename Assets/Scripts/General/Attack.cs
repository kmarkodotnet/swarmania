using System;
using UnityEngine;

public class Attack : MonoBehaviour
{
    [SerializeField] int damage = 10;
    [SerializeField] float attackRadius = 0.1f;
    [SerializeField] float periodicity = 1f;
    [SerializeField] float lookDistance = 5f;

    bool spawn = true;

    DateTime lastChecked;

    private void Start()
    {
        lastChecked = DateTime.Now;
    }

    public float GetAttackRadius()
    {
        return attackRadius;
    }

    public float GetDamage()
    {
        return damage;
    }
    internal bool AttackTarget(GameObject targetToAttack, out float dmg)
    {
        dmg = damage;
        return AttackTarget(targetToAttack);
    }

    internal bool AttackTarget(GameObject targetToAttack)
    {
        if((DateTime.Now-lastChecked).TotalSeconds > ((periodicity*FindObjectOfType<Config>().GetAttackPeriodicyScale()) / Constants.TimeScale) && targetToAttack)
        {
            var isAlive = new CommonService().IsAlive(targetToAttack);
            if (!isAlive)
            {
                GetComponent<BugActionSet>().Idle();
                return false;
            }
            var isStillAlive = targetToAttack.GetComponent<Health>().DealDamage(damage);
            lastChecked = DateTime.Now;
            if (!isStillAlive)
            {
                targetToAttack = GetComponent<BugActionSet>().FindNearestEnemy();
                GetComponent<BugActionSet>().MoveToAttack(targetToAttack);
            }
            else
            {
                gameObject.GetComponent<BugActionSet>().Attack();
            }
            return isStillAlive;
        }
        return true;
    }
}
