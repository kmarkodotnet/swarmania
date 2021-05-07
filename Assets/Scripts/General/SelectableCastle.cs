using Assets.Scripts.Enums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectableCastle : SelectableObject
{
    [SerializeField] Vector3 selectionOffset;
    [SerializeField] float selectionScale = 1;
    protected override int GetId()
    {
        return gameObject.GetComponent<Castle>().GetId();
    }

    protected override GameObject GetSelectionGamObject()
    {
        var instance = Instantiate(selectionPrefab, transform.position + selectionOffset, Quaternion.identity);
        instance.transform.localScale = new Vector3(selectionScale, selectionScale, 1);
        return instance;
    }

    protected override void SetIcons(Sprite sprite, int id)
    {
        FindObjectOfType<Control>().SetupCastleIcon(sprite);
        FindObjectOfType<Control>().SetupCastleBugs(gameObject);
    }

    protected override string GetIconGameObjectName()
    {
        return "CastleTop";
    }

    protected override void RemoveIcon()
    {
        //FindObjectOfType<Control>().RemoveCastleIcon();
        //FindObjectOfType<Control>().ClearCastleBugs();        
    }
}
