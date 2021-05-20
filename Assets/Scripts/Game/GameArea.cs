using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameArea : MonoBehaviour
{

    private Vector3 mousePosition;

    public float dragSpeed = 2;
    private Vector3 dragOrigin;


    //private void Awake()
    //{
    //    var c = FindObjectsOfType<GameArea>().Length;
    //    if (c > 1)
    //    {
    //        Destroy(gameObject);
    //    }
    //    else
    //    {
    //        DontDestroyOnLoad(gameObject);
    //    }
    //}

    void OnGUI()
    {
        //Utils.DrawScreenRect(new Rect(320, 32, 256, 128), new Color(0.8f, 0.8f, 0.95f, 0.25f));
        //Utils.DrawScreenRectBorder(new Rect(320, 32, 256, 128), 2, new Color(0.8f, 0.8f, 0.95f));
    }

    private void OnMouseOver()
    {
        if (!Context.IsContextActive() && new CommonService().GetCursorState() != CursorStateEnum.Move && FindObjectOfType<SelectionControl>().GetSelectedObjects().Any())
        {
            Debug.LogError("move");
            new CommonService().SetCursorState(CursorStateEnum.Move, FindObjectOfType<Config>().GetDefaultCursorTexture());
        }else if(!Context.IsContextActive() && new CommonService().GetCursorState() != CursorStateEnum.Default && !FindObjectOfType<SelectionControl>().GetSelectedObjects().Any())
        {
            Debug.LogError("default");
            new CommonService().SetCursorState(CursorStateEnum.Move, Config.defaultSelectionCursorTextureStatic);
        }
        if (Input.GetMouseButtonDown(1))
        {
            Debug.Log("move");
            FindObjectOfType<SelectionControl>().Move();
        }
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetMouseButtonDown(0))
        {

        }
    }

    public void Update()
    {
        //if (Input.GetMouseButtonDown(0))
        //{
        //    dragOrigin = Input.mousePosition;
        //    return;
        //}

        //if (!Input.GetMouseButton(0)) return;

        //Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - dragOrigin);
        //Vector3 move = new Vector3(pos.x * dragSpeed, 0, pos.y * dragSpeed);

        //transform.Translate(move, Space.World);



        ////Transforming Selection Area
        //if (Input.GetMouseButton(0))
        //{
        //    Vector3 currentMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //    Vector3 lowerLeft = new Vector3(
        //        Mathf.Min(mousePosition.x, currentMousePosition.x),
        //        Mathf.Min(mousePosition.y, currentMousePosition.y)
        //        );
        //    Vector3 upperRight = new Vector3(
        //        Mathf.Max(mousePosition.x, currentMousePosition.x),
        //        Mathf.Max(mousePosition.y, currentMousePosition.y)
        //        );
        //    selectionAreaTransform.position = lowerLeft;
        //    selectionAreaTransform.localScale = upperRight - lowerLeft;
        //}

        //if (Input.GetMouseButtonDown(0))
        //{
        //    mousePosition = UnitControlClass.GetMouseWorldPosition();

        //    selectionAreaTransform.gameObject.SetActive(true);
        //}
        ////Creating the selection process
        //if (Input.GetMouseButtonUp(0))
        //{
        //    Collider2D[] collider2DArray = Physics2D.OverlapAreaAll(mousePosition, UnitControlClass.GetMouseWorldPosition());
        //    foreach (ControllableUnits control in selectedControllableUnits)
        //    {
        //        //if(ControlUnitSelected != null)

        //        control.animPeasant.Play("Peasant_Idle");
        //        control.isSelected = false;


        //    }
        //    selectedControllableUnits.Clear();
        //    foreach (Collider2D collider2D in collider2DArray)
        //    {
        //        ControllableUnits controllableUnit = collider2D.GetComponent<ControllableUnits>();
        //        if (controllableUnit != null)
        //        {
        //            controllableUnit.animPeasant.Play("Peasant_Idle_Selected");
        //            controllableUnit.isSelected = true;
        //            selectedControllableUnits.Add(controllableUnit);
        //        }
        //    }
        //    selectionAreaTransform.gameObject.SetActive(false);
    }
}