using GlmNet;
using System;
using static OpenGL.Gl;

namespace rlglnet
{
    class rlglMesh
    {
        public uint VAO { get; private set; }
        public uint VBO_D { get; private set; }
        public uint VBO_S { get; private set; }
        public unsafe void createMesh(int nNodesPerEdge, float size)
        {

            VertexDataStatic[] vertexDataStatic = CreateRectangularMesh(nNodesPerEdge, size);
            VertexDataDynamic[] vertexDataDynamic = CreateRectangularMeshHeightData_flat(nNodesPerEdge, size);
            VAO = glGenVertexArray();
            VBO_S = glGenBuffer();
            VBO_D = glGenBuffer();

            glBindVertexArray(VAO);

            glBindBuffer(GL_ARRAY_BUFFER, VBO_S);
            fixed (VertexDataStatic* v = &vertexDataStatic[0])
            {
                glBufferData(GL_ARRAY_BUFFER, sizeof(VertexData) * vertexDataStatic.Length, v, GL_STATIC_DRAW);
            }
            glBindBuffer(GL_ARRAY_BUFFER, VBO_D);
            fixed (VertexDataDynamic* v = &vertexDataDynamic[0])
            {
                glBufferData(GL_ARRAY_BUFFER, sizeof(VertexDataStatic) * vertexDataDynamic.Length, v, GL_DYNAMIC_DRAW);
            }

            glBindBuffer(GL_ARRAY_BUFFER, VBO_S);

            //position xy
            glVertexAttribPointer(0, 2, GL_FLOAT, false, sizeof(VertexDataStatic), NULL);
            glEnableVertexAttribArray(0);
            //color rgb
            glVertexAttribPointer(1, 3, GL_FLOAT, false, sizeof(VertexDataStatic), NULL);
            glEnableVertexAttribArray(1);
            //texture uv
            glVertexAttribPointer(2, 2, GL_FLOAT, false, sizeof(VertexDataStatic), NULL);
            glEnableVertexAttribArray(2);

            glBindBuffer(GL_ARRAY_BUFFER, VBO_D);
            //pos z
            glVertexAttribPointer(3, 1, GL_FLOAT, false, sizeof(VertexDataDynamic), NULL);
            glEnableVertexAttribArray(3);
            //normal xyz
            glVertexAttribPointer(4, 3, GL_FLOAT, false, sizeof(VertexDataDynamic), NULL);
            glEnableVertexAttribArray(4);
            //tangent xyz
            glVertexAttribPointer(5, 3, GL_FLOAT, false, sizeof(VertexDataDynamic), NULL);
            glEnableVertexAttribArray(5);
        }

        struct VertexDataStatic
        {
            public vec2 posXY;
            public vec3 color;
            public vec2 uv;
        }

        struct VertexDataDynamic
        {
            public float posZ;
            public vec3 normal;
            public vec3 tangent;
        }

        struct VertexData
        {
            public vec3 pos;
            public vec3 normal;
            public vec3 tangent;
            public vec3 color;
            public vec2 uv;
        }

