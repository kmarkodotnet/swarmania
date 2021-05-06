using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Unity.Barracuda;
using UnityEngine;

public class EntityCollection : MonoBehaviour
{
    [SerializeField] int calculateHordesFrame = 3;
    [SerializeField] int maxHordesNumber = 24;
    [SerializeField] Horde hordePrefab;
    [SerializeField] List<NNModel> models;

    private ConcurrentDictionary<int,GameObject> _bugs;
    private Dictionary<string, List<HordePoolObject>> _hordePool;
    private List<Horde> _hordes;

    private void Awake()
    {
        var c = FindObjectsOfType<EntityCollection>().Length;
        if (c > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
        _bugs = new ConcurrentDictionary<int, GameObject>();
        _hordes = new List<Horde>();
        _hordePool = new Dictionary<string, List<HordePoolObject>>();
        
        _hordePool = CreateHordePool(10);

    }

    private void Start()
    {
        //var breeder = FindObjectOfType<AiBreeder>();
        //maxHordesNumber = breeder.GetPlayersNum() * breeder.GetMaxN() * breeder.GetMaxM()*2;
        
        //breeder.Breed();
    }

    private Dictionary<string, List<HordePoolObject>> CreateHordePool(int v)
    {
        var hpd = new Dictionary<string, List<HordePoolObject>>();
        var teams = new List<string> { "0", "1" };
        foreach (var team in teams)
        {
            var hp = new List<HordePoolObject>();
            for (int i = 0; i < v; i++)
            {
                var h = Instantiate(hordePrefab);
                hp.Add(new HordePoolObject
                {
                    Horde = h,
                    State = HordePoolObject.HordePoolObjectState.ReadyToUse
                });
            }
            hpd.Add(team, hp);
        }
        
        return hpd;
    }

    internal void AwardHordePool()
    {
        foreach (var hp in _hordePool)
        {
            for (int i = 0; i < _hordePool[hp.Key].Count; i++)
            {
                var horde = _hordePool[hp.Key][i].Horde;
                horde.AwardBeforeEndEpisode();
            }
        }
    }

    public void ResetHordePool()
    {
        foreach (var hp in _hordePool)
        {
            for (int i = 0; i < _hordePool[hp.Key].Count; i++)
            {
                _hordePool[hp.Key][i].State = HordePoolObject.HordePoolObjectState.ReadyToUse;
            }
        }
    }

    public void EndHordePoolEpisode()
    {
        foreach (var hp in _hordePool)
        {
            for (int i = 0; i < _hordePool[hp.Key].Count; i++)
            {
                _hordePool[hp.Key][i].Horde.EndEpisode();
            }
        }
    }

    private Horde GetHorde(string teamId)
    {
        Debug.Log(teamId);
        var readyToUseHorde = _hordePool[teamId].FirstOrDefault(h => h.State == HordePoolObject.HordePoolObjectState.ReadyToUse);
        if(readyToUseHorde == null)
        {
            var x = string.Join(", ",_hordePool.Select(h => h.Key));
        }
        readyToUseHorde.State = HordePoolObject.HordePoolObjectState.Used;
        return readyToUseHorde.Horde;
    }

    public void AddBug(GameObject bug)
    {
        if (_bugs == null)
        {
            _bugs = new ConcurrentDictionary<int, GameObject>();
        }
        if (!_bugs.TryAdd(bug.GetComponent<Bug>().GetId(), bug))
        {
            //Debug.LogError($"{bug.GetComponent<Bug>().GetId()} - failed to add to entitycollection");
        }
    }

    public int GetTeamWeight(GameObject bug)
    {
        var myTeamId = bug.GetComponent<SelectableObject>().GetOwnerId();
        var teamMates = _bugs.Count(b => b.Value.GetComponent<SelectableObject>().GetOwnerId() == myTeamId);
        return teamMates;
    }

    public void RemoveBug(GameObject bug)
    {
        var id = bug.GetComponent<Bug>().GetId();
        var ownerId = bug.GetComponent<SelectableObject>().GetOwnerId();
        var containerHorde = _hordes.FirstOrDefault(r => r.ContainsBug(bug));
        if (containerHorde != null)
        {
            containerHorde.Remove(bug);
        }
        GameObject b;
        if (_bugs.ContainsKey(id) && (!_bugs.TryRemove(id, out b) || b == null))
        {

        }
        foreach (var attackerBug in _bugs)
        {
            attackerBug.Value.GetComponent<BugStateHandler>().SetNoAttackTarget(bug);
        }
        Destroy(bug);
    }

    public void ClearBugs()
    {
        var c = _bugs.Count;
        var keys = _bugs.Keys.ToArray();
        for (int i = 0; i < c; i++)
        {
            if(_bugs.ContainsKey(keys[i]))
                RemoveBug(_bugs[keys[i]]);
        }
        _bugs = new ConcurrentDictionary<int, GameObject>();
    }
    internal void ClearHordes()
    {
        _hordes = new List<Horde>();
    }

    public Horde GetMyHorde(GameObject bug)
    {
        var containerHorde = _hordes.FirstOrDefault(r => r.ContainsBug(bug));
        if (containerHorde == null)
        {
            var teamId = bug.GetComponent<SelectableObject>().GetOwnerId();
            
            var horde = GetHorde(teamId);
            horde.Lookup(bug, teamId, true);

            _hordes.Add(horde);
            return horde;
        }
        return containerHorde;
    }

    public void CalculateHordes()
    {
        var hordes  = new List<Horde>();

        foreach (var bug in _bugs)
        {
            if (!hordes.Any(r => r.ContainsBug(bug.Value)))
            {
                var teamId = bug.Value.GetComponent<SelectableObject>().GetOwnerId();
                var horde = GetHorde(teamId);
                horde.Lookup(bug.Value, teamId, true);

                hordes.Add(horde);                
            }
        }
        _hordes.Clear();
        _hordes = hordes;
    }
}
