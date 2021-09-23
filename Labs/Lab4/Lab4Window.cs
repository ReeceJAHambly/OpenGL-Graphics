using Labs.Utility;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Timers;

namespace Labs.Lab4
{
    public class Lab4Window : GameWindow
    {
        //L4T1 Changes primitive type from triangles to lines 
        //L4T2 Changes primitive tyoe from lines to triangles 
        //L4T3 Loaded an image from the disk into memory
        //L4T4 Loaded texture from memory onto the graphics card
        //L4T5 Added texture coordintes to vertices - mesh is broken
        //L4T6 Fixed mesh
        //L4T7 Can visualize texture coordinates
        //L4T8 Flipped the image before loading onto the graphics card
        //L4T9 Loaded a second texture
        //L4T10 Used second texture to discard some fragments creating a dissolve effect
        public Lab4Window()
            : base(
                800, // Width
                600, // Height
                GraphicsMode.Default,
                "Lab 4 Textures",
                GameWindowFlags.Default,
                DisplayDevice.Default,
                3, // major
                3, // minor
                GraphicsContextFlags.ForwardCompatible
                )
        {
        }

        private int[] mVBO_IDs = new int[2];
        private int mVAO_ID;
        private ShaderUtility mShader;
        private int mTexture_ID;
        private int mDisTexture_ID;
        private Timer mTimer;
        protected override void OnLoad(EventArgs e)
        {
            string filepath = @"G:\3D Graphics\Startup Code 3D Graphics\Labs\Lab4\marbleTexture.jpg";
            string filepathDissolve = @"G:\3D Graphics\Startup Code 3D Graphics\Labs\Lab4\dissolveTexture.jpg";
            if (System.IO.File.Exists(filepath))
            {
                Bitmap TextureBitmap = new Bitmap(filepath);
                TextureBitmap.RotateFlip(RotateFlipType.RotateNoneFlipY);
                BitmapData TextureData = TextureBitmap.LockBits(
                    new System.Drawing.Rectangle(0, 0, TextureBitmap.Width,
                    TextureBitmap.Height), ImageLockMode.ReadOnly,
                    System.Drawing.Imaging.PixelFormat.Format32bppRgb);


                GL.ActiveTexture(TextureUnit.Texture0);
                GL.GenTextures(1, out mTexture_ID);
                GL.BindTexture(TextureTarget.Texture2D, mTexture_ID);

                GL.TexImage2D(TextureTarget.Texture2D,
                0, PixelInternalFormat.Rgba, TextureData.Width, TextureData.Height,
                0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra,
                PixelType.UnsignedByte, TextureData.Scan0);

                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter,
                    (int)TextureMinFilter.Linear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter,
                    (int)TextureMagFilter.Linear);

                TextureBitmap.UnlockBits(TextureData);

               
            }
            else
            {
                throw new Exception("Could not find file " + filepath);
            }
            if(System.IO.File.Exists(filepathDissolve))
            {
                Bitmap TextureBitmapDissolve = new Bitmap(filepathDissolve);
                TextureBitmapDissolve.RotateFlip(RotateFlipType.RotateNoneFlipY);
                BitmapData TextureDataDissolve = TextureBitmapDissolve.LockBits(
                    new System.Drawing.Rectangle(0, 0, TextureBitmapDissolve.Width,
                    TextureBitmapDissolve.Height), ImageLockMode.ReadOnly,
                    System.Drawing.Imaging.PixelFormat.Format32bppRgb);

                GL.ActiveTexture(TextureUnit.Texture1);
                GL.GenTextures(2, out mDisTexture_ID);
                GL.BindTexture(TextureTarget.Texture2D, mDisTexture_ID);

                GL.TexImage2D(TextureTarget.Texture2D,
                0, PixelInternalFormat.Rgba, TextureDataDissolve.Width, TextureDataDissolve.Height,
                0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra,
                PixelType.UnsignedByte, TextureDataDissolve.Scan0);

                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter,
                    (int)TextureMinFilter.Linear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter,
                    (int)TextureMagFilter.Linear);

                TextureBitmapDissolve.UnlockBits(TextureDataDissolve);
            }
            else
            {
                throw new Exception("Could not find file " + filepathDissolve);
            }

           

            // Set some GL state
            GL.ClearColor(Color4.Firebrick);

