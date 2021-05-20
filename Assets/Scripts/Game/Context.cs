public static class Context
{
    private static bool isContextActive { get; set; }
    private static bool isMove { get; set; }
    private static bool isAttack { get; set; }
    private static bool isHarvest { get; set; }

    public static bool IsContextActive()
    {
        return isContextActive;
    }
    public static void FinishContext()
    {
        isMove = false;
        isAttack = false;
        isHarvest = false;
        isContextActive = false;
        var cs = new CommonService();
        cs.SetCursorState(CursorStateEnum.Move, Config.moveCursorTextureStatic);
    }

    public static void Move()
    {
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
        return isContextActive && isMove;
    }

    public static void Attack()
    {
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
        return isContextActive && isAttack;
    }

    public static void Harvest()
    {
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
        return isContextActive && isHarvest;
    }
}