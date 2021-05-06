using Assets.Scripts.Enums;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class SelectableObject : MonoBehaviour
{
    [SerializeField] protected string ownerId;
    [SerializeField] protected GameObject selectionPrefab;

    protected GameObject selection;
    //protected PlayerManager playerManager;
    protected bool IsSelected;

    protected SingleSelectStateEnum SingleSelectState;
    protected SelectionCoreComponent selectionComponent;

    [SerializeField]
    protected Texture2D menuIcon;
    [SerializeField]
    protected Texture2D menuIconRo;

    protected void Start()
    {
        selectionComponent = FindObjectOfType<SelectionCoreComponent>();
        IsSelected = false;
        SingleSelectState = SingleSelectStateEnum.None;
    }

    protected void Update()
    {
        if (selectionComponent && selectionComponent.GetIsSSelecting())
        {
            if (IsOwnedByPlayer())
            {
                var isSelected = selectionComponent.IsWithinSelectionBounds(gameObject);
                SetIsSelected(isSelected);
            }
            else
            {
                SetIsSelected(false);
            }
        }
        else if (SingleSelectState != SingleSelectStateEnum.None)
        {
            SetIsSelected(SingleSelectState == SingleSelectStateEnum.Selected);
            SingleSelectState = SingleSelectStateEnum.None;
        }
    }

    public Texture2D GetMenuIcon()
    {
        return menuIcon;
    }

    public Texture2D GetMenuIconRo()
    {
        return menuIconRo;
    }

    public GameObject GetSelection()
    {
        return selection;
    }

    public void SetOwnerId(string ownerId)
    {
        this.ownerId = ownerId;
    }

    public bool GetIsSelected()
    {
        return IsSelected;
    }

    public string GetOwnerId()
    {
        return ownerId;
    }

    public bool IsOwnedByPlayer()
    {
        return FindObjectOfType<PlayerManager>().IsPlayerId(ownerId);
    }

    protected virtual int GetId()
    {
        throw new NotImplementedException("No GetId() for SelectedObject");
    }
    public void SetIsSelected(bool isSelected)
    {
        var id = GetId();
        if (isSelected)
        {
            var isObjectSelected = selectionComponent.SetSelected(id, gameObject);
            if (!selection && isObjectSelected)
            {
                IsSelected = isSelected;
                SetIconCore();
                FindObjectOfType<SelectionControl>().AddSelectedObject(gameObject);
                selection = GetSelectionGamObject();
                selectionComponent.RefreshSelectedIcons();
            }
        }
        else
        {
            if (selection)
            {
                RemoveIcon();
                selectionComponent.RemoveSelected(id, gameObject);
                Destroy(selection);
                FindObjectOfType<SelectionControl>().CleanSelectableObject(gameObject);
                IsSelected = false;
                selectionComponent.RefreshSelectedIcons();
            }
        }
    }

    protected virtual GameObject GetSelectionGamObject()
    {
        return Instantiate(selectionPrefab, transform.position, Quaternion.identity);
    }

    protected virtual void RemoveIcon()
    {
        //throw new NotImplementedException();
    }

    protected void SetIconCore()
    {
        var id = GetId();
        var cs = new CommonService();
        Transform bug = cs.GetChildrenByName(transform, GetIconGameObjectName());
        var sprite = bug.GetComponent<SpriteRenderer>().sprite;
        SetIcons(sprite, id);
    }

    protected virtual void SetIcons(Sprite sprite, int id)
    {
        throw new NotImplementedException();
    }

    protected virtual string GetIconGameObjectName()
    {
        throw new NotImplementedException();
    }

    public void DestroySelection()
    {
        Destroy(selection);
    }

    protected void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0)
            //&& EventSystem.current.IsPointerOverGameObject()
            )
        {
            Debug.LogError("SSelectableObject");
            SingleSelectState = IsSelected ? SingleSelectStateEnum.Deselected : SingleSelectStateEnum.Selected;
        }
    }
}
