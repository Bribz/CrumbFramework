using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CrumbShared;
using CrumbShared.Objects;

namespace CrumbClient
{
    public class LocalPlayerAvatar : PlayerAvatar
    {
        private CClient m_NetClient;
        private Vector2f InputDirection;

        public LocalPlayerAvatar(CClient client) : base()
        {
            m_NetClient = client;
            InputDirection = new Vector2f();
        }

        private void poll_input()
        {
            if(InputDirection.X < -.5f)
            {
                m_NetClient.CachedInputUpdate.Keys |= InputKeys.Left;
            }
            if (InputDirection.X > .5f)
            {
                m_NetClient.CachedInputUpdate.Keys |= InputKeys.Right;
            }
            if (InputDirection.Y < -.5f)
            {
                m_NetClient.CachedInputUpdate.Keys |= InputKeys.Down;
            }
            if (InputDirection.Y > .5f)
            {
                m_NetClient.CachedInputUpdate.Keys |= InputKeys.Up;
            }
        }

        public override void update_input(InputKeys keys)
        {
            base.update_input(keys);
        }

        public override void tick(float delta)
        {
            poll_input();
            base.tick(delta);
        }
    }
}
