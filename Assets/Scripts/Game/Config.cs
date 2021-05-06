using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Config : MonoBehaviour
{
    [SerializeField] float movementScale = 1f;
    [SerializeField] float attackPeriodicyScale = 1f;
    [SerializeField] float harvestPeriodicyScale = 1f;

    [SerializeField] Texture2D attackCursorTexture;
    [SerializeField] Texture2D defaultCursorTexture;
    [SerializeField] Texture2D iconSelectionCursorTexture;


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

    public Texture2D GetDefaultCursorTexture()
    {
        return defaultCursorTexture;
    }
    public Texture2D GetAttackCursorTexture()
    {
        return attackCursorTexture;
    }
    public Texture2D GetIconSelectionCursorTexture()
    {
        return iconSelectionCursorTexture;
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
