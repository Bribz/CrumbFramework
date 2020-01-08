using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CrumbShared;
using CrumbShared.Objects;

namespace CrumbServer.GameServer
{
    internal class MapManager
    {
        private PlayerManager m_PlayerManager;
        private List<Entity> m_Monsters;
        private List<Entity> m_NPCs;

        public MapManager()
        {
            m_PlayerManager = new PlayerManager();
            m_Monsters = new List<Entity>();
            m_NPCs = new List<Entity>();
        }

        public void tick(float delta)
        {
            m_PlayerManager.tick(delta);
        }
        
        public PlayerAvatar player_join(GameConnection connection, Vector3f position, float rotation = 0f)
        {
            return m_PlayerManager.add(position, rotation);
        }

        public void player_left(GameConnection connection)
        {
            var avatar = m_PlayerManager.Players.Find(p => p.Data.CharacterID == connection.CharacterID);
            m_PlayerManager.remove(avatar);
        }
    }
}
