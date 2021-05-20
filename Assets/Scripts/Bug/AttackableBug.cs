﻿using System;
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
            if (!Context.IsContextActive() && new CommonService().GetCursorState() != CursorStateEnum.Attack)
            {
                new CommonService().SetCursorState(CursorStateEnum.Attack, FindObjectOfType<Config>().GetAttackCursorTexture());
            }
            if (!Context.IsContextActive() && Input.GetMouseButtonDown(1))
            {
                FindObjectOfType<SelectionControl>().Attack(gameObject);
            }
            if (Context.IsContextActive() && Input.GetMouseButtonDown(0))
            {
                FindObjectOfType<SelectionControl>().Attack(gameObject);
                Context.FinishContext();
            }
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
