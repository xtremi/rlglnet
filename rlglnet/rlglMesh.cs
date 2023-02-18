using static OpenGL.Gl;

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




}