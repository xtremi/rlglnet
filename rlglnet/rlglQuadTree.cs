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
        public float size()
        {
            return _quadTree._totalSize / (float)(Level + 1);
        }
        protected rlglQuadTreeElement[] children = null;


        public bool isSplit(){
            return children != null; 
        }

        public void splitIfNear(GlmNet.vec3 pos, ref List<rlglQuadTreeElement> quads)
        {
            if(Level >= _quadTree._maxSubdivisions)
            {
                return;
            }
            
            if(GlmNet.glm.distance(pos, Center) < size())
            {
                split();
                foreach(rlglQuadTreeElement quad in children)
                {
                    quad.splitIfNear(pos, ref quads);
                }
            }
            else
            {
                quads.Add(this);
            }

        }
        public void split()
        {
            float childrenSize = size() / 2.0f;
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
        
        public List<rlglQuadTreeElement> getQuads(GlmNet.vec3 pos)
        {
            List<rlglQuadTreeElement> quads = new List<rlglQuadTreeElement>();
            root.splitIfNear(pos, ref quads);
            return quads;
        }

    }
}
