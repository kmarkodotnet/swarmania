using System.Linq;
using UnityEngine;

public class GameArea : MonoBehaviour
{
    public float dragSpeed = 2;

    private void OnMouseOver()
    {
        if (!ControlContext.IsContextActive())
        {
            var state = new CommonService().GetCursorState();
            var anySelectedObjects = FindObjectOfType<SelectionControl>().GetSelectedObjects().Any();
            if (state != CursorStateEnum.Move && anySelectedObjects)
            {
                new CommonService().SetCursorState(CursorStateEnum.Move, FindObjectOfType<Config>().GetMoveCursorTexture());
            }
            else if (state != CursorStateEnum.Default && !anySelectedObjects)
            {
                new CommonService().SetCursorState(CursorStateEnum.Move, Config.defaultSelectionCursorTextureStatic);
            }
        }
        
        if (Input.GetMouseButtonDown(1))
        {
            Debug.Log("move");
            FindObjectOfType<SelectionControl>().Move();
            ControlContext.FinishContext();
        }
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetMouseButtonDown(0))
        {

        }
    }
}