using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrumbShared.Objects
{
    public class PlayerAvatarFactory : EntityFactory
    {
        public PlayerAvatarFactory() : base()
        {

        }

        public PlayerAvatar spawn(Vector3f position, float rotation = 0f)
        {
            return new PlayerAvatar() { Position = position, Rotation = rotation };
        }
    }
}
