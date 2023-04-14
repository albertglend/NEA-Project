using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Mathematics;
using ImGuiNET;

namespace Boids
{
    // window class, data and function that relate to creating and upkeeping the BOIDs window
    class Window : GameWindow
    {
        // creates new instance of window with default attributes 
        public Window(GameWindowSettings gameWindowSettings,
 NativeWindowSettings nativeWindowSettings)
            : base(gameWindowSettings, nativeWindowSettings)
        {
            // creates new Shader object and Random seed object
            myShader = new Shader();
            rnd = new Random();
        }
        // creates Shader class object
        private Shader myShader;
        // creates float time variable
        private float time;
        // creates int count variable with default value zero
        private int count = 0;
        // creates new random variable
        private Random rnd;
        // creates new int seed variable with default value zero
        private int seed = 0;
        // creates new empty list of BOIDs
        private List<Boid> Boids = new List<Boid>();
        // creates new ImGUIController for GUI and user input
        private ImGuiController controller;

        //Function to run window on command Run();
        protected override void OnLoad()
        {
            base.OnLoad();
            //random set to start on default seed
            rnd = new Random(seed);
            //calls Boid class 
            Boid.Setup();
            // saves directory for shader GLSL(opengl shading language) as string,  used to apply shader data
            myShader = new Shader("Shaders/VertexShader.glsl", "Shaders/FragmentShader.glsl");
            // for loop to create new instances of BOID objects as vector data
            for (int i = 0; i < 100; i++)
            {
                Boids.Add(new Boid(new Vector2(800, 800), rnd));
            }
            // creates new instance of ImGUI controller for UI
            controller = new ImGuiController(800, 800);
        }

        // function called everytime the data is rendered to the frame for the window
        protected override void OnRenderFrame(FrameEventArgs args)
        {
            // runs at specified render frame rate
            base.OnRenderFrame(args);
            // updates GUI controller to linked variables
            controller.Update(this, (float)args.Time);
            //increases frame count
            count++;

            // binds shader details to window for rendering
            myShader.Bind();
            //increases time since window was instantiated in seconds
            time += (float)args.Time;
            // updates uniform time based on window time
            myShader.SetFloat("u_time", time);
            // create vector z distance from camera to render the boids on a plane visible to the camera
            Matrix4 projection = Matrix4.CreateOrthographicOffCenter(0, 800, 800, 0, 0.01f, 100);
            //sets this projection as a string uniform for shaders to apply
            myShader.SetMatrix4("u_projection", projection);

            // sets colour of background with GL shader (dark grey default)
            GL.ClearColor(0.2f, 0.2f, 0.2f, 1.0f);
            // buffer and apply the colour of background
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);
            // calls the update function on every boid to retreive data from new render frame and then draw it with the shader vertices information
            for (int i = 0; i < Boids.Count; i++)
            {
                Boids[i].Update((float)(args.Time), Boids);
                Boids[i].Draw(myShader);
            }
            // test for imgui (temporary)
            if (ImGui.Begin("test"))
            {
                ImGui.Text("success");
                ImGui.End();
            }
            // render input window 
            RenderImGui();
            controller.Render();
            ImGuiController.CheckGLError("End of frame");
            //presents rendered vector GL data to the user, switches previous frame to current frame
            SwapBuffers();
        }
        //Getter function for position relative to window, multiplied by window size so as to give reflective verex data
        private float GetPosition()
        {
            return (float)(rnd.NextDouble() * 800);
        }
        //Checks to see if user inputs IO data to the application
        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);
            // checks if a key is active on each frame update
            KeyboardState input = KeyboardState;
            // if "esc" is pressed the application quits
            if (input.IsKeyDown(Keys.Escape))
            {
                Close();
            }
            //temporary
            if (MouseState.IsButtonDown(MouseButton.Left))
            {

            }

            // temporary
        }
        protected void MouseFollower(MouseState mouseState)
        {

        }

        protected override void OnResize(ResizeEventArgs args)
        {
            // GL Resize commands 
            base.OnResize(args);
            GL.Viewport(0, 0, Size.X, Size.Y);

            // GUI Resize commands
            controller.WindowResized(Size.X, Size.Y);
        }

        protected override void OnUnload()
        {
            //window closing error checking Gl closing window method
            try
            {
                base.OnUnload();
            }
            catch (Exception e)
            {
                Console.WriteLine("Window closing error {0}", e);
            }
        }
        // function to render GUI boundary settings(Input reference, minimum float value, maximum float value)
            protected void RenderImGui()
            {
                // create new ImGUI object, namespace "settings"
                if (ImGui.Begin("settings"))
                {
                    //create slider data with boundary setting data and attach reference field so that user can affect the BOID force rules directly
                    ImGui.SliderFloat("visualrange", ref Boid.CohesionInput, 10.0f, 80.0f);
                    ImGui.SliderFloat("separation", ref Boid.SeparationInput, 10.0f, 80.0f);
                    ImGui.SliderFloat("separation", ref Boid.AlignmentInput, 1.0f, 20.0f);
                    ImGui.SliderFloat("separation", ref Boid.VisualRange, 10.0f, 80.0f);
                    ImGui.End();
                }
            }
        }
    }


