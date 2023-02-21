namespace rlglnet
{
    public class rlglTerrainMeshObject : rlglRenderableObject
    {
        public rlglTerrainMeshObject(rlglSurfaceMesh mesh, rlglTerrainShader shader) : base(mesh, shader)
        {
        }
        public override void SetShaderUniformValues()
        {
            ((rlglStandardShader)Shader).SetModelMatrixUniform(ModelMatrix);
            ((rlglStandardShader)Shader).SetColorUniform(new GlmNet.vec3(Color.x, Color.y, Color.z) );
        }

    }

    public class rlglStandardObject : rlglRenderableObject
    {
        public rlglStandardObject(rlglStandardMesh mesh, rlglStandardShader shader) : base(mesh, shader)
        {
        }
        public override void SetShaderUniformValues()
        {
            ((rlglStandardShader)Shader).SetModelMatrixUniform(ModelMatrix);
            ((rlglStandardShader)Shader).SetColorUniform(new GlmNet.vec3(Color.x, Color.y, Color.z));
        }

    }


}
