


using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Graphics.OpenGL4;
using System.Reflection.Metadata.Ecma335;
using OpenTK.Mathematics;
using System.Runtime.CompilerServices;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System.IO;

namespace OpenGLEngine
{
    public class Game : GameWindow
    {
        private List<Entity> drawingObjects;
        private Matrix4 projection;
        private Camera camera;
        private bool keyboardEnabled;
        private bool mouseEnabled;
        private Vector2 lastMousePos;
        private Shader defaultShader;
        public Game(int width, int height, string title) : base(GameWindowSettings.Default, new NativeWindowSettings() { Size = (width, height), Title = title })
        {
            drawingObjects = new List<Entity>();
            camera = new Camera(new Vector3(0f, 0f, 3f), (float)Size.X/(float)Size.Y);
            keyboardEnabled = true;
            mouseEnabled = true;
            CursorState = CursorState.Grabbed;
        }
        public Shader getDefaultShader()
        {
            string shaderPath = "C:\\Users\\brian\\source\\repos\\OpenGLEngine\\OpenGLEngine\\Shaders\\";
            if (defaultShader == null)
                defaultShader = new Shader($"{shaderPath}vertex.glsl", $"{shaderPath}frag.glsl");
            return defaultShader;
        }
        protected override void OnLoad()
        {
            base.OnLoad();
            setClearColor(0.2f, 0.3f, 0.3f);
            GL.Enable(EnableCap.DepthTest);
            lastMousePos = new Vector2(MouseState.X, MouseState.Y);
        }
        
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            foreach(Entity obj in drawingObjects)
            {
                obj.draw(camera);
            }

            SwapBuffers();
        }

        /// <summary>
        /// Inputs are registered here
        /// </summary>
        /// <param name="e"></param>
        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);
            float cameraSpeed = 1.5f;
            float sensitivity = 0.2f;
            if (KeyboardState.IsKeyDown(Keys.Escape))
                Close();
            if (keyboardEnabled)
            {
                if(KeyboardState.IsKeyDown(Keys.W))
                    camera.Position += camera.Front * cameraSpeed * (float)e.Time; //Forward 

                if (KeyboardState.IsKeyDown(Keys.S))
                    camera.Position -= camera.Front * cameraSpeed * (float)e.Time; //Backwards

                if (KeyboardState.IsKeyDown(Keys.A))
                    camera.Position -= camera.Right * cameraSpeed * (float)e.Time; //Left

                if (KeyboardState.IsKeyDown(Keys.D))
                    camera.Position += camera.Right * cameraSpeed * (float)e.Time; //Right

                if (KeyboardState.IsKeyDown(Keys.Space))
                    camera.Position += camera.Up * cameraSpeed * (float)e.Time; //Up

                if (KeyboardState.IsKeyDown(Keys.LeftControl))
                    camera.Position -= camera.Up * cameraSpeed * (float)e.Time; //Down
            }
            if (mouseEnabled)
            {
                // Calculate the offset of the mouse position
                var deltaX = MouseState.X - lastMousePos.X;
                var deltaY = MouseState.Y - lastMousePos.Y;
                lastMousePos = new Vector2(MouseState.X, MouseState.Y);

                // Apply the camera pitch and yaw (we clamp the pitch in the camera class)
                camera.Yaw += deltaX * sensitivity;
                camera.Pitch -= deltaY * sensitivity; // Reversed since y-coordinates range from bottom to top
            }
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(0, 0, Size.X, Size.Y);
        }
        protected override void OnUnload()
        {
            base.OnUnload();
            foreach(Entity entity in drawingObjects)
            {
                entity.Dispose();
            }
        }
        public void addDrawableObject(Entity obj)
        {
            obj.setProjectionMat(projection);
            drawingObjects.Add(obj);            
        }
        public void setClearColor(float r, float g, float b)
        {
            GL.ClearColor(r, g, b, 1f);
        }
    }
}