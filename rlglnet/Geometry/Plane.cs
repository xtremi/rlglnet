using GlmNet;

namespace rlglnet.Geometry
{
    public class Plane
    {
        public Plane(vec3 normal, vec3 position)
        {
            Normal = normal;
            Position = position;
        }

        private vec3 _normal;
        public vec3 Normal { 
            get { return _normal; }
            set { _normal = glm.normalize(value); } 
        }
        public vec3 Position { get; private set; }

    };
}