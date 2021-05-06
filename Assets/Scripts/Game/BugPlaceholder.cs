using UnityEngine;
using UnityEngine.EventSystems;

public class BugPlaceholder : MonoBehaviour, IPointerUpHandler, IPointerDownHandler//, IPointerClickHandler
{
    [SerializeField] int? id;

    public int? GetId()
    {
        return id;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log(gameObject.name + ": I was clicked!");
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log(gameObject.name + ": I was clicked!");
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log(gameObject.name + ": I was clicked!");
    }

    public void SetId(int? id)
    {
        this.id = id;
    }
    GameObject lastselect;
    //void Update()
    //{
    //    if (EventSystem.current.currentSelectedGameObject == null)
    //    {
    //        Debug.Log("Set");
    //        EventSystem.current.SetSelectedGameObject(lastselect);
    //        Debug.Log("Setttt");
    //    }
    //    else
    //    {
    //        Debug.Log("noset");
    //        lastselect = EventSystem.current.currentSelectedGameObject;
    //        Debug.Log("nosetttt");
    //    }
    //}

    private void OnMouseOver()
    {
        var cs = new CommonService();
        var currentState = cs.GetCursorState();
        if (currentState != CursorStateEnum.IconSelection)
        {
            cs.SetCursorState(CursorStateEnum.IconSelection, FindObjectOfType<Config>().GetIconSelectionCursorTexture());
        }
        if (Input.GetMouseButtonDown(0))
        {
            Control.ControlClickLocked = true;
        }
    }
}
