using UnityEngine;
using UnityEngine.EventSystems;

public class BugPlaceholder : MonoBehaviour
{
    [SerializeField] int? id;

    public int? GetId()
    {
        return id;
    }

    public void SetId(int? id)
    {
        this.id = id;
    }
    GameObject lastselect;

}
