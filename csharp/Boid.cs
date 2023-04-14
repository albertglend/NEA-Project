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
        // Create Controller for User Input
        private ImGuiController Controller;
        private Vector2 size = new Vector2(5, 10);
        // Create random seed variable
        private Random rnd;
        // Create Vertex Array Object variable (descriptor of vertex data)
        private static int VAO;
        // Set visual range base value of BOID
        public static float VisualRange = 40.0f;
        // Set separation input base value of BOID can be changed from the GUI
        public static float SeparationInput = 20.0f;
        // Set cohesion input base value of BOID can be changed from the GUI
        public static float CohesionInput = 100.0f;
        // Set alignment input base value of BOID can be changed from the GUI
        public static float AlignmentInput = 10.0f;

        // Getter function of BOID position
        public Vector2 Position { get => position; }
        // Create new instance of empty state BOIDs with new random seed
        public Boid()
        {
            this.rnd = new Random();
            this.Controller = new ImGuiController(800,800);
        }
        // Create new instance of BOID with random seed and window size as perameters and then reset the window
        public Boid(Vector2 WindowSize, Random rnd)
        {
            this.WindowSize = WindowSize;
            this.rnd = rnd;
            this.Controller = new ImGuiController(800,800);

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
            // apply to the model the updated rotation of the boid from the x-axis to the model for drawing based on vector position of object 
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
        // Update of BOIDs on a set framerate deltatime, the visual movement involved in the program
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
        //function to find other boids based on a vicinity of size set in the GUI by the user
        public List<Boid> FindBoidsInRange(List<Boid> Boids)
        {
            // Creates new list of BOID objects
            List<Boid> FoundBoids = new List<Boid>();
            // Fills list with all BOIDs in program window
            if (position.Y < 0 || position.Y >= WindowSize.Y) return FoundBoids;
            // Sorts out the BOIDs that are within the set circle viscinity
            foreach (Boid boid in Boids)
            {
                // Creates a position marker for current BOID
                Vector2 BoidPosition = boid.Position;
                // Disincludes Boids outside of window and itself
                if (BoidPosition.Y < 0 || BoidPosition.Y >= WindowSize.Y) continue;
                if (BoidPosition == position) continue;

                // Vector Distance of current BOID and other BOID in foreach loop
                Vector2 VectorDistance = BoidPosition - Position;
                // distance stored as double having solved with Pythagoras theorem on vector distance for mathematical comparison
                double Distance = Math.Sqrt(VectorDistance.X * VectorDistance.X + VectorDistance.Y * VectorDistance.Y);
                //mathematical comparison of interBOID distance and visual range variable to add to new list of BOIDs within the set viscinity
                if (Distance <= VisualRange)
                {
                    FoundBoids.Add(boid);
                }

            }
            // returns list of boids in viscinity
            return FoundBoids;
        }
        
        //function to find the centre point of all boids in the visual range (used in cohesion)
        public Vector2 CentreOfMass(List<Boid> FoundBoids)
        {
            // finds mean position of vector positions of all BOIDs in currentBOIDs visual range
            Vector2 CentreOfMass = position;
            foreach (Boid boid in FoundBoids)
                CentreOfMass += boid.Position;
            CentreOfMass = CentreOfMass / (FoundBoids.Count + 1);
            // returns position of centre point
            return CentreOfMass;
        }
        // cohesion function: returns vector force used so BOID turns towards neigbouring boids (affected by user in GUI)
        // the final version of this code uses perceived centre of mass which differs as it does not include the current BOIDs impact on average position of the flock
        public Vector2 Cohesion(List<Boid> FoundBoids)
        {
            // sums vector position of all other BOIDs and divides by their quantity producing a perceived centre of mass
            Vector2 CentreOfMass = Vector2.Zero;
            foreach (Boid boid in FoundBoids)
            {
                CentreOfMass += boid.Position;
            }
            CentreOfMass /= (FoundBoids.Count);
            // applies force of current BOID towards centre of mass vector position, applied with a force defined by the user
            return (CentreOfMass - Position) / CohesionInput;

        }
        // separation function: returns vector force used so each BOID stays further than a customizable distance from one another 
        public Vector2 Separation(List<Boid> FoundBoids)
        {
            // creates new vector displacement with all components instantiated to 0
            Vector2 displacement = Vector2.Zero;
            // finds the difference between the position of current BOID and each BOID in visual range
            foreach (Boid boid in FoundBoids)
            {
                Vector2 difference = position - boid.position;
                // checks if the distance is less than the separation input minimum then applies a negative force of the vector (foreign)BOID position to current BOID to create a separation force
                if (Math.Sqrt(Math.Pow(difference.X, 2.0) + Math.Pow(difference.Y, 2.0)) < SeparationInput)

                    displacement = displacement - (boid.position - position);
            }
            // either returns vector zero force or the separation force
            return displacement;
        }
        // alignment function: returns vector force based on the average vector of all BOIDs in flock so as to create parallel travel
        public Vector2 Alignment(List<Boid> FoundBoids)
        {
            // instantiating new vector for average velocity 
            Vector2 AverageDirection = Vector2.Zero;
            // calculates total sum of vector velocity of all BOIDs in visual range
            foreach (Boid boid in FoundBoids)
            {
                AverageDirection += boid.velocity;
            }
            // finds mean vector velocity from sum velocity and number of BOIDs in visual range
            AverageDirection = AverageDirection / (FoundBoids.Count);

            // returns the alignment force divided by the users input so as to effect magnitude of alignment effect
            return (AverageDirection - velocity) / AlignmentInput;
        }
        // function and implement the BOID rules' combined vector forces 
        public void CollectAllVectors(List<Boid> boids)
        {
            //sum the vector forces of all BOID rules

            Vector2 v1 = Cohesion(boids);
            Vector2 v2 = Separation(boids);
            Vector2 v3 = Alignment(boids);
            velocity = (velocity + v1 + v2 + v3);
            //normalise forces in order to not infinitely accelerate and creates basic unit length
            velocity.Normalize();
            // Creates range of max velocity for speed variation against unit length
            maxVelocity = 80.0f + (float)(rnd.NextDouble() * 40.0);
            //applies speed variation
            velocity = velocity * maxVelocity;
        }
        //function applying openGL library angle mathematics based on the axis in radians
        public Matrix4 Rotation(double angle, Vector3 axis)
        {
            return Matrix4.CreateFromAxisAngle(axis, (float)angle);
        }
    }

}