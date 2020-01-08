using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrumbShared.Objects
{
    public class EntityFactory
    {
        public EntityFactory()
        {

        }

        public virtual Entity spawn(Vector3f position, float rotation = 0f)
        {
            return new Entity() { Position=position, Rotation = rotation};
        }
    }
}
