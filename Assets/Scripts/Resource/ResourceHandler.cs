using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceHandler : MonoBehaviour
{
    [SerializeField] ResourceTypeEnum preferedResourceType;
    [SerializeField] ResourceTypeEnum resourceType;
    [SerializeField] float maxResourceCarryAmmount = 1;
    [SerializeField] float resourceCarryAmmount = 0;
    System.DateTime lastChecked;
    [SerializeField] float periodicity = 1f;

    public ResourceTypeEnum GetPreferedResourceType()
    {
        return preferedResourceType;
    }

    public float GetMaxResourceCarryAmmount()
    {
        return maxResourceCarryAmmount;
    }

    public void SetResourceCarryAmmount(ResourceTypeEnum resourceType, float resourceCarryAmmount)
    {
        this.resourceType = resourceType;
        if(maxResourceCarryAmmount <= resourceCarryAmmount)
        {
            this.resourceCarryAmmount = maxResourceCarryAmmount;
        }
        else
        {
            this.resourceCarryAmmount = resourceCarryAmmount;
        }
    }
    bool x = false;
    internal bool HarvestTarget(GameObject targetToHarvest)
    {
        if ((System.DateTime.Now - lastChecked).TotalSeconds > ((periodicity * FindObjectOfType<Config>().GetHarvestkPeriodicyScale()) / Constants.TimeScale) && targetToHarvest)
        {
            lastChecked = System.DateTime.Now;
            var resourceToCarry = targetToHarvest.GetComponent<Resource>().GetResourceAmmount(1);
            if (resourceToCarry <= 0)
            {
                //TODO: BugStateHandler.targetToHarvest törlése
                //TODO: vagy jobb lenne ha új resource-t keresne?
                // esetleg ilyenkor inkább döntést kellene kérnie? pl harvest v attack v scout esetleg back to castle?
                GetComponent<BugActionSet>().Idle();
            }
            else
            {
                if (x)
                {
                    GameObject birthCastle = GetComponent<Bug>().GetBirthCastle();
                    GetComponent<BugStateHandler>().SetTargetCastle(birthCastle);
                    GetComponent<BugActionSet>().MoveToCastle(birthCastle);
                    x = false;
                }
                else
                {
                    var resourceType = targetToHarvest.GetComponent<Resource>().GetResourceType();
                    SetResourceCarryAmmount(resourceType, resourceToCarry);
                    x = true;
                }                
                
            }
            //TODO: allocate time to harvest                
        }
        return true;
    }

    internal ResourceTypeEnum GetCarriedResourceType()
    {
        return this.resourceType;
    }

    internal float EmptyCarriedResourceAmmount()
    {
        var result = resourceCarryAmmount;
        resourceCarryAmmount = 0;
        return result;
    }
}
