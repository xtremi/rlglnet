using GLFW;
using GlmNet;
using System;

namespace rlglnet
{

    class CameraControl
    {
        static float KEY_MOVE_SPEED = 0.055f;
        static float MOUSE_MOVE_SPEED = 0.15f;

        public void process(ref Camera cam, Window window, vec2 mousePos)
        {
            processKeyEvents(ref cam, window);
            processMouseMovement(ref cam, mousePos);
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

        class MouseData
        {
            public vec2 prevPos;
            public vec2 offset;
            public bool firstMovement = true;

        }
        MouseData mouse = new MouseData();
        float yaw = 0.0f, pitch = 0.0f;

        void processMouseMovement(ref Camera cam, vec2 pos)
        {
            if (mouse.firstMovement)
            {
                mouse.firstMovement = false;
                mouse.prevPos = pos;
            }
            mouse.offset.x = pos.x - mouse.prevPos.x;
            mouse.offset.y = mouse.prevPos.y - pos.y;
            mouse.prevPos = pos;

            mouse.offset *= MOUSE_MOVE_SPEED;

            yaw -= mouse.offset.x;
            pitch += mouse.offset.y;

            pitch = Math.Clamp(pitch, -89.9f, 89.9f);

            vec3 front;
            front.x = glm.cos(glm.radians(yaw)) * glm.cos(glm.radians(pitch));
            front.y = glm.sin(glm.radians(yaw)) * glm.cos(glm.radians(pitch));
            front.z = glm.sin(glm.radians(pitch));
            cam.Front = glm.normalize(front);
        }

    }
}