using GLFW;
using GlmNet;
using System;

namespace rlglnet
{

    class CameraControl
    {
        public CameraControl(vec2 windowCenterPos) => WindowCenterPos = windowCenterPos;

        static float KEY_MOVE_SPEED = 0.150f;
        static float MOUSE_MOVE_SPEED = 0.15f;

        float Yaw   = 0.0f;
        float Pitch = 0.0f;
        vec2 WindowCenterPos;
        public bool off { private get; set; }

        class MouseData
        {
            public vec2 prevPos;
            public vec2 offset;
            public bool firstMovement = true;
        }
        MouseData mouse = new MouseData();


        public void process(ref Camera cam, Window window, vec2 mousePos)
        {
            if (!off) { 
                processKeyEvents(ref cam, window);
                processMouseMovement(ref cam, mousePos);
            }
        }


        void processKeyEvents(ref Camera cam, Window window)
        {
            if (Glfw.GetKey(window, GLFW.Keys.W) == InputState.Press)
            {
                cam.moveForward(KEY_MOVE_SPEED);
            }
            else if (Glfw.GetKey(window, GLFW.Keys.S) == InputState.Press)
            {
                cam.moveBacward(KEY_MOVE_SPEED);
            }
            if (Glfw.GetKey(window, GLFW.Keys.A) == InputState.Press)
            {
                cam.moveLeft(KEY_MOVE_SPEED);
            }
            else if (Glfw.GetKey(window, GLFW.Keys.D) == InputState.Press)
            {
                cam.moveRight(KEY_MOVE_SPEED);
            }
        }



        void processMouseMovement(ref Camera cam, vec2 pos)
        {
            if (mouse.firstMovement)
            {
                //mouse.firstMovement = false;
                //mouse.prevPos = pos;
            }
            mouse.offset.x = pos.x - WindowCenterPos.x;
            mouse.offset.y = WindowCenterPos.y - pos.y;
            mouse.prevPos = pos;

            mouse.offset *= MOUSE_MOVE_SPEED;

            Yaw -= mouse.offset.x;
            Pitch += mouse.offset.y;

            Pitch = Math.Clamp(Pitch, -89.9f, 89.9f);

            vec3 front;
            front.x = glm.cos(glm.radians(Yaw)) * glm.cos(glm.radians(Pitch));
            front.y = glm.sin(glm.radians(Yaw)) * glm.cos(glm.radians(Pitch));
            front.z = glm.sin(glm.radians(Pitch));
            cam.Front = glm.normalize(front);
        }

    }
}