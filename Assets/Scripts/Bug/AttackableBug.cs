using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AttackableBug : MonoBehaviour
{
    [SerializeField] BugTypeEnum BugType;

    private void OnMouseOver()
    {
        var selectedObjects = FindObjectOfType<SelectionControl>().GetSelectedObjects();
        if (selectedObjects.Any() && IsEnemy(selectedObjects.First()))
        {
            if (new CommonService().GetCursorState() != CursorStateEnum.Attack)
            {
                new CommonService().SetCursorState(CursorStateEnum.Attack, FindObjectOfType<Config>().GetAttackCursorTexture());
            }else if (Context.IsAttack())
            {
                Context.ActivateAttack();
            }
            if (!Context.IsContextActive() && Input.GetMouseButtonDown(1))
            {
                //TODO: context-ben félrekattint, akkor legyen lezárva
                //TODO: context-ben jobb-al kattint, legyen szintén lezárva
                FindObjectOfType<SelectionControl>().Attack(gameObject);
                Context.FinishContext();
            }
            if (Context.IsContextActive() && Context.IsAttack() && Input.GetMouseButtonDown(0))
            {
                FindObjectOfType<SelectionControl>().Attack(gameObject);
                Context.FinishContext();
            }
        }
    }

    private void OnMouseExit()
    {
        if (Context.IsAttack())
        {
            Context.InactivateAttack();
        }
    }

    public BugTypeEnum GetBugType()
    {
        return BugType;
    }
    private bool IsEnemy(GameObject attacker)
    {
        return new CommonService().AreEnemies(gameObject, attacker);
    }
}
