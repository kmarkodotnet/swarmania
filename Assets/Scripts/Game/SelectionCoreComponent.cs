using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SelectionCoreComponent : MonoBehaviour
{
    [SerializeField] float selectionEpsilon = 0.05f;

    bool isSelecting = false;
    Vector3 mousePosition1;
    bool isSelected = false;
    Dictionary<int, GameObject> selectedBugs;

    GameObject selectedCastle;


    private void Awake()
    {
        var c = FindObjectsOfType<SelectionCoreComponent>().Length;
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
        selectedBugs = new Dictionary<int, GameObject>();
        selectedCastle = null;
    }
    private void OnMouseUp()
    {
        if (ControlContext.IsMove())
        {
            Debug.Log("lets move");
            Move();
        }
    }

    private void OnMouseOver()
    {
        if (ControlContext.IsMove())
        {
            ControlContext.ActivateMove();
        }
    }
    private void OnMouseExit()
    {
        if (ControlContext.IsMove())
        {
            ControlContext.InactivateMove();
        }
    }

    void Update()
    {
        if (!ControlContext.IsContextActive())
        {
            if (!Control.ControlClickLocked)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    selectedBugs = new Dictionary<int, GameObject>();
                    selectedCastle = null;
                    isSelecting = true;
                    mousePosition1 = Input.mousePosition;

                }
                if (Input.GetMouseButtonUp(0))
                {
                    isSelecting = false;
                    RefreshSelectedIcons();
                }
            }
            else
            {
                Control.ControlClickLocked = false;
            }
        }       

    }

    public void RefreshSelectedIcons()
    {
        if (selectedBugs.Count <= 0)
        {
            FindObjectOfType<Control>().RemoveBugIcons();
        }
        else if(selectedBugs.Count > 0)
        {
            FindObjectOfType<Control>().SetBugIcons(selectedBugs);
        }else if(selectedCastle == null)
        {
            FindObjectOfType<Control>().RemoveCastleIcon();
        }

        if(selectedCastle != null)
        {
            FindObjectOfType<Control>().SetControl();
        }
    }

    public void Stop()
    {
        var bugs = selectedBugs.Values;
        foreach (var bug in bugs)
        {
            bug.GetComponent<BugMovement>().Stop();
        }
    }

    public void Move()
    {
        FindObjectOfType<SelectionControl>().Move();
        ControlContext.FinishContext();
    }

    void OnGUI()
    {
        if (isSelecting)
        {
            var rect = Utils.GetScreenRect(mousePosition1, Input.mousePosition);
            Utils.DrawScreenRect(rect, new Color(0.8f, 0.8f, 0.95f, 0.25f));
            Utils.DrawScreenRectBorder(rect, 2, new Color(0.8f, 0.8f, 0.95f));
            
        }
        //if (set.Count > 0 && !isSelecting)
        //{
        //    var rect2 = Utils.GetScreenRect(Camera.main.ViewportToScreenPoint(new Vector3(0.1f, 0.06f, 0)), Camera.main.ViewportToScreenPoint(new Vector3(0.9f, 0.2f, 0)));
        //    Utils.DrawScreenRect(rect2, new Color(0.8f, 0.2f, 0.15f, 0.75f));
        //    Utils.DrawScreenRectBorder(rect2, 3, new Color(0.3f, 0.1f, 0.15f));
        //}
    }

    internal bool SetSelected(int id, GameObject gameObject)
    {
        if(gameObject.GetComponent<SelectableCastle>() && selectedBugs.Count <= 0)
        {
            selectedCastle = gameObject;
            return true;
        }
        else if (gameObject.GetComponent<SelectableCastle>() && selectedBugs.Count > 0)
        {
            selectedCastle = null;
            return false;
        }
        else if (gameObject.GetComponent<SelectableBug>() && selectedBugs.Count > 0 && !gameObject.GetComponent<SelectableBug>().IsOwnedByPlayer())
        {
            return false;
        }
        else if(gameObject.GetComponent<SelectableBug>()  && selectedBugs.Count < 20)
        {
            if (!selectedBugs.ContainsKey(id))
                selectedBugs.Add(id, gameObject);
            return true;
        }
        return false;
    }
    internal void RemoveSelected(int id, GameObject gameObject)
    {
        if (gameObject.GetComponent<SelectableCastle>())
        {
            selectedCastle = null;
        }
        else
        {
            selectedBugs.Remove(id);
        }        
    }

    public bool GetIsSSelecting()
    {
        return isSelecting;
    }

    public bool IsWithinSelectionBounds(GameObject gameObject)
    {
        if (!isSelecting)
            return false;

        var camera = Camera.main;
        var viewportBounds =
            Utils.GetViewportBounds(camera, mousePosition1, Input.mousePosition);
        var c = gameObject.transform.position;
        
        c = Camera.main.WorldToViewportPoint(c);
        c.z = viewportBounds.extents.z;
        var isSelected =  viewportBounds.Contains(c);

        if (!isSelected && viewportBounds.extents.x < Mathf.Epsilon && viewportBounds.extents.y < Mathf.Epsilon )
        {
            var bs = gameObject.GetComponent<CapsuleCollider2D>().bounds;
            var p = viewportBounds.center;
            p = Camera.main.ViewportToWorldPoint(p);
            p.z = bs.extents.z;
            isSelected = bs.Contains(p);
        }

        return isSelected;
    }
}