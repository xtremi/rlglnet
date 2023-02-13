using System;
using System.Collections.Generic;
using System.Text;

namespace rlglnet
{

    class rlglObject
    {
        public rlglObject()
        {
            ScaleVec = new GlmNet.vec3(1.0f);
            Position = new GlmNet.vec3(0.0f);
            RotAxis = new GlmNet.vec3(1.0f, 0.0f, 0.0f);
            RotAngle = 1.0f;
        }
        public GlmNet.vec3 ScaleVec
        {
            get { return ScaleVec; }
            set {
                ScaleVec = value;
                NeedModelMatrixCalc = true;
            } 
        }
        public GlmNet.vec3 Position
        {
            get { return Position; }
            set
            {
                Position = value;
                NeedModelMatrixCalc = true;
            }
        }
        public GlmNet.vec3 RotAxis
        {
            get { return RotAxis; }
            set
            {
                RotAxis = value;
                NeedModelMatrixCalc = true;
            }
        }
        public float RotAngle
        {
            get { return RotAngle; }
            set
            {
                RotAngle = value;
                NeedModelMatrixCalc = true;
            }
        }
        public GlmNet.mat4 ModelMatrix { get; set; } = new GlmNet.mat4(1.0f);

        public bool NeedModelMatrixCalc { get; private set; } = false;

    }
    public class rlglRenderableObject : rlglObject
    {
        public rlglShader Shader { get; private set; }
        public rlglMesh   Mesh { get; private set; }
        public GlmNet.vec4 Color { get; set; } = new GlmNet.vec4(1.0f);


    }
}
