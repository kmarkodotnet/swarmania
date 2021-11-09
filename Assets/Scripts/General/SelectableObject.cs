using Assets.Scripts.Enums;
using System;
using UnityEngine;

public class SelectableObject : MonoBehaviour
{
    [SerializeField] protected string ownerId;
    [SerializeField] protected GameObject selectionPrefab;

    protected GameObject selection;
    protected bool IsSelected;

    protected SingleSelectStateEnum SingleSelectState;
    protected SelectionCoreComponent selectionCoreComponent;

    [SerializeField]
    protected Sprite menuIcon;
    [SerializeField]
    protected Sprite menuIconRo;

    protected void Start()
    {
        selectionCoreComponent = FindObjectOfType<SelectionCoreComponent>();
        IsSelected = false;
        SingleSelectState = SingleSelectStateEnum.None;
    }

    protected void Update()
    {
        if (selectionCoreComponent && selectionCoreComponent.GetIsSSelecting())
        {
            if (IsOwnedByPlayer())
            {
                var isSelected = selectionCoreComponent.IsWithinSelectionBounds(gameObject);
                SetIsSelected(isSelected);
            }
            else
            {
                SetIsSelected(false);
            }
        }
        else if (SingleSelectState != SingleSelectStateEnum.None)
        {
            Debug.Log("SELECT");
            SetIsSelected(SingleSelectState == SingleSelectStateEnum.Selected);
            SingleSelectState = SingleSelectStateEnum.None;
        }
    }

    public Sprite GetMenuIcon()
    {
        return menuIcon;
    }

    public Sprite GetMenuIconRo()
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
            var isObjectSelected = selectionCoreComponent.SetSelected(id, gameObject);
            if (!selection && isObjectSelected)
            {
                IsSelected = isSelected;
                SetIconCore();
                FindObjectOfType<SelectionControl>().AddSelectedObject(gameObject);
                selection = GetSelectionGamObject();
                selectionCoreComponent.RefreshSelectedIcons();
            }if (selection && !isObjectSelected)
            {
                RemoveSelectableObjectSelection(id);
            }
        }
        else
        {
            if (selection)
            {
                RemoveSelectableObjectSelection(id);
            }
        }
    }

    private void RemoveSelectableObjectSelection(int id)
    {
        RemoveIcon();
        selectionCoreComponent.RemoveSelected(id, gameObject);
        Destroy(selection);
        FindObjectOfType<SelectionControl>().CleanSelectableObject(gameObject);
        IsSelected = false;
        selectionCoreComponent.RefreshSelectedIcons();
    }

    protected virtual GameObject GetSelectionGamObject()
    {
        return Instantiate(selectionPrefab, transform.position, Quaternion.identity);
    }

    protected virtual void RemoveIcon()
    {

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
            SingleSelectState = IsSelected ? SingleSelectStateEnum.Deselected : SingleSelectStateEnum.Selected;
        }
    }
}
