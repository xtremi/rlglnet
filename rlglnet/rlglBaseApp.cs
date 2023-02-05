using System;
using System.IO;
using System.Diagnostics;

using GLFW;
using GlmNet;
using static OpenGL.Gl;
using OpenGL;

namespace rlglnet
{
    public class rlglBaseApp
    {
        private static Random rand;
        private float meshBaseSize = 150.0f;
        public void SetFlatTerrain()
        {
            FlatSurfaceFunction surfaceFunction = new FlatSurfaceFunction();
            surfaceFunction.Height = 0.0f;
            mesh1.UpdateMeshHeight(surfaceFunction);
            mesh2.UpdateMeshHeight(surfaceFunction);
            mesh3.UpdateMeshHeight(surfaceFunction);
        }
        public void SetSineWaveTerrain(float height, float waveLength)
        {
            SineSurfaceFunction surfaceFunction = new SineSurfaceFunction();
            surfaceFunction.Amplitude = height;
            surfaceFunction.WaveLength= waveLength;
            mesh1.UpdateMeshHeight(surfaceFunction);
            mesh2.UpdateMeshHeight(surfaceFunction);
            mesh3.UpdateMeshHeight(surfaceFunction);
        }
        public void SetPlaneWaveTerrain(float height, float waveLength)
        {
            PlaneWaveFunction surfaceFunction = new PlaneWaveFunction();
            surfaceFunction.Amplitude = height;
            surfaceFunction.WaveLength = waveLength;
            mesh1.UpdateMeshHeight(surfaceFunction);
            mesh2.UpdateMeshHeight(surfaceFunction);
            mesh3.UpdateMeshHeight(surfaceFunction);
        }
        public void SetSimplexNoiseTerrain(vec2 offset, float amplitude, float frequency, int octaves, float persistance, float roughness)
        {
            SimplexNoise2Dfunction surfaceFunction = new SimplexNoise2Dfunction();
            surfaceFunction.Amplitude = amplitude;
            surfaceFunction.Frequency = frequency;
            surfaceFunction.Octaves = octaves;
            surfaceFunction.Persistance = persistance;
            surfaceFunction.Roughness = roughness;
            surfaceFunction.Offset = offset;
            mesh1.UpdateMeshHeight(surfaceFunction);
            mesh2.UpdateMeshHeight(surfaceFunction);
            mesh3.UpdateMeshHeight(surfaceFunction);
        }

        public Window window { get; private set; }
        private CameraControl cameraControl;
        private Camera camera;
        private rlglMesh mesh1, mesh2, mesh3, mesh4;

        int  uniColLoc;
        int  uniVPMloc;
        int  uniMloc;
        int  uniLightPos;
        long frameCounter = 0;
        int totalNodesNotIndexed;
        int  totalElementsIndexed;
        vec2 windowCenter;

        private FocusCallback WindowFocusCallback;

        public void InitWindow(vec2 windowSize)
        {
            string logFile = "log.txt";
            File.Delete(logFile);
            Trace.Listeners.Add(new TextWriterTraceListener(logFile, "mainlog"));
            Trace.AutoFlush = true;
            Trace.WriteLine("rlglBaseApp::InitWindow");

            PrepareContext();

            //Window and cursor:
            windowCenter = windowSize / 2.0f;
            CreateWindow((int)windowSize.x, (int)windowSize.y);
            WindowFocusCallback = (glfwWindow, focused) => OnWindowFocus(glfwWindow, focused);

            glEnable(GL_DEPTH_TEST);
            glEnable(GL_CULL_FACE);
            glEnable(GL_FRONT);

            Glfw.SetWindowFocusCallback(
                window, WindowFocusCallback);

            //Shader:
            uint program = CreateProgram();
            uniColLoc = glGetUniformLocation(program, "uColor");
            uniVPMloc = glGetUniformLocation(program, "uVPmat");
            uniMloc = glGetUniformLocation(program, "uMmat");
            uniLightPos = glGetUniformLocation(program, "uLightPos");

            //Mesh:
            mesh1 = new rlglMesh();
            mesh2 = new rlglMesh();
            mesh3 = new rlglMesh();
            int nNodesPerEdge = 200;

            vec3 pos1 = new vec3(0.0f, 0.0f * meshBaseSize, 0.0f);
            vec3 pos2 = new vec3(0.0f, 1.0f * meshBaseSize, 0.0f);
            vec3 pos3 = new vec3(0.0f, 2.0f * meshBaseSize, 0.0f);
            mesh1.initializeMesh(nNodesPerEdge, pos1, meshBaseSize);
            mesh2.initializeMesh(nNodesPerEdge, pos2, meshBaseSize);
            mesh3.initializeMesh(nNodesPerEdge, pos3, meshBaseSize);
            SetFlatTerrain();

            //color:
            rand = new Random();
            SetRandomColor(uniColLoc);

            //Camera:
            cameraControl = new CameraControl(windowCenter);
            camera = new Camera(windowSize.x / windowSize.y, 45.0f, 0.1f, 1000.0f);
            camera.CamPos = new vec3(-meshBaseSize/2.0f, 3.0f* meshBaseSize/2.0f, 25.0f);
            camera.Front = new vec3(1.0f, 1.0f, -1.0f);
            camera.Up = new vec3(0.0f, 0.0f, 1.0f);
        }


        private void OnWindowFocus(Window window, bool focused)
        {
            if (focused)
            {
                //int w, h;
                //Glfw.GetWindowSize(window, out w, out h);
                //Glfw.SetCursorPosition(window, (double)w / 2.0, (double)h / 2.0);
                cameraControl.off = false;
                Glfw.SetInputMode(window, GLFW.InputMode.Cursor, (int)GLFW.CursorMode.Hidden);
            }
            else
            {
                cameraControl.off = true;
            }
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
            
            Glfw.SetCursorPosition(window, windowCenter.x, windowCenter.y);

           
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
            glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);

            if (frameCounter++ % 600 == 0)
            {
                SetRandomColor(uniColLoc);
            }

            mat4 modelM1 = new mat4(1.0f);
            mat4 modelM2 = new mat4(1.0f);
            mat4 modelM3 = new mat4(1.0f);
            glm.translate(modelM1, new vec3(0.0f, 0.0f * meshBaseSize * 1.01f, 0.0f));
            glm.translate(modelM2, new vec3(0.0f, 1.0f * meshBaseSize * 1.01f, 0.0f));
            glm.translate(modelM3, new vec3(0.0f, 2.0f * meshBaseSize * 1.01f, 0.0f));

            glUniformMatrix4fv(uniMloc, 1, false, modelM1.to_array());
            mesh1.draw();
            glUniformMatrix4fv(uniMloc, 1, false, modelM2.to_array());
            mesh2.draw();
            glUniformMatrix4fv(uniMloc, 1, false, modelM3.to_array());
            mesh3.draw();
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
            //Trace.WriteLine("rlglBaseApp::CreateShader");

            var shader = glCreateShader(type);
            glShaderSource(shader, source);
            glCompileShader(shader);

            int arg = 0;
            glGetShaderiv(shader, GL_COMPILE_STATUS, &arg);
            if (arg == 0)
            {
                string log = glGetShaderInfoLog(shader);
                Trace.WriteLine("Error shader compilation");
                Trace.WriteLine(log);
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
