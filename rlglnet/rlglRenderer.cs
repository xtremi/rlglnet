
using System.Collections.Generic;

namespace rlglnet
{
    public class rlglRenderer
    {
        public void Render(List<rlglRenderableObject> objects)
        {
            foreach (rlglRenderableObject obj in objects)
            {
                //if in view ...
                PrepareRender(obj);
                Render(obj);
            }
        }

        protected void PrepareRender(rlglRenderableObject obj)
        {

            if (obj.Shader != previousShader)
            {
                obj.Shader.Use();
                obj.Shader.SetUniformValues();  //uniforms used for all object of shader
            }
            if (obj.Mesh != previousMesh)
            {
                obj.Mesh.Bind();
            }
            obj.SetShaderUniformValues();   //individual uniforms per object

        }

        protected void Render(rlglRenderableObject obj)
        {

            obj.Mesh.Draw();

        }


        rlglShader previousShader = null;
        rlglMesh previousMesh = null;

    }
}
