using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Config : MonoBehaviour
{
    [SerializeField] float movementScale = 1f;
    [SerializeField] float attackPeriodicyScale = 1f;
    [SerializeField] float harvestPeriodicyScale = 1f;

    [SerializeField] Texture2D attackCursorTexture;
    [SerializeField] Texture2D moveCursorTexture;
    [SerializeField] Texture2D harvestCursorTexture;

    [SerializeField] Texture2D inactiveAttackCursorTexture;
    [SerializeField] Texture2D inactiveMoveCursorTexture;
    [SerializeField] Texture2D inactiveHarvestCursorTexture;

    [SerializeField] Texture2D defaultSelectionCursorTexture;

    public static Texture2D moveCursorTextureStatic;
    public static Texture2D attackCursorTextureStatic;
    public static Texture2D harvestCursorTextureStatic;

    public static Texture2D inactiveMoveCursorTextureStatic;
    public static Texture2D inactiveAttackCursorTextureStatic;
    public static Texture2D inactiveHarvestCursorTextureStatic;

    public static Texture2D defaultSelectionCursorTextureStatic;

    private void Awake()
    {
        var c = FindObjectsOfType<AiBreeder>().Length;
        if (c > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }
    private void Start()
    {
        moveCursorTextureStatic = moveCursorTexture;
        attackCursorTextureStatic = attackCursorTexture;
        harvestCursorTextureStatic = harvestCursorTexture;

        inactiveMoveCursorTextureStatic = inactiveMoveCursorTexture;
        inactiveAttackCursorTextureStatic = inactiveAttackCursorTexture;
        inactiveHarvestCursorTextureStatic = inactiveHarvestCursorTexture;

        defaultSelectionCursorTextureStatic = defaultSelectionCursorTexture;

        var cs = new CommonService();
        cs.SetCursorState(CursorStateEnum.Default, defaultSelectionCursorTexture);
    }

    public Texture2D GetMoveCursorTexture()
    {
        return moveCursorTexture;
    }
    public Texture2D GetAttackCursorTexture()
    {
        return attackCursorTexture;
    }

    public Texture2D GetHarvestCursorTexture()
    {
        return harvestCursorTexture;
    }
    public Texture2D GetDefaultCursorTexture()
    {
        return defaultSelectionCursorTexture;
    }

    public float GetMovementScale()
    {
        return movementScale;
    }
    public float GetAttackPeriodicyScale()
    {
        return attackPeriodicyScale;
    }
    public float GetHarvestkPeriodicyScale()
    {
        return harvestPeriodicyScale;
    }
}
