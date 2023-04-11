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
        // Create velocity variable and set max of variable
        private float maxVelocity = 100.0f;
        private Vector2 velocity;
        // Create time variable
        private float time;
        // Create BOID position variable (openTK)
        private Vector2 position;
        // Create WindowSize variable (openTK)
        private Vector2 WindowSize;
        // Create relative size of BOID variable (openTK)

        private ImGuiController Controller;
        private Vector2 size = new Vector2(5, 10);
        // Create random seed variable
        private Random rnd;
        // Create Vertex Array Object variable (descriptor of vertex data)
        private static int VAO;
        // Set visual range base value of BOID
        private static float VisualRange = 40.0f;
        // Set Separation magnitude base value of BOID
        private static float SeparationInput = 20.0f;
        // Getter function of BOID position
        public Vector2 Position { get => position; }
        // Create new instance of BOIDs with new random seed
        public Boid()
        {
            this.rnd = new Random();
        }
        // Create new instance of BOID with random seed and window size as perameters and then reset the window
        public Boid(Vector2 WindowSize, Random rnd)
        {
            this.WindowSize = WindowSize;
            this.rnd = rnd;
            Reset();

        }
        // Set up of BOID
        public static void Setup()
        {
            //vertices of the BOID : triangle
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
            // Generate VAO, VBO (Vertex Buffer Object) and EBO (Element Buffer Object) : Setting up the vertex date
            VAO = GL.GenVertexArray();
            int VBO = GL.GenBuffer();
            int EBO = GL.GenBuffer();
            // Binds the vertex data
            GL.BindVertexArray(VAO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO);
            // initialises buffer object (array of unformatted data object)    
            GL.BufferData(BufferTarget.ArrayBuffer, Vertices.Length *
 sizeof(float), Vertices, BufferUsageHint.StaticDraw);
            // Draws the vertices
            GL.BufferData(BufferTarget.ElementArrayBuffer,
 Indices.Length * sizeof(uint), Indices, BufferUsageHint.StaticDraw);
            // Defines generic vertex attribute data, Clears the VAO, VBO
            GL.VertexAttribPointer(0, 2,
 VertexAttribPointerType.Float, false, 2 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
            
        }

        // Draws the vertex data to the window
        public void Draw(Shader myShader)

        {
            //applying shader and creating the BOID, then scaling down, and applying a uniform (global perameters for the shader)
            GL.BindVertexArray(VAO);
            // Create model or BOID drawing
            Matrix4 model = Matrix4.CreateScale(size.X, size.Y, 1);
            // apply the relative angle rotation of the boid from the x-axis to the model
            model = model * Rotation(Math.Atan2(velocity.Y, velocity.X) - (float)Math.PI / 2, Vector3.UnitZ);
            // applies translation to model
            model = model * Matrix4.CreateTranslation(position.X, position.Y, 0);
            // applies shader uniform
            myShader.SetMatrix4("u_model", model);
            myShader.SetFloat("u_time", time);
            // Draw function (openTK) (render to screen)
            GL.DrawElements(PrimitiveType.Triangles, 3, DrawElementsType.UnsignedInt, 0);

        }
        private void Reset()
        {
            //Applies random position to the boid and sets new random velocity for direction upon window Reset (function) call
            position.X = (float)rnd.NextDouble() * WindowSize.X;
            position.Y = (float)rnd.NextDouble() * WindowSize.Y;
            velocity.X = (float)(rnd.NextDouble() * 2 - 1) * maxVelocity;
            velocity.Y = (float)(rnd.NextDouble() * 2 - 1) * maxVelocity;
            time = (float)rnd.NextDouble();


        }
        // Update of BOIDs relative to time, the visual movement involved in the program
        public void Update(float DeltaTime, List<Boid> boids)
        {

            //using deltatime to ensure the velocity is always the same no matter the framerate
            time += DeltaTime;
            // checks to see position of BOID compared to window, corrects by moving to the other side so that the window is finite
            if (position.Y > WindowSize.Y + 10)
                position.Y = -10;
            else if (position.Y < -10)
                position.Y = WindowSize.Y + 10;
            if (position.X > WindowSize.X + 10)
                position.X = -10;
            else if (position.X < -10)
                position.X = WindowSize.X + 10;
            //Make call to the FindBoidsInRange function and instantiate a list of BOID in vicinity of currentBOID
            List<Boid> foundBoids = FindBoidsInRange(boids);
            // Conditional velocity vector updater 
            if (foundBoids.Count != 0)
            {
                CollectAllVectors(foundBoids);
            }
            // updating position based on velocity due to BOID rules multiplied by the set framerate DeltaTime, essentially p = p + vt
            position = position + velocity * DeltaTime;
        }
        //function to find 
        public List<Boid> FindBoidsInRange(List<Boid> Boids)
        {
            List<Boid> FoundBoids = new List<Boid>();

            if (position.Y < 0 || position.Y >= WindowSize.Y) return FoundBoids;

            foreach (Boid boid in Boids)
            {

                Vector2 BoidPosition = boid.Position;
                if (BoidPosition.Y < 0 || BoidPosition.Y >= WindowSize.Y) continue;
                if (BoidPosition == position) continue;

                Vector2 VectorDistance = BoidPosition - Position;
                double Distance = Math.Sqrt(VectorDistance.X * VectorDistance.X + VectorDistance.Y * VectorDistance.Y);
                if (Distance <= VisualRange)
                {
                    FoundBoids.Add(boid);
                }
            }
            return FoundBoids;
        }
        public Vector2 CentreOfMass(List<Boid> FoundBoids)
        {

            Vector2 CentreOfMass = position;
            foreach (Boid boid in FoundBoids)
                CentreOfMass += boid.Position;
            CentreOfMass = CentreOfMass / (FoundBoids.Count + 1);

            return CentreOfMass;
        }

        public Vector2 Cohesion(List<Boid> FoundBoids)
        {

            Vector2 CentreOfMass = Vector2.Zero;
            foreach (Boid boid in FoundBoids)
            {
                CentreOfMass += boid.Position;
            }
            CentreOfMass /= (FoundBoids.Count);
            return (CentreOfMass - Position) / 100.0f;

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

            return displacement;
        }
        public Vector2 Alignment(List<Boid> FoundBoids)
        {
            Vector2 AverageDirection = Vector2.Zero;
            foreach (Boid boid in FoundBoids)
            {
                AverageDirection += boid.velocity;
            }
            AverageDirection = AverageDirection / (FoundBoids.Count);


            return (AverageDirection - velocity) / 8.0f;
        }
        public void CollectAllVectors(List<Boid> boids)
        {
            Vector2 v1 = Cohesion(boids);
            Vector2 v2 = Separation(boids);
            Vector2 v3 = Alignment(boids);
            velocity = (velocity + v1 + v2 + v3);
            velocity.Normalize();
            maxVelocity = 80.0f + (float)(rnd.NextDouble() * 40.0);
            velocity = velocity * maxVelocity;



        }

        public Matrix4 Rotation(double angle, Vector3 axis)
        {
            return Matrix4.CreateFromAxisAngle(axis, (float)angle);
        }
    }

}