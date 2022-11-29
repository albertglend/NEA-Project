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
        private float maxVelocity = 1.0f;

        private Vector2 velocity;

        private float time;
        private Vector2 position;
        private Vector2 WindowSize;
        private Vector2 size = new Vector2(5, 10);
        private Random rnd;
        private static int VAO;

        private static float VisualRange = 60.0f;
        private static float SeparationInput = 40;
        public Vector2 Position { get => position; }
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
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO);

            GL.BufferData(BufferTarget.ArrayBuffer, Vertices.Length *
 sizeof(float), Vertices, BufferUsageHint.StaticDraw);
            GL.BufferData(BufferTarget.ElementArrayBuffer,
 Indices.Length * sizeof(uint), Indices, BufferUsageHint.StaticDraw);

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

            model = model * Rotation(Math.Atan2(velocity.Y, velocity.X) - (float)Math.PI / 2, Vector3.UnitZ);

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
            velocity.X = (float)(rnd.NextDouble() * 2 - 1) * maxVelocity;
            velocity.Y = (float)(rnd.NextDouble() * 2 - 1) * maxVelocity;
            time = (float)rnd.NextDouble();


        }
        public void Update(float DeltaTime, List<Boid> boids)
        {

            //using deltatime to ensure the velocity is always the same no matter the framerate
            position = position + velocity * DeltaTime;
            time += DeltaTime;
            if (position.Y > WindowSize.Y + 10)
                position.Y = -10;
            else if (position.Y < -10)
                position.Y = WindowSize.Y + 10;
            if (position.X > WindowSize.X + 10)
                position.X = -10;
            else if (position.X < -10)
                position.X = WindowSize.X + 10;

            List<Boid> foundBoids = FindBoidsInRange(boids);
            CollectAllVectors(foundBoids);
        }
        public List<Boid> FindBoidsInRange(List<Boid> Boids)
        {
            List<Boid> FoundBoids = new List<Boid>();

            if (position.Y < 0 || position.Y >= WindowSize.Y) return FoundBoids;

            foreach (Boid boid in Boids)
            {

                Vector2 BoidPosition = boid.Position;
                if (BoidPosition == position) continue;
                if (BoidPosition.Y < 0 || BoidPosition.Y >= WindowSize.Y) continue;

                Vector2 VectorDistance = BoidPosition - Position;
                double Distance = Math.Sqrt(VectorDistance.X * VectorDistance.X + VectorDistance.Y * VectorDistance.Y);
                if (Distance <= VisualRange)
                {
                    FoundBoids.Add(boid);
                }
            }
            return FoundBoids;
        }
        public Vector2 Cohesion(List<Boid> FoundBoids)
        {

            Vector2 CentreOfMass = position;
            foreach (Boid boid in FoundBoids)
                CentreOfMass += boid.Position;

            CentreOfMass = CentreOfMass / (FoundBoids.Count+1);
            Vector2 SteeringCentre = (CentreOfMass - position) / 100;
            return SteeringCentre;

        }
        public Vector2 Separation(List<Boid> FoundBoids)
        {
            Vector2 displacement = Vector2.Zero;
            foreach (Boid boid in FoundBoids)
            {
                    Vector2 difference = position - boid.position;
                    if (Math.Sqrt(Math.Pow(difference.X, 2.0) + Math.Pow(difference.Y, 2.0)) < SeparationInput)
                    
                        displacement = displacement - (boid.position - position);
            }
            return displacement / 100;
        }
        public Vector2 Alignment(List<Boid> FoundBoids)
        {
            Vector2 AverageDirection = Vector2.Zero;

                foreach (Boid boid in FoundBoids)
                {
                    if (FoundBoids.Count != 0)
                    AverageDirection += boid.velocity;
                }
                AverageDirection = AverageDirection / (FoundBoids.Count + 1);
            

            return AverageDirection;
        }
        public void CollectAllVectors(List<Boid> boids)
        {
            Vector2 v1 = Cohesion(boids);
            Vector2 v2 = Separation(boids);
            Vector2 v3 = Alignment(boids);
            velocity = (velocity + v1 + v2 + v3);
            velocity.Normalize();
            velocity = velocity * maxVelocity;
            position = (position + velocity);



        }

        public Matrix4 Rotation(double angle, Vector3 axis)
        {
            return Matrix4.CreateFromAxisAngle(axis, (float)angle);
        }
    }

}