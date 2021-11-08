using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SelectionControl : MonoBehaviour
{
    List<GameObject> SelectedObjects;
    GameObject AttackedObject;

    private void Awake()
    {
        var c = FindObjectsOfType<SelectionControl>().Length;
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
        SelectedObjects = new List<GameObject>();
    }

    public bool Attack(GameObject attackedObject)
    {
        try
        {
            if (SelectedObjects.Any())
            {
                AttackedObject = attackedObject;
                SelectedObjects
                    .Where(o => o.GetComponent<BugActionSet>() != null).ToList()
                    .ForEach(o => o.GetComponent<BugActionSet>().MoveToAttack(attackedObject));
                return true;
            }
            return false;
        }
        catch (System.Exception)
        {
            return false;
        }        
    }
    public void Move()
    {
        AttackedObject = null;
        if (SelectedObjects.Any())
        {   
            var target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var owned = SelectedObjects.Where(o => o.GetComponent<BugActionSet>() != null && IsBugOwnedByPlayer(o));

            owned.ToList()
                .ForEach(o => o.GetComponent<BugActionSet>().Move(target));
        }
    }

    public void Harvest(GameObject target)
    {
        if (SelectedObjects.Any())
        {
            var owned = SelectedObjects.Where(o => o.GetComponent<BugActionSet>() != null && IsBugOwnedByPlayer(o));

            owned.ToList()
                .ForEach(o => o.GetComponent<BugActionSet>().MoveToHarvest(target));
        }
    }
    private bool IsBugOwnedByPlayer(GameObject bug)
    { 
        var isOwned = bug.GetComponent<SelectableBug>().IsOwnedByPlayer();
        return isOwned;
    }

    public GameObject GetAttackedObject()
    {
        return AttackedObject;
    }

    public void AddSelectedObject(GameObject selectableObject)
    {
        SelectedObjects.Add(selectableObject);
    }

    public List<GameObject> GetSelectedObjects()
    {
        return SelectedObjects;
    }

    public void RemoveSelectableObject(GameObject selectableObject)
    {
        SelectedObjects.Remove(selectableObject);
    }

}
