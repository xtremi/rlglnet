using GlmNet;
using System;
using static OpenGL.Gl;
using System.Runtime.InteropServices;

namespace rlglnet
{
    public class rlglSurfaceMesh : rlglMesh
    {
        public rlglSurfaceMesh(
            vec3 centerPos,
            float edgeSize,
            int nNodesPerEdge)
        {
            _centerPos = centerPos;
            _edgeSize = edgeSize;
            _nNodesPerEdge = nNodesPerEdge;
            _nElementsIndexed = (_nNodesPerEdge - 1) * (_nNodesPerEdge - 1);
            _nNodesNotIndexed = _nElementsIndexed * 6;
            _indexed = true;
        }

        struct VertexDataStatic
        {
            public vec2 posXY;
            public vec2 uv;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 0)]
        struct VertexDataDynamic
        {
            public float posZ;
            public vec3 color;
            public vec3 normal;
            public vec3 tangent;
        }

        public uint VBO_D { get; private set; }
        public uint VBO_S { get; private set; }
        public uint EBO_S { get; private set; }

        private vec3 _centerPos;
        private float _edgeSize;
        private int   _nNodesPerEdge;
        private ISurface3Dfunction _currentSurfaceFunction;


        public override unsafe void Init()
        {
            VertexDataStatic[] vertexDataStatic = CreateRectangularMeshIndexed();
            int[] elementIndices = CreateRectangularMeshIndices();
            VAO = glGenVertexArray();
            VBO_S = glGenBuffer();
            EBO_S = glGenBuffer();
            VBO_D = glGenBuffer();

            glBindVertexArray(VAO);

            glBindBuffer(GL_ARRAY_BUFFER, VBO_S);
            fixed (VertexDataStatic* vs = &vertexDataStatic[0])
            {
                glBufferData(GL_ARRAY_BUFFER, sizeof(VertexDataStatic) * vertexDataStatic.Length, vs, GL_STATIC_DRAW);
            }

            glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, EBO_S);
            fixed (int* ei = &elementIndices[0])
            {
                glBufferData(GL_ELEMENT_ARRAY_BUFFER, sizeof(int) * elementIndices.Length, ei, GL_STATIC_DRAW);
            }

            glBindBuffer(GL_ARRAY_BUFFER, VBO_S);
            //position xy
            glVertexAttribPointer(0, 2, GL_FLOAT, false, sizeof(VertexDataStatic), NULL);
            glEnableVertexAttribArray(0);
            //texture uv
            glVertexAttribPointer(1, 2, GL_FLOAT, false, sizeof(VertexDataStatic), (IntPtr)8);
            glEnableVertexAttribArray(1);

