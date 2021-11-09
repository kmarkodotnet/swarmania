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
        myId = "0";
        InitPlayers();
    }

    private void InitPlayers()
    {
        players = new Players();
        var ps = FindObjectsOfType<Player>();
        ps.ToList().ForEach(p => players.Add(p));
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
            return players.Any(p => p.GetId().ToString().ToUpper() == playerId.ToUpper());
        }

        public List<Player> GetPlayers()
        {
            return players;
        }
    }
}
