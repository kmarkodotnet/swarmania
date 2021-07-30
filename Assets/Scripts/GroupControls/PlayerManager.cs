using System.Collections;
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
        players.Add(new Player { Id = myId, Name = "me" });
        players.Add(new Player
        {
            Id =
            "1"//System.Guid.NewGuid()
            ,
            Name = "Enemy1"
        });

    }

    public List<string> GetEnemyIds(string playerId)
    {
        return players.GetPlayers().Where(p => p.Id.ToLower() != playerId.ToLower()).Select(p => p.Id).ToList();
    }

    public bool IsExistingPlayer(string id)
    {
        return players.IsExists(id);
    }

    public void Add(string id)
    {
        players.Add(new Player { Id = id, Name = id });
    }

    public bool IsPlayerId(string id)
    {
        return myId.ToString().ToUpper() == id.ToUpper();
    }

    public class Players
    {
        private List<Player> players;
        public Players()
        {
            players = new List<Player>();
        }
        public void Add(Player player)
        {
            players.Add(player);
        }

        public bool IsExists(string playerId)
        {
            return players.Any(p => p.Id.ToString().ToUpper() == playerId.ToUpper());
        }

        public List<Player> GetPlayers()
        {
            return players;
        }
    }
}
