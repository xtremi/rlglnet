using System;
using System.Collections.Generic;
using System.Text;

namespace rlglnet
{

    public class rlglObject
    {
        public bool NeedModelMatrixCalc { get; private set; } = false;


        public rlglObject()
        {
        }
        private GlmNet.vec3 _scaleVec;
        private GlmNet.vec3 _position;
        private GlmNet.vec3 _rotAxis;
        private float _rotAngle;
        public GlmNet.vec3 ScaleVec
        {
            get { return _scaleVec; }
            set {
                _scaleVec = value;
                NeedModelMatrixCalc = true;
            } 
        }
        public GlmNet.vec3 Position
        {
            get { return _position; }
            set
            {
                _position = value;
                NeedModelMatrixCalc = true;
            }
        }
        public GlmNet.vec3 RotAxis
        {
            get { return _rotAxis; }
            set
            {
                _rotAxis = value;
                NeedModelMatrixCalc = true;
            }
        }
        public float RotAngle
        {
            get { return _rotAngle; }
            set
            {
                _rotAngle = value;
                NeedModelMatrixCalc = true;
            }
        }

        private GlmNet.mat4 _modelMatrix = new GlmNet.mat4(1.0f);
        public GlmNet.mat4 ModelMatrix { 
            get {
                if (NeedModelMatrixCalc)
                {
                    CalculateModelMatrix();
                    NeedModelMatrixCalc = false;
                }
                return _modelMatrix;
            }
            set {
                NeedModelMatrixCalc = false;
                _modelMatrix = value;
            } }

        public void CalculateModelMatrix()
        {
            GlmNet.mat4 m = new GlmNet.mat4(1.0f);
            GlmNet.mat4 translation = GlmNet.glm.translate(m, _position);
            GlmNet.mat4 rotation = GlmNet.glm.rotate(m, _rotAngle, _rotAxis);
            GlmNet.mat4 scale = GlmNet.glm.scale(m, _scaleVec);

            _modelMatrix = translation * rotation * scale;
            NeedModelMatrixCalc = false;
        }

    }
    public class rlglRenderableObject : rlglObject
    {
        public rlglShader Shader { get; private set; }
        public rlglMesh   Mesh { get; private set; }
        public GlmNet.vec4 Color { get; set; } = new GlmNet.vec4(1.0f);

        public rlglRenderableObject(rlglMesh mesh, rlglShader shader)
        {
            Mesh = mesh;
            Shader = shader;
        }

        public virtual void SetShaderUniformValues()
        {
        }
    }



}
