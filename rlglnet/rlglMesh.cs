using static OpenGL.Gl;
using GlmNet;
using System;

namespace rlglnet
{

    public abstract class rlglMesh
    {
        protected uint VAO = 0;
        protected bool _indexed = false;
        protected int _nElementsIndexed = 0;
        protected int _nNodesNotIndexed = 0;
        public abstract void Init();

        public abstract void Delete();
        public virtual void Draw()
        {
            if (!_indexed) { 
                glDrawArrays(GL_TRIANGLES, 0, _nNodesNotIndexed);
            }
            else
            {
                unsafe
                {
                    glDrawElements(GL_TRIANGLES, _nElementsIndexed * 6, GL_UNSIGNED_INT, NULL);
                }
            }
        }

        public virtual void Bind()
        {
            glBindVertexArray(VAO);
        }


    }

    public abstract class rlglStandardMesh : rlglMesh
    {
        protected uint VBO = 0;
        protected uint EBO = 0;

        protected struct VertexData
        {
            public vec3 pos;
            public vec3 normal;
            public vec3 color;
            public vec2 uv;
        }

        protected abstract VertexData[] CreateVertexData();
        protected abstract int[] CreateElementIndices();

        public override unsafe void Init()
        {
            VertexData[] vertexData = CreateVertexData();
            int[] elementIndices = CreateElementIndices();
            _nElementsIndexed = elementIndices.Length / 6;
            _indexed = true;

            VAO = glGenVertexArray();
            VBO = glGenBuffer();
            EBO = glGenBuffer();

            glBindVertexArray(VAO);

            glBindBuffer(GL_ARRAY_BUFFER, VBO);
            fixed (VertexData* vs = &vertexData[0])
            {
                glBufferData(GL_ARRAY_BUFFER, sizeof(VertexData) * vertexData.Length, vs, GL_STATIC_DRAW);
            }

            glBindBuffer(GL_ELEMENT_ARRAY_BUFFER, EBO);
            fixed (int* ei = &elementIndices[0])
            {
                glBufferData(GL_ELEMENT_ARRAY_BUFFER, sizeof(int) * elementIndices.Length, ei, GL_STATIC_DRAW);
            }

            glBindBuffer(GL_ARRAY_BUFFER, VBO);
            //position xyz
            glVertexAttribPointer(0, 3, GL_FLOAT, false, sizeof(VertexData), NULL);
            glEnableVertexAttribArray(0);
            //texture normal
            glVertexAttribPointer(1, 3, GL_FLOAT, false, sizeof(VertexData), (IntPtr)12);
            glEnableVertexAttribArray(1);
            //pos color
            glVertexAttribPointer(2, 3, GL_FLOAT, false, sizeof(VertexData), (IntPtr)24);
            glEnableVertexAttribArray(2);
            //color uv
            glVertexAttribPointer(3, 2, GL_FLOAT, false, sizeof(VertexData), (IntPtr)36);
            glEnableVertexAttribArray(3);

        }

        public override void Delete()
        {
            glDeleteVertexArray(VAO);
            glDeleteBuffer(VBO);
        }

    }



    public class rlglMeshQuad : rlglStandardMesh
    {
        protected override VertexData[] CreateVertexData()
        {
            vec3 normalVec = new vec3(0.0f, 0.0f, 1.0f);
            vec3 colorVec = new vec3(1.0f, 0.0f, 1.0f);

            return new VertexData[]
            {
                new VertexData{ pos = new vec3(-0.5f, -0.5f, 0.0f), uv = new vec2(0.0f, 0.0f), normal = normalVec, color = colorVec },
                new VertexData{ pos = new vec3( 0.5f, -0.5f, 0.0f), uv = new vec2(1.0f, 0.0f), normal = normalVec, color = colorVec },
                new VertexData{ pos = new vec3( 0.5f,  0.5f, 0.0f), uv = new vec2(1.0f, 1.0f), normal = normalVec, color = colorVec },
                new VertexData{ pos = new vec3(-0.5f,  0.5f, 0.0f), uv = new vec2(0.0f, 1.0f), normal = normalVec, color = colorVec }
            };
        }
        protected override int[] CreateElementIndices()
        {
            return new int[]{
                0, 1, 2, 0, 2, 3
            };
        }
    }

}