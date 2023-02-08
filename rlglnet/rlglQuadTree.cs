using System;
using System.Collections.Generic;
using System.Text;

namespace rlglnet
{

    class rlglQuadTreeElement
    {
        GlmNet.vec3 _center;
        int _level;
        rlglQuadTree _quadTree;
        public rlglQuadTreeElement(rlglQuadTree quadTree, GlmNet.vec3 center, int level)
        {
            _quadTree = quadTree;
            _center = center;
            _level = level;
        }

        protected rlglQuadTreeElement[] children = null;
        public bool isSplit(){
            return children != null; 
        }
        public void split()
        {
            float size = _quadTree._totalSize / (float)(_level + 2);
            children = new rlglQuadTreeElement[4]
            {
                new rlglQuadTreeElement(_quadTree, _center + size * new GlmNet.vec3(-1.0f, -1.0f, 0.0f), _level + 1),
                new rlglQuadTreeElement(_quadTree, _center + size * new GlmNet.vec3( 1.0f, -1.0f, 0.0f), _level + 1),
                new rlglQuadTreeElement(_quadTree, _center + size * new GlmNet.vec3( 1.0f,  1.0f, 0.0f), _level + 1),
                new rlglQuadTreeElement(_quadTree, _center + size * new GlmNet.vec3(-1.0f,  1.0f, 0.0f), _level + 1)
            };
        }

    }
    class rlglQuadTree
    {
        rlglQuadTree(float totalSize, int _maxSubdivision)
        {
            _totalSize = totalSize;
            _maxSubdivisions = _maxSubdivision;

            root = new rlglQuadTreeElement(this, new GlmNet.vec3(0.0f), 0);
            root.split();
        }
        public float _totalSize { get; private set; }
        int _maxSubdivisions;

        rlglQuadTreeElement root;


    }
}
