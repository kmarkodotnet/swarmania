using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resource : MonoBehaviour
{
    [SerializeField] ResourceTypeEnum resourceType;
    [SerializeField] float resourceAmmount = 1;

    public ResourceTypeEnum GetResourceType()
    {
        return resourceType;
    }

    public float GetTotalResourceAmmount()
    {
        return resourceAmmount;
    }

    public float GetResourceAmmount(float resourceToCarry)
    {
        if(resourceToCarry >= resourceAmmount)
        {
            var carriableResource = resourceAmmount;
            resourceAmmount = 0;

            var c = GetComponentInChildren<SpriteRenderer>().color;
            GetComponentInChildren<SpriteRenderer>().color = new Color(c.r, c.g, c.b, 0);

            return carriableResource;
        }
        else
        {
            resourceAmmount -= resourceToCarry;
            return resourceToCarry;
        }
    }
}
