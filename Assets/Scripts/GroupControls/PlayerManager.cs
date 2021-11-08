using Assets.Scripts.GroupControls;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    private string myId;
    private Players players;
    private void Awake()
    {
        var c = FindObjectsOfType<PlayerManager>().Length;
        if (c > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Start()
    {
        myId = "0";// System.Guid.NewGuid();
        InitPlayers();
        //Time.timeScale = 500;
        //void FixedUpdate
    }

    private void InitPlayers()
    {
        players = new Players();
        var ps = FindObjectsOfType<Player>();
        ps.ToList().ForEach(p => players.Add(p));
        //players.Add(new Player { Id = myId, Name = "me" });
        //players.Add(new Player
        //{
        //    Id =
        //    "1"//System.Guid.NewGuid()
        //    ,
        //    Name = "Enemy1"
        //});

    }

    public List<string> GetEnemyIds(string playerId)
    {
        return players.GetPlayers().Where(p => p.GetId().ToLower() != playerId.ToLower()).Select(p => p.GetId()).ToList();
    }

    public bool IsExistingPlayer(string id)
    {
        return players.IsExists(id);
    }

    public bool IsPlayerId(string id)
    {
        return myId.ToString().ToUpper() == id.ToUpper();
    }

}
