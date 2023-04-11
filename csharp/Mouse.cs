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
using OpenTK.Windowing.Common.Input;
using OpenTK.Input;



namespace Boids;

class CustomMouse
{
    public CustomMouse()
    {
          
    }


   public void Setup(GameWindow window)
   {
    window.MouseMove += new Action<MouseMoveEventArgs>(OnMouseMove);
   }
   private void OnMouseMove(MouseMoveEventArgs args)
   {
    Console.WriteLine("success{0} {1}, ",args.X, args.Y);

   }

}