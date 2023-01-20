using GlmNet;
using System;
using static OpenGL.Gl;
using System.Runtime.InteropServices;

namespace rlglnet
{

    


    class rlglMesh
    {
        public uint VAO { get; private set; }
        public uint VBO_D { get; private set; }
        public uint VBO_S { get; private set; }

        private float _sizeXY;
        private int   _nNodesPerEdge;

        public unsafe void initializeMesh(int nNodesPerEdge, float size)
        {
            _sizeXY = size;
            _nNodesPerEdge = nNodesPerEdge;

            int a = sizeof(VertexDataStatic);
            int b = sizeof(VertexDataDynamic);

            VertexDataStatic[] vertexDataStatic = CreateRectangularMesh();
            VAO = glGenVertexArray();
            VBO_S = glGenBuffer();
            VBO_D = glGenBuffer();
            glBindVertexArray(VAO);

            glBindBuffer(GL_ARRAY_BUFFER, VBO_S);
            fixed (VertexDataStatic* vs = &vertexDataStatic[0])
            {
                glBufferData(GL_ARRAY_BUFFER, sizeof(VertexDataStatic) * vertexDataStatic.Length, vs, GL_STATIC_DRAW);
            }

            glBindBuffer(GL_ARRAY_BUFFER, VBO_S);
            //position xy
            glVertexAttribPointer(0, 2, GL_FLOAT, false, sizeof(VertexDataStatic), NULL);
            glEnableVertexAttribArray(0);
            //texture uv
            glVertexAttribPointer(1, 2, GL_FLOAT, false, sizeof(VertexDataStatic), NULL);
            glEnableVertexAttribArray(1);

            glBindBuffer(GL_ARRAY_BUFFER, VBO_D);
            //pos z
            glVertexAttribPointer(2, 1, GL_FLOAT, false, sizeof(VertexDataDynamic), NULL);
            glEnableVertexAttribArray(2);
            //color rgb
            glVertexAttribPointer(3, 3, GL_FLOAT, false, sizeof(VertexDataDynamic), NULL);
            glEnableVertexAttribArray(3);
            //normal xyz
            glVertexAttribPointer(4, 3, GL_FLOAT, false, sizeof(VertexDataDynamic), NULL);
            glEnableVertexAttribArray(4);
            //tangent xyz
            glVertexAttribPointer(5, 3, GL_FLOAT, false, sizeof(VertexDataDynamic), NULL);
            glEnableVertexAttribArray(5);
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

        public unsafe void UpdateMeshHeight(ISurface3Dfunction surfaceFunction)
        {
            VertexDataDynamic[] vertexDataDynamic = CreateRectangularMeshHeightData(surfaceFunction);
            UpdateDynamicBuffer(vertexDataDynamic);
        }

        //public unsafe void SetFlatMesh(float posz)
        //{
        //    VertexDataDynamic[] vertexDataDynamic = CreateRectangularMeshHeightData_flat(posz);
        //    UpdateDynamicBuffer(vertexDataDynamic);
        //}

        //public unsafe void SetSineWaveMesh(float amplitude, float waveLength)
        //{
        //    VertexDataDynamic[] vertexDataDynamic = CreateRectangularMeshHeightData_sineWave(amplitude, waveLength);
        //    UpdateDynamicBuffer(vertexDataDynamic);
        //}


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



        private VertexDataDynamic[] CreateRectangularMeshHeightData_flat(float zpos)
        {
            int elementsPerEdge = _nNodesPerEdge - 1;
            VertexDataDynamic[] vertData = new VertexDataDynamic[elementsPerEdge * elementsPerEdge * 6];
            for (int i = 0; i < vertData.Length; i++)
            {
                vertData[i].posZ = zpos;
                vertData[i].normal = new vec3(0.0f, 0.0f, 1.0f);
                vertData[i].tangent = new vec3(0.0f, 0.0f, 1.0f);
            }
            return vertData;
        }

        private VertexDataDynamic[] CreateRectangularMeshHeightData(ISurface3Dfunction surfaceFunc)
        {
            int elementsPerEdge = _nNodesPerEdge - 1;
            VertexDataDynamic[] vertData = new VertexDataDynamic[elementsPerEdge * elementsPerEdge * 6];
            vec3[] vertPosition = new vec3[elementsPerEdge * elementsPerEdge * 6];

            vec3 startPos = new vec3(-_sizeXY / 2.0f, -_sizeXY / 2.0f, 0.0f);
            vec3 cornerPos = startPos;

            float elSize = _sizeXY / (float)elementsPerEdge;
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
                        vertData[index].color = new vec3(
                            0.5f + 0.2f*cscale,
                            1.0f - 0.8f*cscale,
                            0.2f);
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

        //private VertexDataDynamic[] CreateRectangularMeshHeightData_sineWave(float amplitude, float waveLength)
        //{
        //    int elementsPerEdge = _nNodesPerEdge - 1;
        //    VertexDataDynamic[] vertData = new VertexDataDynamic[elementsPerEdge * elementsPerEdge * 6];
        //    vec3[] vertPosition = new vec3[elementsPerEdge * elementsPerEdge * 6];

        //    vec3 startPos = new vec3(-_sizeXY / 2.0f, -_sizeXY / 2.0f, 0.0f);
        //    vec3 cornerPos = startPos;

        //    float elSize = _sizeXY / (float)elementsPerEdge;
        //    vec3[] elNodeOffsets = GetDualTriangleNodeOffsets3D(elSize);
        //    vec2[] uvCoords = unitUVcoords;

        //    vec3 tangent = new vec3(1.0f, 0.0f, 0.0f);  //should be calculated
        //    vec3 color = new vec3(1.0f, 0.0f, 0.0f);
        //    float sineK = 2.0f * (float)Math.PI / waveLength;

        //    int index = 0;
        //    for (int i = 0; i < elementsPerEdge; i++)
        //    {
        //        for (int j = 0; j < elementsPerEdge; j++)
        //        {
        //            for (int k = 0; k < 6; k++)
        //            {
        //                vertPosition[index + k] = cornerPos + elNodeOffsets[k];
        //                vertPosition[index + k].z = amplitude * 
        //                    glm.sin(sineK * vertPosition[index + k].x);
        //            }
        //            /* 1 2 3 4 5 6 - 1*/
        //            vec3 p1 = vertPosition[index];
        //            vec3 dir1 = glm.normalize(vertPosition[index + 1] - p1);
        //            vec3 dir2 = glm.normalize(vertPosition[index + 2] - p1);
        //            vec3 normal = glm.normalize(glm.cross(dir1, dir2));
        //            for (int k = 0; k < 6; k++)
        //            {
        //                vertData[index].posZ = vertPosition[index].z;
        //                vertData[index].normal = normal;
        //                vertData[index++].tangent = tangent;
        //            }
        //            cornerPos.x += elSize;
        //        }
        //        cornerPos.x = startPos.x;
        //        cornerPos.y += elSize;
        //    }
        //    return vertData;
        //}

        


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



        //struct vertexData
        //{
        //    public vec3 pos;
        //    public vec3 normal;
        //    public vec3 tangent;
        //    public vec3 color;
        //    public vec2 uv;
        //}

        //public unsafe void createMesh(int nNodesPerEdge, float size)
        //{

        //    VertexDataStatic[] vertexDataStatic = CreateRectangularMesh(nNodesPerEdge, size);
        //    //VertexDataDynamic[] vertexDataDynamic = CreateRectangularMeshHeightData_flat(nNodesPerEdge, 0.0f);
        //    VertexDataDynamic[] vertexDataDynamic = CreateRectangularMeshHeightData_sineWave(nNodesPerEdge, 10.0f, size);
        //    VAO = glGenVertexArray();
        //    VBO_S = glGenBuffer();
        //    VBO_D = glGenBuffer();

        //    glBindVertexArray(VAO);

        //    glBindBuffer(GL_ARRAY_BUFFER, VBO_S);
        //    fixed (VertexDataStatic* vs = &vertexDataStatic[0])
        //    {
        //        glBufferData(GL_ARRAY_BUFFER, sizeof(VertexDataStatic) * vertexDataStatic.Length, vs, GL_STATIC_DRAW);
        //    }
        //    glBindBuffer(GL_ARRAY_BUFFER, VBO_D);
        //    fixed (VertexDataDynamic* vd = &vertexDataDynamic[0])
        //    {
        //        glBufferData(GL_ARRAY_BUFFER, sizeof(VertexDataDynamic) * vertexDataDynamic.Length, vd, GL_DYNAMIC_DRAW);
        //    }

        //    glBindBuffer(GL_ARRAY_BUFFER, VBO_S);
        //    //position xy
        //    glVertexAttribPointer(0, 2, GL_FLOAT, false, sizeof(VertexDataStatic), NULL);
        //    glEnableVertexAttribArray(0);
        //    //color rgb
        //    glVertexAttribPointer(1, 3, GL_FLOAT, false, sizeof(VertexDataStatic), NULL);
        //    glEnableVertexAttribArray(1);
        //    //texture uv
        //    glVertexAttribPointer(2, 2, GL_FLOAT, false, sizeof(VertexDataStatic), NULL);
        //    glEnableVertexAttribArray(2);

        //    glBindBuffer(GL_ARRAY_BUFFER, VBO_D);
        //    //pos z
        //    glVertexAttribPointer(3, 1, GL_FLOAT, false, sizeof(VertexDataDynamic), NULL);
        //    glEnableVertexAttribArray(3);
        //    //normal xyz
        //    glVertexAttribPointer(4, 3, GL_FLOAT, false, sizeof(VertexDataDynamic), NULL);
        //    glEnableVertexAttribArray(4);
        //    //tangent xyz
        //    glVertexAttribPointer(5, 3, GL_FLOAT, false, sizeof(VertexDataDynamic), NULL);
        //    glEnableVertexAttribArray(5);
        //}

        //private VertexData[] CreateVertexArray_test(int nNodesPerEdge, float size)
        //{
        //    int elementsPerEdge = nNodesPerEdge - 1;
        //    VertexData[] vertData = new VertexData[elementsPerEdge * elementsPerEdge * 6];

        //    vec3 startPos = new vec3(-size / 2.0f, -size / 2.0f, 0.0f);
        //    vec3 cornerPos = startPos;

        //    float elSize = size / (float)elementsPerEdge;
        //    vec3[] elNodeOffsets = getDualTriangleNodeOffsets(elSize);
        //    vec2[] uvCoords = unitUVcoords;

        //    vec3 tangent = new vec3(1.0f, 0.0f, 0.0f);
        //    vec3 color = new vec3(1.0f, 0.0f, 0.0f);

        //    int index = 0;
        //    for (int i = 0; i < elementsPerEdge; i++)
        //    {

        //        for (int j = 0; j < elementsPerEdge; j++)
        //        {
        //            for (int k = 0; k < 6; k++)
        //            {
        //                vec3 pos = cornerPos + elNodeOffsets[k];
        //                pos.z = (size / 20.0f) * MathF.Pow(glm.sin(0.05f * (pos.x + pos.y)), 2.0f);
        //                vertData[index + k].pos = pos;
        //                vertData[index + k].tangent = tangent;
        //                vertData[index + k].color = color;
        //                vertData[index + k].uv = uvCoords[k];
        //            }
        //            /* 1 2 3 4 5 6 - 1*/
        //            vec3 p1 = vertData[index].pos;
        //            vec3 dir1 = glm.normalize(vertData[index + 1].pos - p1);
        //            vec3 dir2 = glm.normalize(vertData[index + 2].pos - p1);
        //            vec3 normal = glm.normalize(glm.cross(dir1, dir2));
        //            for (int k = 0; k < 6; k++)
        //            {
        //                vertData[index++].normal = normal;
        //            }


        //            cornerPos.x += elSize;
        //        }
        //        cornerPos.x = startPos.x;
        //        cornerPos.y += elSize;
        //    }


        //    return vertData;
        //}

        private VertexDataStatic[] CreateRectangularMesh()
        {
            int elementsPerEdge = _nNodesPerEdge - 1;
            VertexDataStatic[] vertdata = new VertexDataStatic[elementsPerEdge * elementsPerEdge * 6];

            vec2 startPos = new vec2(-0.5f * _sizeXY);
            vec2 cornerPos = startPos;

            float elsize = _sizeXY / (float)elementsPerEdge;
            vec2[] elnodeoffsets = GetDualTriangleNodeOffsets2D(elsize);
            vec2[] uvcoords = unitUVcoords;

            vec3 color = new vec3(1.0f, 0.0f, 0.0f);

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

    }


}