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
   public class Shader
   {
       private int Id;
       public int ID { get => Id; }
       public Shader()
       {

       }
       public Shader(string VertexPath, string FragmentPath)
       {
           string VertexSource = File.ReadAllText(VertexPath);
           string FragmentSource = File.ReadAllText(FragmentPath);
           int VertexShader = GL.CreateShader(ShaderType.VertexShader);
           int FragmentShader = GL.CreateShader(ShaderType.FragmentShader);
           GL.ShaderSource(VertexShader, VertexSource);
           GL.ShaderSource(FragmentShader, FragmentSource);

           GL.CompileShader(VertexShader);
           GL.CompileShader(FragmentShader);
           CheckShader(VertexShader);
           CheckShader(FragmentShader);

           Id = GL.CreateProgram();
           GL.AttachShader(Id, VertexShader);
           GL.AttachShader(Id, FragmentShader);

           GL.LinkProgram(Id);
           CheckProgram(Id);

           GL.DeleteShader(VertexShader);
           GL.DeleteShader(FragmentShader);
       }
       private void CheckShader(int Shader)
       {
           GL.GetShader(Shader, ShaderParameter.CompileStatus, out int Code);
           if (Code != (int)All.True)
           {
               string Log = GL.GetShaderInfoLog(Shader);
               throw new Exception("Error occurred: " + Log);
           }

       }
       private int GetLocation(string Name)
       {
           return GL.GetUniformLocation(Id, Name);
       }
       public void SetFloat(string Name, float Value)
       {
           GL.Uniform1(GetLocation(Name), Value);
       }
       public void SetMatrix4(string Name, Matrix4 Value)
       {
           GL.UniformMatrix4(GetLocation(Name), false, ref Value);
       }

       private void CheckProgram(int Program)
       {
           GL.GetProgram(Program, GetProgramParameterName.LinkStatus,
out int Code);
           if (Code != (int)All.True)
           {
               throw new Exception("Error occurred while linking program");
           }
       }
       public void Bind()
       {
           GL.UseProgram(Id);
       }

   }
}