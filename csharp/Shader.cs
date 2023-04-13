using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Boids
{
    // Shader class performs library graphics software to apply mathematical objects to be rendered to the screen dynamically in real time
   public class Shader
   {
        // creating unique object ID and retreiving with a getter
       private int Id;
       public int ID { get => Id; }
       //Empty Object Initialiser
       public Shader()
       {

       }
       // Object Initialiser (perameters linked to drawing object
       //VertexPath is the data from which the class draws on the vertices
       //FragmentPath is the data which allows the prograsm to connect up the points with lines and fill in the defined shape with specified uniform colour
       
       public Shader(string VertexPath, string FragmentPath)
       {
        // sourcedata read in by program
           string VertexSource = File.ReadAllText(VertexPath);
           string FragmentSource = File.ReadAllText(FragmentPath);
           // openGL shaders definition 
           int VertexShader = GL.CreateShader(ShaderType.VertexShader);
           int FragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            // openGL shader source replaced from defaults
           GL.ShaderSource(VertexShader, VertexSource);
           GL.ShaderSource(FragmentShader, FragmentSource);
            //open GL shader compiling of source code strings
           GL.CompileShader(VertexShader);
           GL.CompileShader(FragmentShader);
            //Function call to verify shaders are applied properly
           CheckShader(VertexShader);
           CheckShader(FragmentShader);
            // creates program object
           Id = GL.CreateProgram();
           //attaches shader to program object
           GL.AttachShader(Id, VertexShader);
           GL.AttachShader(Id, FragmentShader);
            // links program object to (all user defined uniform variables will be initialised to zero), then preforms check on link
           GL.LinkProgram(Id);
           CheckProgram(Id);
            // clears shader data
           GL.DeleteShader(VertexShader);
           GL.DeleteShader(FragmentShader);
       }    
       // error checking function within shader compilation
       private void CheckShader(int Shader)
       {
           GL.GetShader(Shader, ShaderParameter.CompileStatus, out int Code);
           if (Code != (int)All.True)
           {
               string Log = GL.GetShaderInfoLog(Shader);
               throw new Exception("Error occurred: " + Log);
           }

       }
       // getter function to retrieve the memory location of uniform variable
       private int GetLocation(string Name)
       {
           return GL.GetUniformLocation(Id, Name);
       }
       //setter function for uniform to apply properties to the graphical object
       public void SetFloat(string Name, float Value)
       {
           GL.Uniform1(GetLocation(Name), Value);
       }
       //setter function of matrix to represent the uniform array of the object
       public void SetMatrix4(string Name, Matrix4 Value)
       {
           GL.UniformMatrix4(GetLocation(Name), false, ref Value);
       }
        // error checking for program link status
       private void CheckProgram(int Program)
       {
           GL.GetProgram(Program, GetProgramParameterName.LinkStatus,
out int Code);
           if (Code != (int)All.True)
           {
               throw new Exception("Error occurred while linking program");
           }
       }
       // Parses program object to the render state
       public void Bind()
       {
           GL.UseProgram(Id);
       }

   }
}