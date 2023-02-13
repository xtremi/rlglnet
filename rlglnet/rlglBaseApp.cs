using System;
using System.IO;
using System.Diagnostics;

using GLFW;
using GlmNet;
using static OpenGL.Gl;
using System.Collections.Generic;

namespace rlglnet
{
    public class rlglBaseApp
    {
        //BaseApp
        private List<rlglRenderableObject> renderObjects;
        public Window window { get; private set; }
        private CameraControl cameraControl;
        private Camera camera;
        private vec2 previousCenter = new vec2(0.0f);
        long frameCounter = 0;
        vec2 windowCenter;

        private FocusCallback WindowFocusCallback;

        //Move to specific App
        private float meshBaseSize;
        private List<rlglSurfaceMesh> terrainMeshes;
        rlglQuadTree terrainQuadTree;
        List<rlglQuadTreeElement> terrainQuads;
        TerrainShader terrainShader;

        private void InitTraceLog()
        {
            string logFile = "log.txt";
            File.Delete(logFile);
            Trace.Listeners.Add(new TextWriterTraceListener(logFile, "mainlog"));
            Trace.AutoFlush = true;
        }

        public void InitApp(vec2 windowSize)
        {
            InitTraceLog();
            Trace.WriteLine("rlglBaseApp::InitTraceLog");
            InitWindow(windowSize);
            InitShaders();
            InitMeshes();
            InitRenderObjects();
            InitCamera(windowSize);
            SetFlatTerrain();

            terrainShader.SetModelMatrixUniform(new mat4(1.0f));
        }

        private void InitWindow(vec2 windowSize)
        {
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
        }

        private void InitCamera(vec2 windowSize)
        {
            //Camera:
            cameraControl = new CameraControl(windowCenter);
            camera = new Camera(windowSize.x / windowSize.y, 45.0f, 0.1f, 1000.0f);
            camera.CamPos = new vec3(0.0f, 0.0f, 125.0f);
            camera.Front = new vec3(1.0f, 1.0f, -1.0f);
            camera.Up = new vec3(0.0f, 0.0f, 1.0f);
        }

        public void InitShaders()
        {
            terrainShader = new TerrainShader();
            terrainShader.Create(
                "C:\\coding\\Csharp\\rlglnet\\data\\shaders\\triangle.vert",
                "C:\\coding\\Csharp\\rlglnet\\data\\shaders\\triangle.frag");
            terrainShader.Use();
        }
        public void InitMeshes()
        {
            int nNodesPerEdge = 40;
            meshBaseSize = 500.0f;
            terrainQuadTree = new rlglQuadTree(meshBaseSize, 5);
            terrainQuads = terrainQuadTree.GetQuads(new vec3(0.0f, 0.0f, 0.0f));

            terrainMeshes = new List<rlglSurfaceMesh>();
            foreach (rlglQuadTreeElement quad in terrainQuads)
            {
                rlglSurfaceMesh mesh = new rlglSurfaceMesh(quad.Center, quad.Size(), nNodesPerEdge);
                mesh.Init();
                terrainMeshes.Add(mesh);


                //Move to InitRenderObjects for consistency:
                rlglRenderableObject obj = new rlglRenderableObject(mesh, terrainShader);
            }
        }
        public void InitRenderObjects()
        {

        }
        private void OnWindowFocus(Window window, bool focused)
        {
            if (focused)
            {
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
                Loop();
            }
            Glfw.Terminate();
        }

        public void Loop()
        {
            // Swap fore/back framebuffers, and poll for operating system events.
            Glfw.SwapBuffers(window);
            //Glfw.PollEvents();

            double mouseX, mouseY;
            Glfw.GetCursorPosition(window, out mouseX, out mouseY);
            cameraControl.process(ref camera, window, new vec2((float)mouseX, (float)mouseY));
            
            Glfw.SetCursorPosition(window, windowCenter.x, windowCenter.y);

            terrainShader.SetVPmatrixUniform(camera.VPmatrix());

            float lightPosSpeed = 0.005f;
            float lightPosRad = 150.0f;
            float lightPosHeight = 100.0f;
            vec3 lightPos = new vec3(lightPosRad * glm.sin(lightPosSpeed * (float)frameCounter), lightPosRad * glm.cos(lightPosSpeed * (float)frameCounter), lightPosHeight);
            terrainShader.SetLightPosUniform(lightPos);

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

            
            vec2 currentTerrainPos = new vec2(camera.CamPos.x, camera.CamPos.y);
            float distance = glm.distance(currentTerrainPos, previousCenter);
            float minDistance = terrainQuadTree._totalSize / MathF.Pow(2.0f, terrainQuadTree._maxSubdivisions);

            if(distance > minDistance)
            {
                vec2 translation = currentTerrainPos - previousCenter;
                previousCenter = currentTerrainPos;
                mat4 modelM = new mat4(1.0f);
                glm.translate(modelM, new vec3(currentTerrainPos, 0.0f));
                terrainShader.SetModelMatrixUniform(modelM);
                for (int i = 0; i < terrainMeshes.Count; i++)
                {
                    terrainMeshes[i].Translate(new vec3(translation, 0.0f));
                }
            }

            for (int i = 0; i < terrainMeshes.Count; i++)
            {
                terrainShader.SetColorUniform(
                    new vec3(
                        (float)i /(float)terrainMeshes.Count, 
                        (float)i / (float)terrainMeshes.Count, 
                        (float)i / (float)terrainMeshes.Count)
                    );
                terrainMeshes[i].Draw();
            }
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


        public void SetFlatTerrain()
        {
            FlatSurfaceFunction surfaceFunction = new FlatSurfaceFunction();
            surfaceFunction.Height = 0.0f;
            foreach (rlglMesh mesh in terrainMeshes)
            {
                mesh.UpdateMeshHeight(surfaceFunction);
            }
        }
        public void SetSineWaveTerrain(float height, float waveLength)
        {
            SineSurfaceFunction surfaceFunction = new SineSurfaceFunction();
            surfaceFunction.Amplitude = height;
            surfaceFunction.WaveLength = waveLength;
            foreach (rlglMesh mesh in terrainMeshes)
            {
                mesh.UpdateMeshHeight(surfaceFunction);
            }
        }
        public void SetPlaneWaveTerrain(float height, float waveLength)
        {
            PlaneWaveFunction surfaceFunction = new PlaneWaveFunction();
            surfaceFunction.Amplitude = height;
            surfaceFunction.WaveLength = waveLength;
            foreach (rlglMesh mesh in terrainMeshes)
            {
                mesh.UpdateMeshHeight(surfaceFunction);
            }
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
            foreach (rlglMesh mesh in terrainMeshes)
            {
                mesh.UpdateMeshHeight(surfaceFunction);
            }
        }

    }
}
