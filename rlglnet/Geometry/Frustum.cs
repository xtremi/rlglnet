using GlmNet;
using System.Collections.Generic;

namespace rlglnet.Geometry
{
    public class Frustum
    {

        Plane[] Planes = new Plane[6];
        public Plane Near
        {
            get { return Planes[0]; }
            set { Planes[0] = value; }
        }
        public Plane Far
        {
            get { return Planes[1]; }
            set { Planes[1] = value; }
        }
        public Plane Top
        {
            get { return Planes[2]; }
            set { Planes[2] = value; }
        }
        public Plane Bottom
        {
            get { return Planes[3]; }
            set { Planes[3] = value; }
        }
        public Plane Left
        {
            get { return Planes[4]; }
            set { Planes[4] = value; }
        }
        public Plane Right
        {
            get { return Planes[5]; }
            set { Planes[5] = value; }
        }

        public bool IsInFrustum(List<vec3> coordinates, bool strict)
        {
			const int NUMBER_OF_PLANES = 6;

            List<int> plane_outside_of = new List<int>(coordinates.Count); //The index of the frustum plane of which the the coordinates was outside of (? I think ?)

            //int cornerIndex = 0;
            foreach (vec3 coord in coordinates) {

				bool cornerInFrustum = true;
				for (int i = 0; i < NUMBER_OF_PLANES; i++)
				{
					Plane plane = Planes[i];
					vec3 vec = coord - plane.Position;
					float d = glm.dot(vec, plane.Normal);
					if (d < 0.0f)
					{
						cornerInFrustum = false;
						plane_outside_of.Add(i);	//[cornerIndex++] = i;
						break;
					}
				}

				if (cornerInFrustum)
				{
					return true;
				}
			}

			//If both corners are outside frustum, but not
			//based on the same planes, we keep it.
			//This is for large object, larger than the frusum space
			if (strict)
            {
                for (int i = 1; i < plane_outside_of.Count; i++) {
                	if (plane_outside_of[0] != plane_outside_of[i]) {
                		return true;
                	}
                }
                //if (plane_outside_of[0] != plane_outside_of[1])
                //{
                //	return true;
                //}
            }
            return false;
		}

    };
}