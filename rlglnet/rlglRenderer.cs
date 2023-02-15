
using System;
using System.Collections.Generic;

namespace rlglnet
{
    public class rlglRenderer
    {
        List<rlglRenderableObject> objects = new List<rlglRenderableObject>();

        public void AddObject(rlglRenderableObject obj)
        {
            objects.Add(obj);
        }
        public void Remove(rlglTerrainMeshObject obj)
        {
            objects.Remove(obj);
        }
        public void Render()
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
                //obj.Shader.SetUniformValues();  //uniforms used for all object of shader --> this is not really needed
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