        private VertexDataDynamic[] CreateRectangularMeshHeightData_flat(int nNodesPerEdge, float height)
        {
            int elementsPerEdge = nNodesPerEdge - 1;
            VertexDataDynamic[] vertData = new VertexDataDynamic[elementsPerEdge * elementsPerEdge * 6];
            for (int i = 0; i < vertData.Length; i++)
            {
                vertData[i].posZ = height;
                vertData[i].normal = new vec3(0.0f, 0.0f, 1.0f);
                vertData[i].tangent = new vec3(0.0f, 0.0f, 1.0f);
            }
            return vertData;
        }
        private VertexDataStatic[] CreateRectangularMesh(int nNodesPerEdge, float size)
        {
            int elementsPerEdge = nNodesPerEdge - 1;
            VertexDataStatic[] vertData = new VertexDataStatic[elementsPerEdge * elementsPerEdge * 6];

            vec2 startPos = new vec2(-0.5f * size, -0.5f * size);
            vec2 cornerPos = startPos;

            float elSize = size / (float)elementsPerEdge;
            vec2[] elNodeOffsets = new vec2[] {
                new vec2(0.0f,   0.0f),
                new vec2(elSize, 0.0f),
                new vec2(0.0f,   elSize),
                new vec2(elSize, 0.0f),
                new vec2(elSize, elSize),
                new vec2(0.0f,   elSize)
            };
            vec2[] uvCoords = new vec2[]
            {
                new vec2(0.0f, 0.0f),
                new vec2(1.0f, 0.0f),
                new vec2(0.0f, 1.0f),
                new vec2(1.0f, 0.0f),
                new vec2(1.0f, 1.0f),
                new vec2(0.0f, 1.0f),
            };

            vec3 color = new vec3(1.0f, 0.0f, 0.0f);

            int index = 0;
            for (int i = 0; i < elementsPerEdge; i++)
            {
                for (int j = 0; j < elementsPerEdge; j++)
                {
                    for (int k = 0; k < 6; k++)
                    {
                        vec2 pos = cornerPos + elNodeOffsets[k];
                        vertData[index + k].posXY = pos;
                        vertData[index + k].color = color;
                        vertData[index + k].uv = uvCoords[k];
                    }
                    cornerPos.x += elSize;
                }
                cornerPos.x = startPos.x;
                cornerPos.y += elSize;
            }
            return vertData;
        }

        private VertexData[] CreateVertexArray_test(int nNodesPerEdge, float size)
        {
            int elementsPerEdge = nNodesPerEdge - 1;
            VertexData[] vertData = new VertexData[elementsPerEdge * elementsPerEdge * 6];

            vec3 startPos = new vec3(-size / 2.0f, -size / 2.0f, 0.0f);
            vec3 cornerPos = startPos;

            float elSize = size / (float)elementsPerEdge;
            vec3[] elNodeOffsets = new vec3[] {
                new vec3(0.0f,   0.0f,   0.0f),
                new vec3(elSize, 0.0f,   0.0f),
                new vec3(0.0f,   elSize, 0.0f),
                new vec3(elSize, 0.0f,   0.0f),
                new vec3(elSize, elSize, 0.0f),
                new vec3(0.0f,   elSize, 0.0f)
            };
            vec2[] uvCoords = new vec2[]
            {
                new vec2(0.0f, 0.0f),
                new vec2(1.0f, 0.0f),
                new vec2(0.0f, 1.0f),
                new vec2(1.0f, 0.0f),
                new vec2(1.0f, 1.0f),
                new vec2(0.0f, 1.0f),
            };

            vec3 tangent = new vec3(1.0f, 0.0f, 0.0f);
            vec3 color = new vec3(1.0f, 0.0f, 0.0f);

            int index = 0;
            for (int i = 0; i < elementsPerEdge; i++)
            {

                for (int j = 0; j < elementsPerEdge; j++)
                {
                    for (int k = 0; k < 6; k++)
                    {
                        vec3 pos = cornerPos + elNodeOffsets[k];
                        pos.z = (size / 20.0f) * MathF.Pow(glm.sin(0.05f * (pos.x + pos.y)), 2.0f);
                        vertData[index + k].pos = pos;
                        vertData[index + k].tangent = tangent;
                        vertData[index + k].color = color;
                        vertData[index + k].uv = uvCoords[k];
                    }
                    /* 1 2 3 4 5 6 - 1*/
                    vec3 p1 = vertData[index].pos;
                    vec3 dir1 = glm.normalize(vertData[index + 1].pos - p1);
                    vec3 dir2 = glm.normalize(vertData[index + 2].pos - p1);
                    vec3 normal = glm.normalize(glm.cross(dir1, dir2));
                    for (int k = 0; k < 6; k++)
                    {
                        vertData[index++].normal = normal;
                    }


                    cornerPos.x += elSize;
                }
                cornerPos.x = startPos.x;
                cornerPos.y += elSize;
            }


            return vertData;
        }
    }




}