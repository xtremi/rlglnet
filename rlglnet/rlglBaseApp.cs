using System;
using System.IO;
using GLFW;
using GlmNet;
using static OpenGL.Gl;

namespace rlglnet
{

    public class rlglBaseApp
    {
        private static Random rand;

        public Window window { get; private set; }
        private CameraControl cameraControl;
        private Camera camera;

        int uniColLoc;
        int uniVPMloc;
        int uniLightPos;
        long frameCounter = 0;
        int totalNodes;

        public void InitWindow(vec2 windowSize)
        {
            PrepareContext();

            //Window and cursor:
            CreateWindow(1024, 800);
            //IntPtr handle = window.handle;
            Glfw.SetCursorPosition(window, windowSize.x / 2.0, windowSize.y / 2.0);
            Glfw.SetInputMode(window, GLFW.InputMode.Cursor, (int)GLFW.CursorMode.Disabled);

            //Shader:
            uint program = CreateProgram();
            uniColLoc = glGetUniformLocation(program, "uColor");
            uniVPMloc = glGetUniformLocation(program, "uVPmat");
            uniLightPos = glGetUniformLocation(program, "uLightPos");

            //Mesh:
            rlglMesh mesh = new rlglMesh();
            uint VAO, VBO;
            int nNodesPerEdge = 100;
            int totalElements = (nNodesPerEdge - 1) * (nNodesPerEdge - 1);
            totalNodes = totalElements * 6;
            float meshSize = 250.0f;
            mesh.createMesh(out VAO, out VBO, nNodesPerEdge, meshSize);

            //color:
            rand = new Random();
            SetRandomColor(uniColLoc);

            //Camera:
            cameraControl = new CameraControl();
            camera = new Camera(windowSize.x / windowSize.y, 45.0f, 0.1f, 1000.0f);
            camera.CamPos = new vec3(0.0f, 0.0f, 20.0f);
            camera.Front = new vec3(1.0f, 0.0f, -1.0f);
            camera.Up = new vec3(0.0f, 0.0f, 1.0f);

        }

        public void Run() {
            frameCounter = 0;

            while (!Glfw.WindowShouldClose(window))
            {
                loop();
            }

            Glfw.Terminate();
        }

        public void loop()
        {
            // Swap fore/back framebuffers, and poll for operating system events.
            Glfw.SwapBuffers(window);
            //Glfw.PollEvents();

            double mouseX, mouseY;
            Glfw.GetCursorPosition(window, out mouseX, out mouseY);
            cameraControl.process(ref camera, window, new vec2((float)mouseX, (float)mouseY));
            glUniformMatrix4fv(uniVPMloc, 1, false, camera.VPmatrix().to_array());

            float lightPosSpeed = 0.005f;
            float lightPosRad = 150.0f;
            float lightPosHeight = 100.0f;
            vec3 lightPos = new vec3(lightPosRad * glm.sin(lightPosSpeed * (float)frameCounter), lightPosRad * glm.cos(lightPosSpeed * (float)frameCounter), lightPosHeight);
            glUniform3f(uniLightPos, lightPos.x, lightPos.y, lightPos.z);


            if (Glfw.GetKey(window, GLFW.Keys.Escape) == InputState.Press)
            {
                Glfw.SetWindowShouldClose(window, true);
            }
            if (Glfw.GetKey(window, GLFW.Keys.Q) == InputState.Press)
            {
                glPolygonMode(GL_FRONT_AND_BACK, GL_LINE);
            }
            else if (Glfw.GetKey(window, GLFW.Keys.Q) == InputState.Release)
            {
                glPolygonMode(GL_FRONT_AND_BACK, GL_FILL);
            }

            // Clear the framebuffer to defined background color
            glClear(GL_COLOR_BUFFER_BIT);

            if (frameCounter++ % 600 == 0)
            {
                SetRandomColor(uniColLoc);
            }

            // Draw the triangle.
            glDrawArrays(GL_TRIANGLES, 0, totalNodes);
        }

        private void PrepareContext()
        {
            // Set some common hints for the OpenGL profile creation
            Glfw.WindowHint(Hint.ClientApi, ClientApi.OpenGL);
            Glfw.WindowHint(Hint.ContextVersionMajor, 3);
            Glfw.WindowHint(Hint.ContextVersionMinor, 3);
            Glfw.WindowHint(Hint.OpenglProfile, Profile.Core);
            Glfw.WindowHint(Hint.Doublebuffer, true);
            Glfw.WindowHint(Hint.Decorated, true);
        }

        private void CreateWindow(int width, int height)
        {
            // Create window, make the OpenGL context current on the thread, and import graphics functions
            window = Glfw.CreateWindow(width, height, "rlgl", Monitor.None, Window.None);
            Glfw.MakeContextCurrent(window);
            Import(Glfw.GetProcAddress);

            // Center window
            var screen = Glfw.PrimaryMonitor.WorkArea;
            var x = (screen.Width - width) / 2;
            var y = (screen.Height - height) / 2;
            Glfw.SetWindowPosition(window, x, y);
        }

        private uint CreateProgram()
        {
            var vertex = CreateShader(GL_VERTEX_SHADER, File.ReadAllText("C:/coding/Csharp/rlglnet/data/shaders/triangle.vert"));
            var fragment = CreateShader(GL_FRAGMENT_SHADER, File.ReadAllText("C:/coding/Csharp/rlglnet/data/shaders/triangle.frag"));

            var program = glCreateProgram();
            glAttachShader(program, vertex);
            glAttachShader(program, fragment);

            glLinkProgram(program);

            glDeleteShader(vertex);
            glDeleteShader(fragment);

            glUseProgram(program);
            return program;
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
                string log = glGetShaderInfoLog(shader);
                Console.WriteLine(log);
            }

            return shader;
        }

        private static void SetRandomColor(int location)
        {
            var r = (float)rand.NextDouble();
            var g = (float)rand.NextDouble();
            var b = (float)rand.NextDouble();
            glUniform3f(location, r, g, b);
        }

    }
}
