using static OpenGL.Gl;

namespace rlglnet
{
    public class rlglTerrainShader : rlglShader
    {
        int uniColLoc;
        int uniVPloc;
        int uniMloc;
        int uniLightPos;
        public override void LocateUniforms()
        {
            uniColLoc   = glGetUniformLocation(ID, "uColor");
            uniVPloc    = glGetUniformLocation(ID, "uVPmat");
            uniMloc     = glGetUniformLocation(ID, "uMmat");
            uniLightPos = glGetUniformLocation(ID, "uLightPos");
        }
        public void SetColorUniform(GlmNet.vec3 color)
        {
            SetVec3uniform(color, uniColLoc);
        }
        public void SetLightPosUniform(GlmNet.vec3 lightPos)
        {
            SetVec3uniform(lightPos, uniLightPos);
        }
        public void SetModelMatrixUniform(GlmNet.mat4 modelMatrix)
        {
            SetMat4uniform(modelMatrix, uniMloc);
        }
        public void SetVPmatrixUniform(GlmNet.mat4 vpMatrix)
        {
            SetMat4uniform(vpMatrix, uniVPloc);
        }
    }
}
