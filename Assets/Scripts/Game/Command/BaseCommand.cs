﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseCommand : MonoBehaviour
{
    [SerializeField] 
    protected CommandTypeEnum CommandType;

    [SerializeField] protected Sprite sprite;
    [SerializeField] protected Sprite spriteHover;
    //[SerializeField]
    //public ICommandClick click;

    //public ICommandClick clickV
    //{
    //    get { return click; }
    //    set { click = value; }
    //}

    public void SetSprite(Sprite sprite)
    {
        this.sprite = sprite;
    }

    public void SetSpriteHover(Sprite spriteHover)
    {
        this.spriteHover = spriteHover;
    }

    protected void Update()
    {
    }

    public void OnMouseUp()
    {
        //Click();
    }

    protected void OnMouseOver()
    {
        Debug.Log("spriteHover " + spriteHover);
        gameObject.GetComponent<SpriteRenderer>().sprite = spriteHover;
        if (Input.GetMouseButtonDown(0) && !Control.ControlClickLocked)
        {
            Click();
            Control.ControlClickLocked = true;
        }
    }

    protected void OnMouseExit()
    {
        gameObject.GetComponent<SpriteRenderer>().sprite = sprite;
    }

    public void SetCommandType(CommandTypeEnum commandType)
    {
        CommandType = commandType;
    }
    public CommandTypeEnum GetCommandType()
    {
        return CommandType;
    }

    public virtual void Click()
    {
        Control.ControlClickLocked = true;
        switch (CommandType)
        {
            case CommandTypeEnum.BugMove:
                Move();
                break;
            case CommandTypeEnum.BugStop:
                Stop();
                break;
            case CommandTypeEnum.BugAttack:
                Attack();
                break;
            case CommandTypeEnum.BugHarvest:
                Harvest();
                break;
            case CommandTypeEnum.CastleCreateBug1:
                CreateBug1();
                break;
            case CommandTypeEnum.CastleCreateBug2:
                CreateBug2();
                break;
            case CommandTypeEnum.CastleCreateBug3:
                CreateBug3();
                break;
            default:
                break;
        }   
    }

    private void CreateBug3()
    {
        Debug.Log("CreateBug3");
    }

    private void CreateBug2()
    {
        Debug.Log("CreateBug2");
    }

    private void CreateBug1()
    {
        Debug.Log("CreateBug1");
    }

    private void Harvest()
    {
        Context.Harvest();
    }

    private void Attack()
    {
        Context.Attack();
    }

    private void Stop()
    {
        FindObjectOfType<SelectionCoreComponent>().Stop();
        Context.FinishContext();
    }

    private void Move()
    {
        Context.Move();
    }
}
