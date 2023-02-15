namespace rlglnet
{
    public class rlglTerrainMeshObject : rlglRenderableObject
    {
        public rlglTerrainMeshObject(rlglSurfaceMesh mesh, rlglTerrainShader shader) : base(mesh, shader)
        {
        }
        public override void SetShaderUniformValues()
        {
            ((rlglTerrainShader)Shader).SetModelMatrixUniform(ModelMatrix);
            ((rlglTerrainShader)Shader).SetColorUniform(new GlmNet.vec3(Color.x, Color.y, Color.z) );
        }

    }



}
