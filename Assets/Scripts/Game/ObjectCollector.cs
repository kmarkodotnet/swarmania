using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObjectCollector : MonoBehaviour
{
    private void Awake()
    {
        var c = FindObjectsOfType<ObjectCollector>().Length;
        if (c > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    private Dictionary<string, Dictionary<int, GameObject>> _bugs = new Dictionary<string, Dictionary<int, GameObject>>();

    public void AddBug(string ownerId, int bugId, GameObject bug)
    {
        if (!_bugs.ContainsKey(ownerId))
        {
            _bugs.Add(ownerId, new Dictionary<int, GameObject>());
        }
        _bugs[ownerId].Add(bugId, bug);
    }

    public Dictionary<string, Dictionary<int, GameObject>> Bugs()
    {
        return _bugs;
    }

    public Dictionary<int, GameObject> Bugs(string ownerId)
    {
        return _bugs[ownerId];
    }

    public GameObject Bug(string ownerId, int bugId)
    {
        return _bugs[ownerId][bugId];
    }


    private Dictionary<string, Dictionary<int, GameObject>> _castles = new Dictionary<string, Dictionary<int, GameObject>>();

    public void AddCastle(string ownerId, int castleId, GameObject castle)
    {
        if (!_castles.ContainsKey(ownerId))
        {
            _castles.Add(ownerId, new Dictionary<int, GameObject>());
        }
        _castles[ownerId].Add(castleId, castle);
    }

    public Dictionary<string, Dictionary<int, GameObject>> Castles()
    {
        return _castles;
    }

    public Dictionary<int, GameObject> Castles(string ownerId)
    {
        return _castles[ownerId];
    }

    public GameObject Castle(string ownerId, int castleId)
    {
        return _castles[ownerId][castleId];
    }

    public Power GetPower(string ownerId)
    {
        var bugTypes = _bugs[ownerId].Values.Where(b => b.GetComponent<BugStateHandler>().GetState() != BugStateEnum.Dead).Select(b => b.GetComponent<AttackableBug>().GetBugType());
        var bugLevels = bugTypes.Select(b => new DictionaryService().GetBugLevel(b));

        var p = new Power();

        p.CastleNum = _castles[ownerId].Count;
        p.TotalResource = _castles[ownerId].Values.Sum(c => c.GetComponent<CastleStateHandler>().GetResourcesSum());
        p.Level1Bug = bugLevels.Count(l => l == 1);
        p.Level2Bug = bugLevels.Count(l => l == 2);
        p.Level3Bug = bugLevels.Count(l => l == 3);

        return p;
    }
}
