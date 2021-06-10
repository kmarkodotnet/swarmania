using System;
using UnityEngine;

public static class Context
{
    private static bool isContextActive { get; set; }
    private static bool isMove { get; set; }
    private static bool isAttack { get; set; }
    private static bool isHarvest { get; set; }

    private static GameObject castle { get; set; }
    private static GameObject[] castleBugPrefabs { get; set; }

    internal static void SetupCastle(GameObject gameObject, GameObject[] bugPrefabs)
    {
        castle = gameObject;
        castleBugPrefabs = bugPrefabs;
    }

    internal static void CreateBug(int bugNum)
    {
        var bugType = castleBugPrefabs[bugNum].GetComponent<AttackableBug>().GetBugType();
        castle.GetComponent<CastleStateHandler>().AddNewBugToQueue(bugType);
    }

    public static bool IsContextActive()
    {
        return isContextActive;
    }
    public static void FinishContext()
    {
        ResetVariables();
        var cs = new CommonService();
        cs.SetCursorState(CursorStateEnum.Move, Config.moveCursorTextureStatic);
    }

    private static void ResetVariables()
    {
        isMove = false;
        isAttack = false;
        isHarvest = false;
        isContextActive = false;
    }

    public static void Move()
    {
        ResetVariables();
        isMove = true;
        isContextActive = true;
        InactivateMove();
    }
    public static void ActivateMove()
    {
        var cs = new CommonService();
        cs.SetCursorState(CursorStateEnum.Move, Config.moveCursorTextureStatic);
    }
    public static void InactivateMove()
    {
        var cs = new CommonService();
        cs.SetCursorState(CursorStateEnum.Move, Config.inactiveMoveCursorTextureStatic);
    }
    public static bool IsMove()
    {
        return IsContextActive() && isMove;
    }

    public static void Attack()
    {
        ResetVariables();
        isAttack = true;
        isContextActive = true;
        InactivateAttack();
    }
    public static void ActivateAttack()
    {
        var cs = new CommonService();
        cs.SetCursorState(CursorStateEnum.Attack, Config.attackCursorTextureStatic);
    }
    public static void InactivateAttack()
    {
        var cs = new CommonService();
        cs.SetCursorState(CursorStateEnum.Attack, Config.inactiveAttackCursorTextureStatic);
    }
    public static bool IsAttack()
    {
        return IsContextActive() && isAttack;
    }

    public static void Harvest()
    {
        ResetVariables();
        isHarvest = true;
        isContextActive = true;
        InactivateHarvest();
    }
    public static void ActivateHarvest()
    {
        var cs = new CommonService();
        cs.SetCursorState(CursorStateEnum.Harvest, Config.harvestCursorTextureStatic);
    }

    public static void InactivateHarvest()
    {
        var cs = new CommonService();
        cs.SetCursorState(CursorStateEnum.Harvest, Config.inactiveHarvestCursorTextureStatic);
    }

    public static bool IsHarvest()
    {
        return IsContextActive() && isHarvest;
    }
}