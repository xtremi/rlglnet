

using System;
using System.Collections;
using System.Collections.Generic;

namespace rlglnet.Terrain
{
    public class rlglTerrainChunk
    {
        public rlglSurfaceMesh mesh; 
        public rlglTerrainMeshObject obj;
        public bool active = true;
        public string key = "";
    }
    class rlglTerrainMeshManager : IEnumerable<rlglTerrainChunk>
    {
        private rlglTerrainShader _shader;
        private Dictionary<string, rlglTerrainChunk> _meshMap = new Dictionary<string, rlglTerrainChunk>();
        private int _nNodesPerEdge;
        private ISurface3Dfunction _currentSurfaceFunc;
        private Random _random;


        public rlglTerrainMeshManager(int nNodesPerEdge, rlglTerrainShader shader) 
        {
            _nNodesPerEdge = nNodesPerEdge; 
            _shader = shader;
            _currentSurfaceFunc = new FlatSurfaceFunction();
            _random = new Random(123);
        }

        //Removes all chunks that are not active
        public void CleanUp()
        {
            foreach (Terrain.rlglTerrainChunk chunk in this)
            {
                if (!chunk.active)
                {
                    chunk.mesh.Delete();
                    _meshMap.Remove(chunk.key);
                }
            }
        }

        public void Update()
        {
            foreach (rlglTerrainChunk chunk in this)
            {
                chunk.active = false;
            }
        }
        rlglTerrainChunk NewTerrainChunk(GlmNet.vec3 center, float size)
        {
            string chunkKey = MakeChunkKey(center, size);

            rlglSurfaceMesh terrainMesh = new rlglSurfaceMesh(center, size, _nNodesPerEdge);
            terrainMesh.Init();
            terrainMesh.UpdateMeshHeight(_currentSurfaceFunc);

            rlglTerrainMeshObject terrainObj = new rlglTerrainMeshObject(terrainMesh, _shader);
            terrainObj.Color = new GlmNet.vec4(RandomColor(), 1.0f);

            rlglTerrainChunk terrainChunk = new rlglTerrainChunk { mesh = terrainMesh, obj = terrainObj, key = chunkKey };

            _meshMap[chunkKey] = terrainChunk;
            return terrainChunk;
        }

        public bool ChunkExists(GlmNet.vec3 center, float size)
        {
            string key = MakeChunkKey(center, size);
            rlglTerrainChunk terrainChunk;
            return _meshMap.TryGetValue(key, out terrainChunk);
        }
        public rlglTerrainChunk GetTerrainChunk(GlmNet.vec3 center, float size, out bool isNew)
        {
            string key = MakeChunkKey(center, size);
            rlglTerrainChunk terrainChunk;
            if (_meshMap.TryGetValue(key, out terrainChunk))
            {
                terrainChunk.active = true;
                isNew = false;
                return terrainChunk;
            }
            else
            {
                isNew = true;
                return NewTerrainChunk(center, size);
            }
        }

        public void UpdateTerrainHeight(ISurface3Dfunction surfaceFunc)
        {
            _currentSurfaceFunc = surfaceFunc;
            foreach (Terrain.rlglTerrainChunk chunk in this)
            {
                chunk.mesh.UpdateMeshHeight(surfaceFunc);
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

        public IEnumerator<rlglTerrainChunk> GetEnumerator()
        {
            return _meshMap.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        //Move to utility class
        GlmNet.vec3 RandomColor()
        {

            return new GlmNet.vec3(
                (float)_random.NextDouble(),
                (float)_random.NextDouble(),
                (float)_random.NextDouble());
        }


    }
}