            float[] vertices = {-0.5f, -0.5f,0.5f,
                                -0.25f, -0.5f, 0.5f,
                                0.0f, -0.5f, 0.5f,
                                0.25f, -0.5f, 0.5f,
                                0.5f, -0.5f, 0.5f,
                                -0.5f, 0.0f, 0.5f,
                                -0.25f, 0.0f, 0.1f,
                                0.0f, 0.0f, 0.2f,
                                0.25f, 0.0f, 0.5f,
                                0.5f, 0.0f, 0.9f,
                               -0.5f, 0.5f, 0.6f,
                                -0.25f, 0.5f,0.2f,
                                0.0f, 0.5f, 0.27f,
                                0.25f, 0.5f, 0.1f,
                                0.5f, 0.5f, 0.78f
                                };
        
            uint[] indices = { 5, 0, 1,
                               5, 1, 6,
                               6, 1, 2,
                               6, 2, 7,
                               7, 2, 3,
                               7, 3, 8,
                               8, 3, 4,
                               8, 4, 9,
                               10, 5, 6,
                               10, 6, 11,
                               11, 6, 7,
                               11, 7, 12,
                               12, 7, 8,
                               12, 8, 13,
                               13, 8, 9,
                               13, 9, 14
                             };

            GL.Enable(EnableCap.CullFace);

            mShader = new ShaderUtility(@"Lab4/Shaders/vTexture.vert", @"Lab4/Shaders/fTexture.frag");
            GL.UseProgram(mShader.ShaderProgramID);
            int vPositionLocation = GL.GetAttribLocation(mShader.ShaderProgramID, "vPosition");
            int vColourLocation = GL.GetAttribLocation(mShader.ShaderProgramID, "vTexCoords");

            mVAO_ID = GL.GenVertexArray();
            GL.GenBuffers(mVBO_IDs.Length, mVBO_IDs);

            GL.BindVertexArray(mVAO_ID);
            GL.BindBuffer(BufferTarget.ArrayBuffer, mVBO_IDs[0]);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(vertices.Length * sizeof(float)), vertices, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, mVBO_IDs[1]);
            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(indices.Length * sizeof(uint)), indices, BufferUsageHint.StaticDraw);

            int size;
            GL.GetBufferParameter(BufferTarget.ArrayBuffer, BufferParameterName.BufferSize, out size);
            if (vertices.Length * sizeof(float) != size)
            {
                throw new ApplicationException("Vertex data not loaded onto graphics card correctly");
            }

            GL.GetBufferParameter(BufferTarget.ElementArrayBuffer, BufferParameterName.BufferSize, out size);
            if (indices.Length * sizeof(uint) != size)
            {
                throw new ApplicationException("Index data not loaded onto graphics card correctly");
            }

            GL.EnableVertexAttribArray(vPositionLocation);
            GL.VertexAttribPointer(vPositionLocation, 2, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);

            GL.EnableVertexAttribArray(vColourLocation);
            GL.VertexAttribPointer(vColourLocation, 2, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);

            int uTextureSamplerLocation = GL.GetUniformLocation(mShader.ShaderProgramID,"uTextureSampler2");
            GL.Uniform1(uTextureSamplerLocation, 1);


            mTimer = new Timer();
            mTimer.AutoReset = true;
            mTimer.Interval = 2000;
           
            //mTimer.Elapsed += OnUpdateFrame();
            mTimer.Start();

            GL.BindVertexArray(0);

            base.OnLoad(e);

        }
        protected static  void OnUpdateFrame (Object source, ElapsedEventArgs e)
        {
            float mThreshold = 0.5f;
           
           
            float mRateOfDissolve = 0.01f;
            float timestep; //= mTimer.g;
            float thresholdChange = mRateOfDissolve; // timestep;
            if (mThreshold + thresholdChange < 0 || mThreshold + thresholdChange > 1)
            {
                mRateOfDissolve = -mRateOfDissolve;
            }
            mThreshold += mRateOfDissolve; // timestep;
            //int uThresholdLocation = GL.GetUniformLocation(mShader.ShaderProgramID, "uThreshold");
            //GL.Uniform1(uThresholdLocation, mThreshold);
            //mTimer.AutoReset = true;
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.BindVertexArray(mVAO_ID);
            GL.DrawElements(PrimitiveType.Triangles, 48, DrawElementsType.UnsignedInt, 0);

            GL.BindVertexArray(0);
            this.SwapBuffers();
        }

        protected override void OnUnload(EventArgs e)
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            GL.BindVertexArray(0);
            GL.DeleteBuffers(mVBO_IDs.Length, mVBO_IDs);
            GL.DeleteVertexArray(mVAO_ID);
            mShader.Delete();
            base.OnUnload(e);
        }
    }
}