            glBindBuffer(GL_ARRAY_BUFFER, VBO_D);
            //pos z
            glVertexAttribPointer(2, 1, GL_FLOAT, false, sizeof(VertexDataDynamic), NULL);
            glEnableVertexAttribArray(2);
            //color rgb
            glVertexAttribPointer(3, 3, GL_FLOAT, false, sizeof(VertexDataDynamic), (IntPtr)4);
            glEnableVertexAttribArray(3);
            //normal xyz
            glVertexAttribPointer(4, 3, GL_FLOAT, false, sizeof(VertexDataDynamic), (IntPtr)16);
            glEnableVertexAttribArray(4);
            //tangent xyz
            glVertexAttribPointer(5, 3, GL_FLOAT, false, sizeof(VertexDataDynamic), (IntPtr)28);
            glEnableVertexAttribArray(5);
        }


        public void Translate(vec3 translation)
        {
            _centerPos += translation;
            TerrainColorFunction terrainFunction = new TerrainColorFunction();
            VertexDataDynamic[] vertexDataDynamic = CreateRectangularMeshHeightDataIndexed(_currentSurfaceFunction, terrainFunction);
            UpdateDynamicBuffer(vertexDataDynamic);
        }

        private unsafe void UpdateDynamicBuffer(VertexDataDynamic[] vertexDataDynamic)
        {
            glBindVertexArray(VAO);
            glBindBuffer(GL_ARRAY_BUFFER, VBO_D);
            fixed (VertexDataDynamic* vd = &vertexDataDynamic[0])
            {
                glBufferData(GL_ARRAY_BUFFER,
                    sizeof(VertexDataDynamic) * vertexDataDynamic.Length, vd, GL_DYNAMIC_DRAW);
            }
        }
        public unsafe void UpdateMeshHeight()
        {
            if(_currentSurfaceFunction != null) UpdateMeshHeight(_currentSurfaceFunction);
        }

        public unsafe void UpdateMeshHeight(ISurface3Dfunction surfaceFunction)
        {
            _currentSurfaceFunction = surfaceFunction;
            TerrainColorFunction terrainFunction = new TerrainColorFunction();
            //VertexDataDynamic[] vertexDataDynamic = CreateRectangularMeshHeightData(surfaceFunction, terrainFunction);
            VertexDataDynamic[] vertexDataDynamic = CreateRectangularMeshHeightDataIndexed(_currentSurfaceFunction, terrainFunction);
            UpdateDynamicBuffer(vertexDataDynamic);
        }

      
        /*!
            Create the dynamic data of the terrain:
            - height (pos z), normal, and tangent (tangent not implemented yet). 
         */
        private VertexDataDynamic[] CreateRectangularMeshHeightDataIndexed(
            ISurface3Dfunction surfaceFunc,
            IColorFunction colorFunc)
        {
            int elementsPerEdge = _nNodesPerEdge - 1;
            VertexDataDynamic[] vertData = new VertexDataDynamic[_nNodesPerEdge * _nNodesPerEdge];
            vec3[] vertPosition = new vec3[_nNodesPerEdge * _nNodesPerEdge];

            vec3 startPos = _centerPos;
            startPos -= new vec3(_edgeSize / 2.0f, _edgeSize / 2.0f, 0.0f);
            vec3 cornerPos = startPos;

            float elSize = _edgeSize / (float)elementsPerEdge;

            vec3 tangent = new vec3(1.0f, 0.0f, 0.0f);  //should be calculated
            vec3 color = new vec3(1.0f, 0.0f, 0.0f);

            //Get all position coordinates:
            int index = 0;
            float dL = elSize / 10.0f;
            for (int i = 0; i < _nNodesPerEdge; i++)
            {
                for (int j = 0; j < _nNodesPerEdge; j++)
                {
                    vertData[index].posZ = surfaceFunc.Value(cornerPos.x, cornerPos.y);
                    vertData[index].normal = surfaceFunc.Normal(cornerPos.x, cornerPos.y, dL);
                    vertData[index].tangent = tangent;

                    float colorScale = surfaceFunc.NormalizeValue(vertData[index].posZ);
                    colorFunc.Value(out vertData[index].color, colorScale, vertData[index].normal);
                    index++;

                    cornerPos.x += elSize;
                }
                cornerPos.x = startPos.x;
                cornerPos.y += elSize;
            }
            return vertData; 
        }



      
        /*!
        Given two triangles making up one rectangle as shows below.
        Where the triangles have overlapping but not shared nodes.
        This function returns a vector containing the UV coordinates
        of each node, where coords go from 0.0 to 1.0 from lower/bottom
        to top/right

        Nodes are ordered as on the figure 
            ^ Y
            |
         3&6x-----x5
            | \   |
            |   \ |
           1x-----x2&4   ---> X
         */
        public readonly vec2[] unitUVcoords = new vec2[]{
            new vec2(0.0f, 0.0f),
            new vec2(1.0f, 0.0f),
            new vec2(0.0f, 1.0f),
            new vec2(1.0f, 0.0f),
            new vec2(1.0f, 1.0f),
            new vec2(0.0f, 1.0f),
        };

        /*!
            Creates the indices of the terrain mesh.            
         */
        private int[] CreateRectangularMeshIndices()
        {
            int elementsPerEdge = _nNodesPerEdge - 1;
            int[] vertdata = new int[elementsPerEdge * elementsPerEdge * 6];

            int vertIndex = 0;
            int elIndex = 0;

            for (int i = 0; i < elementsPerEdge; i++)
            {
                for (int j = 0; j < elementsPerEdge; j++)
                {
                    int[] localQuadIndices = {
                        vertIndex,
                        vertIndex + 1,
                        vertIndex + _nNodesPerEdge + 1,
                        vertIndex + _nNodesPerEdge
                    };

                    vertdata[elIndex++] = localQuadIndices[0];
                    vertdata[elIndex++] = localQuadIndices[1];
                    vertdata[elIndex++] = localQuadIndices[2];
                    vertdata[elIndex++] = localQuadIndices[0];
                    vertdata[elIndex++] = localQuadIndices[2];
                    vertdata[elIndex++] = localQuadIndices[3];
                    vertIndex++;
                }
                vertIndex++;
            }

            return vertdata;
        }

        /*!
            Creates the static data for the terrain:
            - XY coordinates and UV texture coordinates
         */
        private VertexDataStatic[] CreateRectangularMeshIndexed()
        {
            int elementsPerEdge = _nNodesPerEdge - 1;
            VertexDataStatic[] vertdata = new VertexDataStatic[_nNodesPerEdge * _nNodesPerEdge];

            vec2 startPos = new vec2(_centerPos.x, _centerPos.y);
            startPos -= new vec2(_edgeSize / 2.0f, _edgeSize / 2.0f);
            vec2 cornerPos = startPos;

            float elsize = _edgeSize / (float)elementsPerEdge;

            int index = 0;
            for (int i = 0; i < _nNodesPerEdge; i++)
            {
                for (int j = 0; j < _nNodesPerEdge; j++)
                {
                    vertdata[index].posXY = cornerPos;
                    vertdata[index].uv = new vec2((float)j / (float)elementsPerEdge, (float)i / (float)elementsPerEdge);
                    index++;
                    cornerPos.x += elsize;
                }
                cornerPos.x = startPos.x;
                cornerPos.y += elsize;
            }
            return vertdata;
        }



        /*!
         Using CreateRectangularMeshIndex instead now.       
         */
        private VertexDataStatic[] CreateRectangularMesh()
        {
            int elementsPerEdge = _nNodesPerEdge - 1;
            VertexDataStatic[] vertdata = new VertexDataStatic[elementsPerEdge * elementsPerEdge * 6];

            vec2 startPos = new vec2(-0.5f * _edgeSize);
            vec2 cornerPos = startPos;

            float elsize = _edgeSize / (float)elementsPerEdge;
            vec2[] elnodeoffsets = GetDualTriangleNodeOffsets2D(elsize);
            vec2[] uvcoords = unitUVcoords;

            int index = 0;
            for (int i = 0; i < elementsPerEdge; i++)
            {
                for (int j = 0; j < elementsPerEdge; j++)
                {
                    for (int k = 0; k < 6; k++)
                    {
                        vec2 pos = cornerPos + elnodeoffsets[k];
                        vertdata[index].posXY = pos;
                        vertdata[index].uv = uvcoords[k];
                        index++;
                    }
                    cornerPos.x += elsize;
                }
                cornerPos.x = startPos.x;
                cornerPos.y += elsize;
            }
            return vertdata;
        }


        /*!
         Using CreateRectangularMeshHeightDataIndex instead now.       
         */
        private VertexDataDynamic[] CreateRectangularMeshHeightData(
          ISurface3Dfunction surfaceFunc,
          IColorFunction colorFunc)
        {
            int elementsPerEdge = _nNodesPerEdge - 1;
            VertexDataDynamic[] vertData = new VertexDataDynamic[elementsPerEdge * elementsPerEdge * 6];
            vec3[] vertPosition = new vec3[elementsPerEdge * elementsPerEdge * 6];

            vec3 startPos = new vec3(-_edgeSize / 2.0f, -_edgeSize / 2.0f, 0.0f);
            vec3 cornerPos = startPos;

            float elSize = _edgeSize / (float)elementsPerEdge;
            vec3[] elNodeOffsets = GetDualTriangleNodeOffsets3D(elSize);
            vec2[] uvCoords = unitUVcoords;

            vec3 tangent = new vec3(1.0f, 0.0f, 0.0f);  //should be calculated
            vec3 color = new vec3(1.0f, 0.0f, 0.0f);

            int index = 0;
            for (int i = 0; i < elementsPerEdge; i++)
            {
                for (int j = 0; j < elementsPerEdge; j++)
                {
                    for (int k = 0; k < 6; k++)
                    {
                        vertPosition[index + k] = cornerPos + elNodeOffsets[k];
                        vertPosition[index + k].z = surfaceFunc.Value(vertPosition[index + k].x, vertPosition[index + k].y);
                    }
                    /* 1 2 3 4 5 6 - 1*/
                    vec3 p1 = vertPosition[index];
                    vec3 dir1 = glm.normalize(vertPosition[index + 1] - p1);
                    vec3 dir2 = glm.normalize(vertPosition[index + 2] - p1);
                    vec3 normal = glm.normalize(glm.cross(dir1, dir2));
                    for (int k = 0; k < 6; k++)
                    {
                        vertData[index].posZ = vertPosition[index].z;

                        float cscale = (vertData[index].posZ - surfaceFunc.Min()) / (surfaceFunc.Max() - surfaceFunc.Min());
                        if (float.IsNaN(cscale))
                        {
                            cscale = 0.0f;
                        }
                        float[] col = new float[3];
                        colorFunc.Value(out vertData[index].color, cscale, normal);
                        for (int c = 0; c < 3; c++) vertData[index].color[c] = col[c];

                        vertData[index].normal = normal;
                        vertData[index++].tangent = tangent;
                    }
                    cornerPos.x += elSize;
                }
                cornerPos.x = startPos.x;
                cornerPos.y += elSize;
            }
            return vertData;
        }

        /*!

      Given two triangles making up one rectangle as shows below.
      Where the triangles have overlapping but not shared nodes.
      This function returns a vector containing the offsets as 3d vectors
      from the lower/bottom corner (x = 0, y = 0) to the individual nodes.

      Nodes are ordered as on the figure
          ^ Y
          |
       3&6x-----x5
          | \   |
          |   \ |
         1x-----x2&4   ---> X

       */
        private static vec3[] GetDualTriangleNodeOffsets3D(float elSize /*!size of element*/)
        {
            return new vec3[] {
                new vec3(0.0f,   0.0f,   0.0f),
                new vec3(elSize, 0.0f,   0.0f),
                new vec3(0.0f,   elSize, 0.0f),
                new vec3(elSize, 0.0f,   0.0f),
                new vec3(elSize, elSize, 0.0f),
                new vec3(0.0f,   elSize, 0.0f)
            };
        }
        /*!
            Similar to GetDualTriangleNodeOffsets3D but only x and y coords as Vec2
         */
        private static vec2[] GetDualTriangleNodeOffsets2D(float elSize /*!size of element*/)
        {
            return new vec2[] {
                new vec2(0.0f,   0.0f),
                new vec2(elSize, 0.0f),
                new vec2(0.0f,   elSize),
                new vec2(elSize, 0.0f),
                new vec2(elSize, elSize),
                new vec2(0.0f,   elSize)
            };
        }

        public override void Delete()
        {
            glDeleteVertexArray(VAO);
            glDeleteBuffer(VBO_S);
            glDeleteBuffer(VBO_D);
            glDeleteBuffer(EBO_S);
        }


    }




}