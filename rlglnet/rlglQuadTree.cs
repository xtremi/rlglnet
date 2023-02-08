using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace rlglnet
{

    class rlglQuadTreeElement : IEnumerator { 
        GlmNet.vec3 _center;
        int _level;
        rlglQuadTree _quadTree;

        public rlglQuadTreeElement(rlglQuadTree quadTree, GlmNet.vec3 center, int level)
        {
            _quadTree = quadTree;
            _center = center;
            _level = level;
        }
        public float size()
        {
            return _quadTree._totalSize / (float)(_level + 1);
        }
        protected rlglQuadTreeElement[] children = null;


        public bool isSplit(){
            return children != null; 
        }
        public void split()
        {
            float childrenSize = size() / 2.0f;
            children = new rlglQuadTreeElement[4]
            {
                new rlglQuadTreeElement(_quadTree, _center + childrenSize * new GlmNet.vec3(-1.0f, -1.0f, 0.0f), _level + 1),
                new rlglQuadTreeElement(_quadTree, _center + childrenSize * new GlmNet.vec3( 1.0f, -1.0f, 0.0f), _level + 1),
                new rlglQuadTreeElement(_quadTree, _center + childrenSize * new GlmNet.vec3( 1.0f,  1.0f, 0.0f), _level + 1),
                new rlglQuadTreeElement(_quadTree, _center + childrenSize * new GlmNet.vec3(-1.0f,  1.0f, 0.0f), _level + 1)
            };
        }


        rlglQuadTreeElement child(uint i)
        {
            return children == null || i > 3 ? null : children[i];
        }

        public object Current => throw new NotImplementedException();

        public bool MoveNext()
        {
            throw new NotImplementedException();
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }
    }
    class rlglQuadTree : IEnumerable<rlglQuadTreeElement>
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

        public IEnumerator<rlglQuadTreeElement> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
