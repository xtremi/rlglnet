using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace rlglnet
{

    public class rlglQuadTreeElement { 
        public GlmNet.vec3 Center { get; private set; }
        public int Level { get; private set; }
        rlglQuadTree _quadTree;

        public rlglQuadTreeElement(rlglQuadTree quadTree, GlmNet.vec3 center, int level)
        {
            _quadTree = quadTree;
            Center = center;
            Level = level;
        }
        public float Size()
        {
            return _quadTree._totalSize / MathF.Pow(2.0f, Level);
        }
        protected rlglQuadTreeElement[] children = null;


        public bool IsSplit(){
            return children != null; 
        }

        public void SplitIfNear(GlmNet.vec3 pos, ref List<rlglQuadTreeElement> quads)
        {
            
            float distance = MathF.Sqrt(MathF.Pow(Center.x - pos.x, 2.0f) + MathF.Pow(Center.y - pos.y, 2.0f) + MathF.Pow(Center.z - pos.z, 2.0f));
            float maxDistance = Size();

            if (Level < _quadTree._maxSubdivisions && distance < maxDistance)
            {
                Split();
                foreach(rlglQuadTreeElement quad in children)
                {
                    quad.SplitIfNear(pos, ref quads);
                }
            }
            else
            {
                quads.Add(this);
            }

        }
        public void Split()
        {
            float childrenSize = Size() / 2.0f;
            children = new rlglQuadTreeElement[4]
            {
                new rlglQuadTreeElement(_quadTree, Center + 0.5f * childrenSize * new GlmNet.vec3(-1.0f, -1.0f, 0.0f), Level + 1),
                new rlglQuadTreeElement(_quadTree, Center + 0.5f * childrenSize * new GlmNet.vec3( 1.0f, -1.0f, 0.0f), Level + 1),
                new rlglQuadTreeElement(_quadTree, Center + 0.5f * childrenSize * new GlmNet.vec3( 1.0f,  1.0f, 0.0f), Level + 1),
                new rlglQuadTreeElement(_quadTree, Center + 0.5f * childrenSize * new GlmNet.vec3(-1.0f,  1.0f, 0.0f), Level + 1)
            };
        }

    }
    public class rlglQuadTree
    {
        public float _totalSize { get; private set; }
        public int _maxSubdivisions { get; private set; }
        rlglQuadTreeElement root;
        
        public rlglQuadTree(float totalSize, int maxSubdivision)
        {
            _totalSize = totalSize;
            _maxSubdivisions = maxSubdivision;
            root = new rlglQuadTreeElement(this, new GlmNet.vec3(0.0f), 0);
        }
        
        public List<rlglQuadTreeElement> GetQuads(GlmNet.vec3 pos)
        {
            List<rlglQuadTreeElement> quads = new List<rlglQuadTreeElement>();
            root.SplitIfNear(pos, ref quads);
            return quads;
        }

    }
}
