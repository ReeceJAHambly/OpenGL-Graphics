using Labs.Utility;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System;

namespace Labs.Lab2
{
    class Lab2_1Window : GameWindow
    {
        private int[] mVertexArrayObjectIDs = new int[2];
        private int[] mVertexBufferObjectIDArray = new int[2];
        private int[] mSquareVertexBufferObjectIDArray= new int[2];
        private ShaderUtility mShader;
        //L21T1 Rendered a black triangle
        public Lab2_1Window()
            : base(
                800, // Width
                600, // Height
                GraphicsMode.Default,
                "Lab 2_1 Linking to Shaders and VAOs",
                GameWindowFlags.Default,
                DisplayDevice.Default,
                3, // major
                3, // minor
                GraphicsContextFlags.ForwardCompatible
                )
        {
        }

        protected override void OnLoad(EventArgs e)
        {
            
            GL.Enable(EnableCap.DepthTest);
            GL.ClearColor(Color4.CadetBlue);

            GL.GenBuffers(2, mSquareVertexBufferObjectIDArray);
            float[] squareVertices = new float[] {-0.2f,-0.4f, 0.2f, 0.0f, 1.1f, 1.0f,
                                                  0.8f, -0.4f, 0.2f, 0.5f, 0.9f, 1.0f,
                                                  0.8f, 0.8f, 0.2f, 0.0f, 1.0f, 1.0f,
                                                  -0.2f, 0.8f, 0.2f, 0.0f, 1.0f, 1.0f};
            uint[] squareIndices = new uint[] { 0, 1, 2, 3 };

            GL.BindBuffer(BufferTarget.ArrayBuffer, mSquareVertexBufferObjectIDArray[0]);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(squareVertices.Length * sizeof(float)), squareVertices, BufferUsageHint.StaticDraw);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer,mSquareVertexBufferObjectIDArray[1]);
            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(squareIndices.Length * sizeof(int)), squareIndices, BufferUsageHint.StaticDraw);

            float[] vertices = new float[] { -0.8f, 0.8f, 0.4f, 1.0f, 0.0f, 1.0f,
                                             -0.6f, -0.4f, 0.4f, 1.0f, 0.0f, 1.0f,
                                             0.2f, 0.2f, 0.4f, 1.0f, 0.0f, 1.0f };

            uint[] indices = new uint[] { 0, 1, 2 };

          

            GL.GenBuffers(2, mVertexBufferObjectIDArray);
            GL.BindBuffer(BufferTarget.ArrayBuffer, mVertexBufferObjectIDArray[0]);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(vertices.Length * sizeof(float)), vertices, BufferUsageHint.StaticDraw);

            int size;
            GL.GetBufferParameter(BufferTarget.ArrayBuffer, BufferParameterName.BufferSize, out size);

            if (vertices.Length * sizeof(float) != size)
            {
                throw new ApplicationException("Vertex data not loaded onto graphics card correctly");
            }

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, mVertexBufferObjectIDArray[1]);
            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(indices.Length * sizeof(int)), indices, BufferUsageHint.StaticDraw);

            GL.GetBufferParameter(BufferTarget.ElementArrayBuffer, BufferParameterName.BufferSize, out size);

            if (indices.Length * sizeof(int) != size)
            {
                throw new ApplicationException("Index data not loaded onto graphics card correctly");
            }

            #region Shader Loading Code

            mShader = new ShaderUtility(@"Lab2/Shaders/vLab21.vert", @"Lab2/Shaders/fSimple.frag");

            #endregion
            int vColourLocation = GL.GetAttribLocation(mShader.ShaderProgramID, "vColour");
            GL.EnableVertexAttribArray(vColourLocation);


          
            base.OnLoad(e);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {  
            base.OnRenderFrame(e);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            //L21T5 Added depth by adding an extra parameter, changing the shader variable and linking and enabling depth testing
            GL.UseProgram(mShader.ShaderProgramID);
            int vColourLocation = GL.GetUniformLocation(mShader.ShaderProgramID, "vColour");
            
            int vPositionLocation = GL.GetAttribLocation(mShader.ShaderProgramID, "vPosition");
          

            GL.BindBuffer(BufferTarget.ArrayBuffer, mSquareVertexBufferObjectIDArray[0]);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, mSquareVertexBufferObjectIDArray[1]);

            #region Square Loading Code
           
            GL.EnableVertexAttribArray(vPositionLocation);
            GL.VertexAttribPointer(vPositionLocation, 3, VertexAttribPointerType.Float, false, 6 *
            sizeof(float), 0);
            GL.VertexAttribPointer(vColourLocation, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));
            #endregion

            GL.DrawElements(PrimitiveType.TriangleFan, 4, DrawElementsType.UnsignedInt, 0);
            //mShader.Delete();
            //L21T4 Moved the red triangle so it is drwan on top of the blue square
            GL.BindBuffer(BufferTarget.ArrayBuffer, mVertexBufferObjectIDArray[0]);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, mVertexBufferObjectIDArray[1]);


            #region Shader Loading Code

            GL.EnableVertexAttribArray(vPositionLocation);
            GL.VertexAttribPointer(vPositionLocation, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
            GL.VertexAttribPointer(vColourLocation, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));

            #endregion
            GL.DrawElements(PrimitiveType.Triangles, 3, DrawElementsType.UnsignedInt, 0);


            //L21T7 Changed vertex colours to see fragments colour values being blended together
            //L21T6 Added per vertex colour to the vertex shader, passed it to the fragment shader and linked the data buffer to the shader program
            //L21T2 Set uniform variable in the fragment shader to colour all fragments red
            this.SwapBuffers();
            //L21T3 Added a blue sqaure by creating new data and element arrays, linking the appropriate variables to the shader
            //and changing the fragment shaders uniform colour variable
        }

        protected override void OnUnload(EventArgs e)
        {
            base.OnUnload(e);
            GL.DeleteBuffers(2, mSquareVertexBufferObjectIDArray);
            GL.DeleteBuffers(2, mVertexBufferObjectIDArray);
            GL.UseProgram(0);
            mShader.Delete();
        }
    }
}
