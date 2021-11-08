using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.GroupControls
{
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
