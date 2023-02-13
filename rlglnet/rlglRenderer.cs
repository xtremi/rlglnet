
namespace rlglnet
{
    public class rlglRenderer
    {
        public void Render(List<rlglRenderableObject> objects)
        {
            foreach (rlglRenderableObject obj in objects)
            {
                Render(obj);
            }
        }

        protected void Render(rlglRenderableObject obj)
        {




        }


        rlglShader previousShader;
        rlglMesh previousMesh;

    }
}
