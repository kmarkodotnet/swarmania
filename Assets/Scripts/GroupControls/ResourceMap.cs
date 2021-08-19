using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ResourceMap : MonoBehaviour
{
    Dictionary<ResourceTypeEnum, List<ResourceLocation>> _resources;

    private void Awake()
    {
        var c = FindObjectsOfType<ResourceMap>().Length;
        if (c > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
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
        if(_resources == null)
        {
            _resources = new Dictionary<ResourceTypeEnum, List<ResourceLocation>>();
        }
        if (!_resources.ContainsKey(resource))
        {
            _resources.Add(resource, new List<ResourceLocation>());
            _resources[resource].Add(location);
        }
        else if(!KnownResource(resource, location))
            _resources[resource].Add(location);
    }

    public void Remove(ResourceTypeEnum resource, ResourceLocation location)
    {
        if (KnownResource(resource, location))
            _resources[resource] = _resources[resource].Where(r => !Equals(r, location)).ToList();
    }
}
