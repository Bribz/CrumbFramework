using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrumbShared.Objects
{
    public class PlayerManager
    {
        public List<PlayerAvatar> Players;
        protected PlayerAvatarFactory m_factory;

        public PlayerManager()
        {
            Players = new List<PlayerAvatar>();
            m_factory = new PlayerAvatarFactory();
        }

        public virtual void tick(float delta)
        {
            foreach(var player in Players)
            {
                player.tick(delta);
            }
        }

        public virtual void clear()
        {
            Players.Clear();
        }

        public virtual void remove(PlayerAvatar avatar)
        {
            Players.Remove(avatar);
        }

        public virtual PlayerAvatar add(Vector3f position, float rotation = 0f)
        {
            var player = m_factory.spawn(position, rotation);
            Players.Add(player);
            return player;
        }
    }
}
