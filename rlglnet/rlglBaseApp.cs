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
        private FocusCallback WindowFocusCallback;

        rlglRenderer renderer;
        FPScontrol _fpsControl = new FPScontrol();

        public double Fps()
        {
            return _fpsControl.CurrentFPSaverage;
        }

        public Window window { get; private set; }
        private CameraControl cameraControl;
        private Camera currentCamera, camera, camera2;
        private vec2 previousCenter = new vec2(0.0f);
        long frameCounter = 0;
        vec2 windowCenter;

        //Move to Terrain Controller App
        private float _meshBaseSize;
        private int _maxQuadTreeSubd;
        private int _nNodesPerEdge;
        Terrain.rlglTerrainMeshManager terrainMeshManager;
        rlglQuadTree terrainQuadTree;
        rlglTerrainShader terrainShader;
        rlglStandardShader standardShader;

        public string FrameStatLog { get; private set; }

        private void InitTraceLog()
        {
            string logFile = "log.txt";
            File.Delete(logFile);
            Trace.Listeners.Add(new TextWriterTraceListener(logFile, "mainlog"));
            Trace.AutoFlush = true;
        }

        public void InitApp(vec2 windowSize)
        {
            renderer = new rlglRenderer();
            _nNodesPerEdge = 64;
            _meshBaseSize = 1500.0f;
            _maxQuadTreeSubd = 7;
            terrainQuadTree = new rlglQuadTree(_meshBaseSize, _maxQuadTreeSubd);
            //terrainMeshObjects = new List<rlglTerrainMeshObject>();

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
            camera = new Camera(windowSize.x / windowSize.y, 45.0f, 0.1f, 1400.0f);
            camera.CamPos = new vec3(0.0f, 0.0f, 125.0f);
            camera.Front = new vec3(1.0f, 1.0f, -1.0f);
            camera.Up = new vec3(0.0f, 0.0f, 1.0f);

            camera2 = new Camera(windowSize.x / windowSize.y, 45.0f, 0.1f, 3000.0f);
            camera2.CamPos = new vec3(0.0f, 0.0f, 400.0f);
            camera2.Front = new vec3(1.0f, 1.0f, -1.0f);
            camera2.Up = new vec3(0.0f, 0.0f, 1.0f);

            currentCamera = camera;
        }

        public void InitShaders()
        {
            terrainShader = new rlglTerrainShader();
            terrainShader.Create(
                "C:\\coding\\Csharp\\rlglnet\\data\\shaders\\terrain.vert",
                "C:\\coding\\Csharp\\rlglnet\\data\\shaders\\terrain.frag");
            //terrainShader.Use();

            standardShader = new rlglStandardShader();
            standardShader.Create(
                "C:\\coding\\Csharp\\rlglnet\\data\\shaders\\standard.vert",
                "C:\\coding\\Csharp\\rlglnet\\data\\shaders\\standard.frag");
            //standardShader.Use();
        }
        public void InitMeshes()
        {
            terrainMeshManager = new Terrain.rlglTerrainMeshManager(_nNodesPerEdge, terrainShader);
            List<rlglQuadTreeElement> terrainQuads = terrainQuadTree.GetQuads(new vec3(0.0f, 0.0f, 0.0f));
            int i = 0;

            foreach (rlglQuadTreeElement quad in terrainQuads)
            {
                bool isNew;
                Terrain.rlglTerrainChunk terrainChunk = terrainMeshManager.GetTerrainChunk(quad.Center, quad.Size(), out isNew);
                renderer.AddObject(terrainChunk.obj);
            }


            rlglMeshQuad quadMesh = new rlglMeshQuad();
            quadMesh.Init();
            rlglStandardObject quadObj = new rlglStandardObject(quadMesh, standardShader);
            renderer.AddObject(quadObj);

            quadObj.ScaleVec = new vec3(100.0f);
            quadObj.Position = new vec3(0.0f, 0.0f, 50.0f);

        }

        private int _lastNumberOfChunksGenerated = 0;
        void updateTerrain()
        {
            terrainMeshManager.Update();

            vec2 currentTerrainPos = new vec2(camera.CamPos.x, camera.CamPos.y);
            terrainQuadTree.Reset();
            camera.ComputeFrustum();
            List<rlglQuadTreeElement> newTerrainQuads = terrainQuadTree.GetQuads(camera.CamPos, camera.Frustum);
            FrameStatLog += newTerrainQuads.Count + " terrain child quads\n";

            //Removing all terrain objects from render queue:
            foreach (Terrain.rlglTerrainChunk chunk in terrainMeshManager)
            {
                renderer.Remove(chunk.obj);
            }

            //Going through all chunks again:
            int nNewChunks = 0;
            foreach (rlglQuadTreeElement quad in newTerrainQuads)
            {
                bool isNewChunk;
                Terrain.rlglTerrainChunk chunk = terrainMeshManager.GetTerrainChunk(quad.Center, quad.Size(), out isNewChunk);
                renderer.AddObject(chunk.obj);
                if (isNewChunk) nNewChunks++;
            }
            if (nNewChunks > 0) _lastNumberOfChunksGenerated = nNewChunks;
            FrameStatLog += _lastNumberOfChunksGenerated + " new chunks generated\n";


            terrainMeshManager.CleanUp();

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
            var fullLoopTime = System.Diagnostics.Stopwatch.StartNew();
            FrameStatLog = "";

            if (!_fpsControl.Process())
            {
                return;
            }

            // Swap fore/back framebuffers, and poll for operating system events.
            Glfw.SwapBuffers(window);

            //Update cursor and camera:
            double mouseX, mouseY;
            Glfw.GetCursorPosition(window, out mouseX, out mouseY);
            cameraControl.process(ref currentCamera, window, new vec2((float)mouseX, (float)mouseY));            
            Glfw.SetCursorPosition(window, windowCenter.x, windowCenter.y);

            //Update light:
            float lightPosSpeed = 0.005f;
            float lightPosRad = 150.0f;
            float lightPosHeight = 100.0f;
            vec3 lightPos = new vec3(lightPosRad * glm.sin(lightPosSpeed * (float)frameCounter), lightPosRad * glm.cos(lightPosSpeed * (float)frameCounter), lightPosHeight);

            //Update shader uniforms:
            terrainShader.SetLightPosUniform(lightPos);
            terrainShader.SetVPmatrixUniform(currentCamera.VPmatrix());

            standardShader.SetLightPosUniform(lightPos);
            standardShader.SetVPmatrixUniform(currentCamera.VPmatrix());

            //Process key events:
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
            if (Glfw.GetKey(window, GLFW.Keys.C) == InputState.Press)
            {
                currentCamera = camera2;
            }
            else
            {
                currentCamera = camera;
            }

            // Clear the framebuffer to defined background color
            glClear(GL_COLOR_BUFFER_BIT | GL_DEPTH_BUFFER_BIT);

            var stopWatch = System.Diagnostics.Stopwatch.StartNew();
            updateTerrain();
            stopWatch.Stop();
            FrameStatLog += "\tUpdateTerrain() time: " + stopWatch.ElapsedMilliseconds + "ms\n";

            stopWatch = System.Diagnostics.Stopwatch.StartNew();
            renderer.Render();
            stopWatch.Stop();
            FrameStatLog += "\tRender() time: " + stopWatch.ElapsedMilliseconds + "ms\n";

            fullLoopTime.Stop();
            FrameStatLog += "Loop time: " + fullLoopTime.ElapsedMilliseconds + "ms\n";
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


        /**************************/

        public void SetFlatTerrain()
        {
            FlatSurfaceFunction surfaceFunction = new FlatSurfaceFunction();
            surfaceFunction.Height = 0.0f;
            terrainMeshManager.UpdateTerrainHeight(surfaceFunction);
        }
        public void SetSineWaveTerrain(float height, float waveLength)
        {
            SineSurfaceFunction surfaceFunction = new SineSurfaceFunction();
            surfaceFunction.Amplitude = height;
            surfaceFunction.WaveLength = waveLength;
            terrainMeshManager.UpdateTerrainHeight(surfaceFunction);

        }
        public void SetPlaneWaveTerrain(float height, float waveLength)
        {
            PlaneWaveFunction surfaceFunction = new PlaneWaveFunction();
            surfaceFunction.Amplitude = height;
            surfaceFunction.WaveLength = waveLength;
            terrainMeshManager.UpdateTerrainHeight(surfaceFunction);
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
            terrainMeshManager.UpdateTerrainHeight(surfaceFunction);

        }




    }
}
