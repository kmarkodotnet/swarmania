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

    private void OnMouseOver()
    {
        if (Context.IsHarvest())
        {
            Context.ActivateHarvest();
            if (Input.GetMouseButtonDown(0))
            {
                FindObjectOfType<SelectionControl>().Harvest(gameObject);
                Context.FinishContext();
            }
        }else
        {
            if (!Context.IsContextActive())
            {
                var cs = new CommonService();
                cs.SetCursorState(CursorStateEnum.Harvest, FindObjectOfType<Config>().GetHarvestCursorTexture());
            }
            if (Input.GetMouseButtonDown(1))
            {
                FindObjectOfType<SelectionControl>().Harvest(gameObject);
                Context.FinishContext();
            }
        }
    }

    private void OnMouseExit()
    {
        if (Context.IsHarvest())
        {
            Context.InactivateHarvest();
        }
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
