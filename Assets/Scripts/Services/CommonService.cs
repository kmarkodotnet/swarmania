using System;
using System.Linq;
using UnityEngine;

public class CommonService
{
    public CursorMode cursorMode = CursorMode.Auto;
    public Vector2 hotSpot = Vector2.zero;
    private static CursorStateEnum _cursorState = CursorStateEnum.Default;
    public CursorStateEnum GetCursorState()
    {
        return _cursorState;
    }
    public void SetCursorState(CursorStateEnum cursorState, Texture2D cursorTexture)
    {
        _cursorState = cursorState;
        Cursor.SetCursor(cursorTexture, hotSpot, cursorMode);
    }

    public GameObject FindNearestEnemy(GameObject me)
    {
        var lookDistance = me.GetComponent<LivingObject>().GetLookDistance();
        var nearUnits = Physics2D.OverlapCircleAll(new Vector2(me.gameObject.transform.position.x, me.gameObject.transform.position.y), lookDistance)
            .Where(u => u.tag == "bug" || u.tag == "castle");
        GameObject nearestEnemy = null;
        float nearestDistance = 0;

        foreach (var unit in nearUnits.Where(nu => new CommonService().IsAlive(nu.gameObject)))
        {
            bool isEnemy = AreEnemies(me, unit);
            if (isEnemy)
            {
                var myCollider = me.GetComponent<CapsuleCollider2D>();
                var otherCollider = unit;
                var currentDistance = Physics2D.Distance(myCollider, otherCollider).distance;
                if (nearestEnemy == null || nearestDistance > currentDistance)
                {
                    nearestEnemy = unit.gameObject;
                    nearestDistance = currentDistance;
                }
            }
        }
        return nearestEnemy;
    }

    internal void MakeDead(GameObject gameObject)
    {
        if(gameObject != null && gameObject.GetComponent<BugActionSet>() != null)
            gameObject.GetComponent<BugActionSet>().Dead();
    }

    public bool AreEnemies(GameObject me, Collider2D unit)
    {
        return AreEnemies(me, unit.gameObject);
    }
    public bool AreEnemies(GameObject me, GameObject otherGameObject)
    {
        var myOwnerId = GetOwnerId(me);
        var otherGameObjectsOwnerId = GetOwnerId(otherGameObject);
        return myOwnerId.ToUpper() != otherGameObjectsOwnerId.ToUpper();
    }

    public Transform GetChildrenByName(Transform transform, string name)
    {
        Transform searchedChild = null;
        for (int i = 0; i < transform.childCount; ++i)
        {
            var child = transform.GetChild(i);
            if (child.name == name)
            {
                searchedChild = child;
            }
        }
        return searchedChild;
    }

    public string GetOwnerId(GameObject go)
    {
        if (go.GetComponent<SelectableBug>())
            return go.GetComponent<SelectableBug>().GetOwnerId();
        else if (go.GetComponent<SelectableCastle>())
            return go.GetComponent<SelectableCastle>().GetOwnerId();
        else if (go.GetComponent<SelectableObject>())
            return go.GetComponent<SelectableObject>().GetOwnerId();
        else
            throw new NotImplementedException("No owner provider for object");
    }

    internal GameObject FindNearestResource(GameObject me)
    {
        var lookDistance = me.GetComponent<LivingObject>().GetLookDistance();
        var nearResources = Physics2D.OverlapCircleAll(new Vector2(me.gameObject.transform.position.x, me.gameObject.transform.position.y), lookDistance)
            .Where(u => u.tag == "resource");
        GameObject nearestResource = null;
        float nearestDistance = 0;

        nearResources = nearResources.Where(nr => IsAnyResource(nr.gameObject));

        foreach (var unit in nearResources.Where(r => me.GetComponent<ResourceHandler>().GetPreferedResourceType() == r.GetComponent<Resource>().GetResourceType()))
        {
            var myCollider = me.GetComponent<CapsuleCollider2D>();
            var otherCollider = unit;
            var currentDistance = Physics2D.Distance(myCollider, otherCollider).distance;
            if (nearestResource == null || nearestDistance > currentDistance)
            {
                nearestResource = unit.gameObject;
                nearestDistance = currentDistance;
            }
        }
        if (nearestResource != null)
        {
            return nearestResource;
        }

        foreach (var unit in nearResources.Where(r => me.GetComponent<ResourceHandler>().GetPreferedResourceType() != r.GetComponent<Resource>().GetResourceType()))
        {
            var myCollider = me.GetComponent<CapsuleCollider2D>();
            var otherCollider = unit;
            var currentDistance = Physics2D.Distance(myCollider, otherCollider).distance;
            if (nearestResource == null || nearestDistance > currentDistance)
            {
                nearestResource = unit.gameObject;
                nearestDistance = currentDistance;
            }
        }
        return nearestResource;
    }

    internal Sprite GetSprite(Transform transform)
    {
        if (transform.GetComponent<Bug>())
            return transform.GetComponent<SpriteRenderer>().sprite;
        else if (transform.GetComponent<Castle>())
            return transform.GetComponent<SpriteRenderer>().sprite;
        else
            throw new Exception("Different kind of object");
        
    }

    internal bool IsAlive(GameObject target)
    {
        return (target.GetComponent<BugStateHandler>() != null && target.GetComponent<BugStateHandler>().GetState() != BugStateEnum.Dead)
            ||
            (target.GetComponent<CastleStateHandler>() != null && target.GetComponent<CastleStateHandler>().GetState() != CastleStateEnum.Destroyed);
    }
    internal bool IsAnyResource(GameObject targetResource)
    {
        return (targetResource.GetComponent<Resource>() != null && targetResource.GetComponent<Resource>().GetTotalResourceAmmount() > 0);
    }
}
