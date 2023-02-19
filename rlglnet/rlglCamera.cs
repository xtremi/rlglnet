using GlmNet;
using rlglnet;

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
            Frustum = new Geometry.Frustum();
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

        public Geometry.Frustum Frustum { get; private set; }
        public void ComputeFrustum()
        {
            /*!
                Based on https://stackoverflow.com/questions/12836967/extracting-view-frustum-planes-gribb-hartmann-method
            */
            mat4 vpMat = VPmatrix();
            vec3 leftN = new vec3(), rightN = new vec3(), bottomN = new vec3();
            vec3 topN = new vec3(), nearN = new vec3(), farN = new vec3();

            for (int i = 2; i >= 0; i--)  leftN[i] = vpMat[i][3] + vpMat[i][0];
            for (int i = 2; i >= 0; i--)  rightN[i] = vpMat[i][3] - vpMat[i][0];
            for (int i = 2; i >= 0; i--)  bottomN[i] = vpMat[i][3] + vpMat[i][1];
            for (int i = 2; i >= 0; i--)  topN[i] = vpMat[i][3] - vpMat[i][1];
            for (int i = 2; i >= 0; i--)  nearN[i] = vpMat[i][3] + vpMat[i][2];
            for (int i = 2; i >= 0; i--) farN[i] = vpMat[i][3] - vpMat[i][2];

            Frustum.Near = new Geometry.Plane(nearN, CamPos + Front * Near);
            Frustum.Far = new Geometry.Plane(farN, CamPos + Front * Far);
            Frustum.Left = new Geometry.Plane(leftN, CamPos);
            Frustum.Right = new Geometry.Plane(rightN, CamPos);
            Frustum.Top = new Geometry.Plane(topN, CamPos);
            Frustum.Bottom = new Geometry.Plane(bottomN, CamPos);
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


    }
}