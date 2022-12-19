using static OpenGL.Gl;
using GlmNet;
using System;

namespace rlglnet
{
    class rlglMesh
    {
        /// <summary>
        /// Creates a VBO and VAO to store the vertices for a triangle.
        /// </summary>
        /// <param name="vao">The created vertex array object for the triangle.</param>
        /// <param name="vbo">The created vertex buffer object for the triangle.</param>
        public unsafe void createMesh(out uint vao, out uint vbo, int nNodesPerEdge, float size)
        {

            VertexData[] vertices = createVertexArray(nNodesPerEdge, size);
            vao = glGenVertexArray();
            vbo = glGenBuffer();

            glBindVertexArray(vao);

            glBindBuffer(GL_ARRAY_BUFFER, vbo);
            fixed (VertexData* v = &vertices[0])
            {
                glBufferData(GL_ARRAY_BUFFER, sizeof(VertexData) * vertices.Length, v, GL_STATIC_DRAW);
            }

            glVertexAttribPointer(0, 3, GL_FLOAT, false, sizeof(VertexData), NULL);
            glEnableVertexAttribArray(0);
            glVertexAttribPointer(1, 3, GL_FLOAT, false, sizeof(VertexData), NULL);
            glEnableVertexAttribArray(1);
            glVertexAttribPointer(2, 3, GL_FLOAT, false, sizeof(VertexData), NULL);
            glEnableVertexAttribArray(2);
            glVertexAttribPointer(3, 3, GL_FLOAT, false, sizeof(VertexData), NULL);
            glEnableVertexAttribArray(3);
            glVertexAttribPointer(4, 2, GL_FLOAT, false, sizeof(VertexData), NULL);
            glEnableVertexAttribArray(4);
        }


        struct VertexData
        {
            public vec3 pos;
            public vec3 normal;
            public vec3 tangent;
            public vec3 color;
            public vec2 uv;
        }

        VertexData[] createVertexArray(int nNodesPerEdge, float size)
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