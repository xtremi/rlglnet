using GlmNet;
using System;

namespace rlglnet
{
    class Camera
    {
        public Camera(
            float aspectRatio,
            float fov,
            float near,
            float far)
        {
            AspectRatio = aspectRatio;
            Fov = fov;
            Near = near;
            Far = far;
            UpdateProjectionMatrix();
        }

        public void UpdateViewMatrix()
        {
            ViewMatrix = glm.lookAt(CamPos, CamPos + Front, Up);
        }

        private void UpdateProjectionMatrix()
        {
            ProjectionMatrix = glm.perspective(
                glm.radians(Fov),
                AspectRatio,
                Near,
                Far);
        }
        public void moveForward(float v)
        {
            CamPos += Front * v;
        }
        public void moveBacward(float v)
        {
            moveForward(-v);
        }
        public void moveLeft(float v)
        {
            moveSidewise(v);
        }
        public void moveRight(float v)
        {
            moveSidewise(-v);
        }
        void moveSidewise(float v)
        {
            CamPos += sideVec() * v;
        }

        public vec3 sideVec()
        {
            return glm.normalize(glm.cross(Up, Front));
        }

        public mat4 VPmatrix()
        {
            UpdateViewMatrix();
            return ProjectionMatrix * ViewMatrix;
        }
        public mat4 ProjectionMatrix { get; private set; }
        public mat4 ViewMatrix { get; private set; }

        public vec3 Front { get; set; }
        public vec3 CamPos { get; set; }
        public vec3 Up { get; set; }

        public float Fov;
        public float Near;
        public float Far;
        public float AspectRatio;

    }
}