using System;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace Boids
{
   class Program
   {
       static void Main(string[] args)
       {
/// try catch block
           var windowSettings = new NativeWindowSettings()
           {
               Size = new OpenTK.Mathematics.Vector2i(800, 800),
               Title = "Basic Window",
               Flags = ContextFlags.ForwardCompatible
           };

           var window = new Window(GameWindowSettings.Default, windowSettings);
           window.Run();           
       }
   }
}