using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;

namespace Boids
{
   class Boid
   {
       private float maxVelocity = 100.0f;
       
       private Vector2 velocity;
       private float angle = (float)Math.PI /2 ;
       private float time;
       private Vector2 position;
       private Vector2 WindowSize;
       private Vector2 size = new Vector2(5, 10);
       private Random rnd;
       private static int VAO;
       
       private static float VisualRange = 150.0f;

    public Vector2 Position{get => position;}
       public Boid()
       {
        this.rnd = new Random();
       }
       public Boid(Vector2 WindowSize, Random rnd)
       {
           this.WindowSize = WindowSize;
           this.rnd = rnd;
           Reset();

       }
       public static void Setup()
       {
           //vertices of the droplet: triangle
           float[] Vertices = new float[]
           {
           -0.8f,-0.6f,
           0f,0.6f,
           0.8f,-0.6f,
           };
           //index of the vertices for constructing triangle
           uint[] Indices = new uint[]
           {
           0,1,2,
           };

           VAO = GL.GenVertexArray();
           int VBO = GL.GenBuffer();
           int EBO = GL.GenBuffer();

           GL.BindVertexArray(VAO);
           GL.BindBuffer(BufferTarget.ArrayBuffer,VBO);
           GL.BindBuffer(BufferTarget.ElementArrayBuffer,EBO);

           GL.BufferData(BufferTarget.ArrayBuffer, Vertices.Length *
sizeof(float), Vertices,BufferUsageHint.StaticDraw);
           GL.BufferData(BufferTarget.ElementArrayBuffer,
Indices.Length * sizeof(uint), Indices,BufferUsageHint.StaticDraw);

           GL.VertexAttribPointer(0, 2,
VertexAttribPointerType.Float, false, 2 * sizeof(float), 0);
           GL.EnableVertexAttribArray(0);
           GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
           GL.BindVertexArray(0);
       }
       public void Draw(Shader myShader)
       {
           //applying shader and creating the two triangle models, then scaling them down, and applying a uniform to the time and model
           GL.BindVertexArray(VAO);
           Matrix4 model = Matrix4.CreateScale(size.X, size.Y, 1);
           model = model * Rotation(angle - (float)Math.PI/2, Vector3.UnitZ);

           model = model * Matrix4.CreateTranslation(position.X, position.Y, 0);
           
           myShader.SetMatrix4("u_model", model);
           myShader.SetFloat("u_time", time);
           
           GL.DrawElements(PrimitiveType.Triangles, 3, DrawElementsType.UnsignedInt, 0);

       }
       private void Reset()
       {
           //random position within the space above the window, the larger position.Y range is the more spread out the raindrops are in the Y-dir
           position.X = (float)rnd.NextDouble() * WindowSize.X;
           position.Y = (float)rnd.NextDouble() * WindowSize.Y;
            angle = (float)(rnd.NextDouble() * Math.PI * 2);
           time = (float)rnd.NextDouble();

        
       }
       public void Update(float DeltaTime, List<Boid> boids)
       {
        velocity.X = maxVelocity * (float)Math.Cos(angle);
        velocity.Y = maxVelocity * (float)Math.Sin(angle);

           //using deltatime to ensure the velocity is always the same no matter the framerate
           //implementing acceleration
           position = position + velocity * DeltaTime;
           time += DeltaTime;
           //calling the position reset of the droplet
           if (position.Y > WindowSize.Y + 10 || position.Y < -10 || position.X > WindowSize.X + 10 || position.X < -10)
           {
               Reset();
           }
           List<Boid> foundBoids = FindBoidsInRange(boids);
           Console.WriteLine(foundBoids.Count);
       }
       public List<Boid> FindBoidsInRange(List<Boid> Boids)
       {
        List<Boid> FoundBoids = new List<Boid>(); 

        if(position.Y < 0 || position.Y >= WindowSize.Y) return FoundBoids;

        foreach(Boid boid in Boids)
        {
            
            Vector2 BoidPosition = boid.Position;
            if(BoidPosition == position) continue;
            if(BoidPosition.Y < 0 || BoidPosition.Y >= WindowSize.Y) continue;

            Vector2 VectorDistance = BoidPosition - Position;
            double Distance = Math.Sqrt(VectorDistance.X * VectorDistance.X + VectorDistance.Y * VectorDistance.Y);
            if(Distance <= VisualRange)
            {
                FoundBoids.Add(boid);
            }
        }
        return FoundBoids;
       }
       public void Separation()
       {
        
       }
       public Matrix4 Rotation( float angle, Vector3 axis)
       {
       return Matrix4.CreateFromAxisAngle(axis, angle);

       }
   }

}