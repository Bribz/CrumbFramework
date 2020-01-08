using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CrumbShared;
using CrumbShared.Objects;

namespace CrumbClient
{
    public class ClientPlayerManager : PlayerManager
    {
        protected CClient m_NetClient;
        public ClientPlayerManager(CClient client) : base()
        {
            m_NetClient = client;
        }

        public override void tick(float delta)
        {
            base.tick(delta);
        }

        public PlayerAvatar add(CharacterData data, Vector3f position, float rotation = 0, bool isLocal = false)
        {
            PlayerAvatar avatar;

            if (!isLocal)
            {
                avatar = new PlayerAvatar(data) { Position = position, Rotation = rotation };
            }
            else
            {
                avatar = new LocalPlayerAvatar(m_NetClient) { Position = position, Rotation = rotation };
            }

            Players.Add(avatar);
            return avatar;
        }

        //Hidden inherited add method
        new private PlayerAvatar add(Vector3f position, float rotation = 0) { return null; }

        public override void remove(PlayerAvatar avatar)
        {
            base.remove(avatar);
        }

        public override void clear()
        {
            base.clear();
        }

        
    }
}
