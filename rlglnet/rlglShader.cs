using static OpenGL.Gl;
using System.IO;

namespace rlglnet
{
    public class rlglShader
    {
        public uint ID { get; private set; }
        public uint VertShaderID { get; private set; }
        public uint FragShaderID { get; private set; }
        public string ErrorLog { get; private set; }

        virtual public void LocateUniforms() { }
        public void Use()
        {
            glUseProgram(ID);
        }
        public  bool Create(string vertShaderPath, string fragShaderPath)
        {
            VertShaderID = CreateShader(GL_VERTEX_SHADER, File.ReadAllText(vertShaderPath));
            if(VertShaderID == 0)
            {
                return false;
            }
            FragShaderID = CreateShader(GL_FRAGMENT_SHADER, File.ReadAllText(fragShaderPath));
            if (FragShaderID == 0)
            {
                return false;
            }
            ID = glCreateProgram();

            glAttachShader(ID, VertShaderID);
            glAttachShader(ID, FragShaderID);

            glLinkProgram(ID);

            glDeleteShader(VertShaderID);
            glDeleteShader(FragShaderID);
            
            LocateUniforms();
            return true;
        }

        private unsafe uint CreateShader(int type, string source)
        {
            var shader = glCreateShader(type);
            glShaderSource(shader, source);
            glCompileShader(shader);

            int arg = 0;
            glGetShaderiv(shader, GL_COMPILE_STATUS, &arg);
            if (arg == 0)
            {
                ErrorLog = glGetShaderInfoLog(shader);
                return 0;
            }

            return shader;
        }

        public void SetVec3uniform(GlmNet.vec3 value, int uniformID)
        {
            glUniform3f(uniformID, value.x, value.y, value.z);
        }

        public void SetMat4uniform(GlmNet.mat4 value, int uniformID)
        {
            glUniformMatrix4fv(uniformID, 1, false, value.to_array());
        }
    }
}