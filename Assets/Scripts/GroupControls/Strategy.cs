using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum StrategyEnum
{
    CollectFood,
    Scout,
    Offensive,
    Defensive,
}

public class Power
{
    public int CastleNum { get; set; }
    public int Level1Bug { get; set; }
    public int Level2Bug { get; set; }
    public int Level3Bug { get; set; }
    public int TotalResource { get; set; }
}

public class ResourceLocation
{
    public float X { get; set; }
    public float Y { get; set; }
}

public class ResourceMap
{
    Dictionary<ResourceTypeEnum, List<ResourceLocation>> _resources;

    public ResourceMap()
    {
        _resources = new Dictionary<ResourceTypeEnum, List<ResourceLocation>>();
        var values = Enum.GetValues(typeof(ResourceTypeEnum));
        foreach (ResourceTypeEnum enumValue in values)
        {
            _resources.Add(enumValue, new List<ResourceLocation>());
        }
    }

    private bool KnownResource(ResourceTypeEnum resource, ResourceLocation location)
    {
        var epsilon = Mathf.Epsilon;
        var resources = _resources[resource];
        return resources.Any(r => Equals(location, r, epsilon));
    }

    private static bool Equals(ResourceLocation location, ResourceLocation r, float epsilon)
    {
        return Mathf.Abs(r.X - location.X) < epsilon && Mathf.Abs(r.Y - location.Y) < epsilon;
    }

    public void Add(ResourceTypeEnum resource, ResourceLocation location)
    {
        if(!KnownResource(resource, location))
            _resources[resource].Add(location);
    }

    public void Remove(ResourceTypeEnum resource, ResourceLocation location)
    {
        if (KnownResource(resource, location))
            _resources[resource] = _resources[resource].Where(r => !Equals(r, location)).ToList();
    }
}
