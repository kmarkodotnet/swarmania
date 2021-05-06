using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Castle : MonoBehaviour
{

    [SerializeField] int castleId;

    private static int _nextId;
    private static int NextId
    {
        get
        {
            var nextId = _nextId;
            _nextId++;
            return nextId;
        }
    }
    private void Awake()
    {
        castleId = NextId;
    }

    public int GetId()
    {
        return castleId;
    }
}
