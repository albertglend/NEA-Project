using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Mathematics;

namespace Boids
{
   class Window : GameWindow
   {
       public Window(GameWindowSettings gameWindowSettings,
NativeWindowSettings nativeWindowSettings)
           : base(gameWindowSettings, nativeWindowSettings)
       { 
        myShader = new Shader();
        rnd = new Random();
       }

       private Shader myShader;
       private float time;
       private int count = 0;
       private Random rnd;
       private int seed = 0;
       private RainDrop[] RainDrops = new RainDrop[1000];
       protected override void OnLoad()
       {
           base.OnLoad();
           //always starts on the same seed
           rnd = new Random(seed);
           RainDrop.Setup();
           myShader = new Shader("Shaders/VertexShader.glsl",
"Shaders/FragmentShader.glsl");
           for (int i = 0; i < RainDrops.Length; i++)
           {
                RainDrops[i] = new RainDrop(new Vector2(800, 800), rnd);
           }
       }

       protected override void OnRenderFrame(FrameEventArgs args)
       {
           base.OnRenderFrame(args);
           count++;

           myShader.Bind();
           time += (float)args.Time;
           myShader.SetFloat("u_time", time);
           Matrix4 projection = Matrix4.CreateOrthographicOffCenter(0,800, 800, 0, 0.01f, 100);
           myShader.SetMatrix4("u_projection", projection);
           
           GL.ClearColor(0.2f, 0.2f, 0.2f, 1.0f);
           GL.Clear(ClearBufferMask.ColorBufferBit);
           for (int i = 0; i < RainDrops.Length; i++)
           {
               RainDrops[i].Update((float)(args.Time));
               RainDrops[i].Draw(myShader);
           }

           SwapBuffers();      
       }
       private float GetPosition()
       {
           return (float)(rnd.NextDouble() * 800);
       }
       protected override void OnUpdateFrame(FrameEventArgs args)
       {
           base.OnUpdateFrame(args);
           KeyboardState input = KeyboardState;
           if (input.IsKeyDown(Keys.Escape))
    {
        Close(); 
    }
       }  

       protected override void OnResize(ResizeEventArgs args)
       {
           base.OnResize(args);

           GL.Viewport(0, 0, Size.X, Size.Y);
       }

       protected override void OnUnload()
       {
           base.OnUnload();
       }
   }
}
