

using System.Collections;
using System.Collections.Generic;

namespace rlglnet.Terrain
{
    class rlglTerrainMeshManager : IEnumerable<rlglSurfaceMesh>
    {
        private Dictionary<string, rlglSurfaceMesh> _meshMap = new Dictionary<string, rlglSurfaceMesh>();
        private int _nNodesPerEdge;
        public rlglTerrainMeshManager(int nNodesPerEdge) => _nNodesPerEdge = nNodesPerEdge;

        rlglSurfaceMesh NewTerrainMeshChunk(GlmNet.vec3 center, float size)
        {
            string key = MakeChunkKey(center, size);
            rlglSurfaceMesh surfaceMesh = new rlglSurfaceMesh(center, size, _nNodesPerEdge);
            surfaceMesh.Init();
            surfaceMesh.UpdateMeshHeight();
            _meshMap[key] = surfaceMesh;
            return surfaceMesh;
        }

        public rlglSurfaceMesh GetTerrainMeshChunk(GlmNet.vec3 center, float size)
        {
            string key = MakeChunkKey(center, size);
            rlglSurfaceMesh surfaceMesh;
            if (_meshMap.TryGetValue(key, out surfaceMesh))
            {
                return surfaceMesh;
            }
            else
            {
                return NewTerrainMeshChunk(center, size);
            }
        }

        string MakeChunkKey(GlmNet.vec3 center, float size)
        {
            string key = "";
            key += center.x.ToString("0.0");
            key += ",";
            key += center.y.ToString("0.0");
            key += "s";
            key += size.ToString("0.0");
            return key;
        }

        public IEnumerator<rlglSurfaceMesh> GetEnumerator()
        {
            return _meshMap.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
