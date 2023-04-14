using System;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace Boids
{
    // Program class to commence window application instance
   class Program
   {
       static void Main(string[] args)
       {
        // try catch exceptions of instantiation of window
        try{ 
            // sets default window settings
           var windowSettings = new NativeWindowSettings()
           {
            // creates instance of window vector size, title and flags of WindowSettings variable
               Size = new OpenTK.Mathematics.Vector2i(800, 800),
               Title = "Basic Window",
               Flags = ContextFlags.ForwardCompatible
           };
            // Creates new BOIDs window and initialises update thread
           var window = new Window(GameWindowSettings.Default, windowSettings);
           window.Run();     
        } 
        //exception catch information printed to console
        catch (Exception e)
        {
            Console.WriteLine("Error loading Game Window, Please press escape and rerun the sln {0}", e);
        }
       }
   }
}