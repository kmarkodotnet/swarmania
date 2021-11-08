using UnityEngine;

public class Resource : MonoBehaviour
{
    [SerializeField] ResourceTypeEnum resourceType;
    [SerializeField] float resourceAmmount = 1;

    private void Start()
    {
        FindObjectOfType<ResourceMap>().Add(resourceType, new ResourceLocation {  X = gameObject.transform.position.x, Y = gameObject.transform.position.y});
    }

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
        if (ControlContext.IsHarvest())
        {
            ControlContext.ActivateHarvest();
            if (Input.GetMouseButtonDown(0))
            {
                FindObjectOfType<SelectionControl>().Harvest(gameObject);
                ControlContext.FinishContext();
            }
        }else
        {
            if (!ControlContext.IsContextActive())
            {
                var cs = new CommonService();
                cs.SetCursorState(CursorStateEnum.Harvest, FindObjectOfType<Config>().GetHarvestCursorTexture());
            }
            if (Input.GetMouseButtonDown(1))
            {
                FindObjectOfType<SelectionControl>().Harvest(gameObject);
                ControlContext.FinishContext();
            }
        }
    }

    private void OnMouseExit()
    {
        if (ControlContext.IsHarvest())
        {
            ControlContext.InactivateHarvest();
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
