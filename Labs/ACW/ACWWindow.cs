using Labs.Utility;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Input;

namespace Labs.ACW
{
    public class ACWWindow : GameWindow
    {
        public ACWWindow()
            : base(
                800, // Width
                600, // Height
                GraphicsMode.Default,
                "Assessed Coursework",
                GameWindowFlags.Default,
                DisplayDevice.Default,
                3, // major
                3, // minor
                GraphicsContextFlags.ForwardCompatible
                )
        {
        }

        public List<StationaryCamera> mCameras = new List<StationaryCamera>();//stores the current cameras in the program
        public StationaryCamera mCurrentCamera; //creates the current camera object
        public List<Model> mModels = new List<Model>(); // List of models that will be in the scene
        public List<Lighting> mLights = new List<Lighting>();

        public List<int> mTextureIDs = new List<int>();

        private string currentShader;
        private int currentShaderID = 0;
        //Points to the next vao/vbo to be assigned
        private int currentVAO = 0;
        private int currentVBO = 0;
        private int[] mVBO_IDs = new int[30];
        private int[] mVAO_IDs = new int[20];
        private ShaderUtility mShader;
        private Dictionary<string, ShaderUtility> mShaders = new Dictionary<string, ShaderUtility>();
        private Matrix4 mCurrentView1; 
        private Matrix4 mGroundPosition;
        private Vector4 mCurrentEyePosition;

        protected void ChangeCamera(int cameraID)
        {
            //Changes the position of the camera
            mCurrentCamera = mCameras[cameraID];
            mCurrentView1 = mCurrentCamera.getPosition();

            int uView = GL.GetUniformLocation(mShaders[currentShader].ShaderProgramID, "uView");
            GL.UniformMatrix4(uView, true, ref mCurrentView1);

            ChangeLightPositions();
        }
        protected void Floor(int vPositionLocation, int vNormalLocation)
        {
            //The floor is generated with a texture (grass)
            currentShader = "texture";
            float[] vertices = new float[] {-10, 0, -10,0,1,0, 0,0,
                                             -10, 0, 10,0,1,0, 1,0,
                                             10, 0, 10,0,1,0, 1,1,
                                             10, 0, -10,0,1,0, 0,1};

            vPositionLocation = GL.GetAttribLocation(mShaders[currentShader].ShaderProgramID, "vPosition");
            vNormalLocation = GL.GetAttribLocation(mShaders[currentShader].ShaderProgramID, "vNormal");

            GL.UseProgram(mShaders[currentShader].ShaderProgramID);
            int vTexCoordsLocation = GL.GetAttribLocation(mShaders[currentShader].ShaderProgramID, "vTexCoords");

            Matrix4 groundPosition = Matrix4.CreateTranslation(0, 0, -5f);
            Vector3 ambientColour = new Vector3(0.05f, 0.05f, 0.05f);
            Vector3 diffuseColour = new Vector3(0.5f, 0.5f, 0.5f);
            Vector3 specularColour = new Vector3(0.7f, 0.7f, 0.7f);
            float shininess = 0.078125f;

            int vaoID = mVAO_IDs[currentVAO];//setting its vao id
            List<int> VBOids = new List<int>();
            VBOids.Add(mVBO_IDs[currentVBO]); //setting its vbo id

            PrimShape ground = new PrimShape(groundPosition, ambientColour, diffuseColour, specularColour, shininess, vaoID, VBOids, vertices,
                vTexCoordsLocation, mTextureIDs[currentShaderID]);
            ground.GenerateBufferObjects(vPositionLocation, vNormalLocation);

            //Adds the ground model to the mModels list
            mModels.Add(ground);

            mGroundPosition = ground.getPosition();

            currentVAO++;//Changing the pointers to point to the next value
            currentVBO++;
            currentShaderID++;

            currentShader = "default";
        }

        

        protected void GenerateExistingModel(int vPositionLocation, int vNormalLocation, Matrix4 modelPosition, string material, string modelType)
        {
            currentShader = "default";
            GL.UseProgram(mShaders[currentShader].ShaderProgramID);
            ModelUtility existingModelUtility;
            switch (modelType.ToLower())
            {
                case "armadillo":
                    {
                        existingModelUtility = ModelUtility.LoadModel(@"Utility/Models/model.bin");
                        break;
                    }
                default: //defaults to a cylinder
                    {
                        existingModelUtility = ModelUtility.LoadModel(@"Utility/Models/cylinder.bin");
                        break;
                    }
            }

            //Setting a default colour
            Vector3 ambientColour = new Vector3(0.05f, 0.05f, 0.05f);
            Vector3 diffuseColour = new Vector3(0.5f, 0.5f, 0.5f);
            Vector3 specularColour = new Vector3(0.7f, 0.7f, 0.7f);
            float shininess = 0.07f;

            switch (material.ToLower())
            {
                case "green":
                    {
                        ambientColour = new Vector3(0.199f, 0.36f, 0.18f);
                        diffuseColour = new Vector3(0.231f, 0.65f, 0.22f);
                        specularColour = new Vector3(0.189f, 0.1f, 0.1f);
                        shininess = 0.01f;
                        break;
                    }
                case "grey":
                    {
                        ambientColour = new Vector3(0.75f, 0.75f, 0.75f);
                        diffuseColour = new Vector3(0.6f, 0.6f, 0.6f);
                        specularColour = new Vector3(0.4f, 0.3f, 0.2f);
                        shininess = 0.1f;
                        break;
                    }
                case "black":
                    {
                        ambientColour = new Vector3(0.01f, 0.01f, 0.5f);
                        diffuseColour = new Vector3(0.005f, 0.005f, 0.4f);
                        specularColour = new Vector3(0.001f, 0.001f, 0.03f);
                        shininess = 0.2f;
                        break;
                    }
                default://Doesn't change the material from the default values
                    { 
                        break;
                    }
            }

            int vaoID = mVAO_IDs[currentVAO];//Set the VAO IDs  

            List<int> VBOids = new List<int>(); //2 BVOS are needed as the existing models already have indices
            VBOids.Add(mVBO_IDs[currentVBO]);
            VBOids.Add(mVBO_IDs[++currentVBO]);

            PreExistingModel newModel = new PreExistingModel(modelPosition, ambientColour, diffuseColour, specularColour, shininess, vaoID, VBOids, existingModelUtility);
            newModel.GenerateBufferObjects(vPositionLocation, vNormalLocation);
            newModel.RotateY((float)(-Math.PI/2));
            mModels.Add(newModel); //Adds the new model to the models list
            currentVAO++;
            currentVBO++;
        }

        protected void CubeGenerate(int vPositionLocation, int vNormalLocation, Matrix4 position, string material)
        {
            //Used similar code from past lab for a generate cube function 
            currentShader = "default";
            Vector3 cubePosition = new Vector3(0, 0, 0);
            cubePosition = Vector3.Transform(cubePosition, position);
            GL.UseProgram(mShaders[currentShader].ShaderProgramID);
            //Getting the texture coords 
            int vTexCoordsLocation = GL.GetAttribLocation(mShaders[currentShader].ShaderProgramID, "vTexCoords");
            Vector3 ambientColour = new Vector3(0.02f, 0.02f, 0.02f);
            Vector3 diffuseColour = new Vector3(0.3f, 0.3f, 0.3f);
            Vector3 specularColour = new Vector3(0.6f, 0.6f, 0.6f);
            float shininess = 0.078125f;

            //Cube vertices
            float[] vertices = {
                -0.8f + cubePosition.X, -0.8f + cubePosition.Y,  -0.8f + cubePosition.Z, 0, 1, 0,
                0.8f + cubePosition.X, -0.8f + cubePosition.Y,  -0.8f + cubePosition.Z, 0, 1, 0,
                0.8f+ cubePosition.X, 0.8f + cubePosition.Y,  -0.8f + cubePosition.Z, 0, 1, 0,
                -0.8f + cubePosition.X, 0.8f + cubePosition.Y,  -0.8f + cubePosition.Z, 0, 1, 0,
                -0.8f + cubePosition.X, -0.8f + cubePosition.Y,  0.8f + cubePosition.Z, 0, 1, 0,
                0.8f + cubePosition.X, -0.8f + cubePosition.Y,  0.8f + cubePosition.Z, 0, 1, 0,
                0.8f + cubePosition.X, 0.8f + cubePosition.Y,  0.8f + cubePosition.Z, 0, 1, 0,
                -0.8f + cubePosition.X, 0.8f + cubePosition.Y,  0.8f + cubePosition.Z, 0, 1, 0

            };

            int[] cubeIndicies = { 0, 7, 3, 0, 4, 7, 1, 2, 6, 6, 5, 1, 0, 2, 1, 0, 3, 2,
                4, 5, 6, 6, 7, 4,
                2, 3, 6, 6, 3, 7,
                0, 1, 5, 0, 5, 4
            };

            //sets the lighting properties of the cube for colour
            switch (material.ToLower())
            {
                case "yellow":
                    {
                        ambientColour = new Vector3(0.247255f, 0.1995f, 0.0745f);
                        diffuseColour = new Vector3(0.75164f, 0.60648f, 0.22648f);
                        specularColour = new Vector3(0.628281f, 0.555802f, 0.366065f);
                        shininess = 0.4f;
                        break;
                    }
                case "grey":
                    {
                        ambientColour = new Vector3(0.19225f, 0.19225f, 0.19225f);
                        diffuseColour = new Vector3(0.50754f, 0.50754f, 0.50754f);
                        specularColour = new Vector3(0.508273f, 0.508273f, 0.508273f);
                        shininess = 0.4f;
                        break;
                    }
                default://Keeps default material
                    {
                        break;
                    }
            }

            int vaoID = mVAO_IDs[currentVAO];//setting its vao id
            List<int> VBOids = new List<int>();
            VBOids.Add(mVBO_IDs[currentVBO]); //setting its vbo id
            currentVBO++;
            VBOids.Add(mVBO_IDs[currentVBO]); //setting its vbo id

            CubePrim newCube = new CubePrim(position, ambientColour, diffuseColour, specularColour, shininess, vaoID, VBOids, vertices, vTexCoordsLocation, mTextureIDs[0]);
            newCube.setIndices(cubeIndicies);
            newCube.isTextured = false;
            newCube.GenerateBufferObjects(vPositionLocation, vNormalLocation);
            mModels.Add(newCube);
            //Changing the pointers to point to the next value
            currentVAO++;
            currentVBO++;

            currentShader = "default";
        }

        //Sets light with position and colour
        protected void LightGenerate(Vector4 lightPosition, Vector3 colour, int lightID)
        {
            
            Lighting newLight = new Lighting(lightPosition, colour, lightID);
            mLights.Add(newLight);

            foreach (string shader in mShaders.Keys)
            {
                GL.UseProgram(mShaders[shader].ShaderProgramID);

                int uLightPositionLocation = GL.GetUniformLocation(mShaders[shader].ShaderProgramID, "uLight[" + newLight.getLightingID() + "].Position");
                lightPosition = Vector4.Transform(lightPosition, mCurrentView1);
                GL.Uniform4(uLightPositionLocation, newLight.getPosition());

                //Setting the light colour
                int uAmbientLightLocation = GL.GetUniformLocation(mShaders[shader].ShaderProgramID, "uLight[" + newLight.getLightingID() + "].AmbientLight");
                GL.Uniform3(uAmbientLightLocation, newLight.GetColour());

                int uDiffuseLightLocation = GL.GetUniformLocation(mShaders[shader].ShaderProgramID, "uLight[" + newLight.getLightingID() + "].DiffuseLight");
                GL.Uniform3(uDiffuseLightLocation, newLight.GetColour());

                int uSpecularLightLocation = GL.GetUniformLocation(mShaders[shader].ShaderProgramID, "uLight[" + newLight.getLightingID() + "].SpecularLight");
                GL.Uniform3(uSpecularLightLocation, newLight.GetColour());
            }
        }

        protected void GenerateDirectionalLight(Vector4 lightPosition, Vector3 colour, int lightID)
        {
            int lightType = 2;//directional lights are type 2
            Lighting newLight = new Lighting(lightPosition, colour, lightID);
            //Setting the direction
            mLights.Add(newLight);

            foreach (string shader in mShaders.Keys)
            {
                GL.UseProgram(mShaders[shader].ShaderProgramID);

                int uLightPositionLocation = GL.GetUniformLocation(mShaders[shader].ShaderProgramID, "uLight[" + newLight.getLightingID() + "].Position");
                lightPosition = Vector4.Transform(lightPosition, mCurrentView1);
                GL.Uniform4(uLightPositionLocation, newLight.getPosition());

                //Setting the light colour
                int uAmbientLightLocation = GL.GetUniformLocation(mShaders[shader].ShaderProgramID, "uLight[" + newLight.getLightingID() + "].AmbientLight");
                GL.Uniform3(uAmbientLightLocation, newLight.GetColour());

                int uDiffuseLightLocation = GL.GetUniformLocation(mShaders[shader].ShaderProgramID, "uLight[" + newLight.getLightingID() + "].DiffuseLight");
                GL.Uniform3(uDiffuseLightLocation, newLight.GetColour());

                int uSpecularLightLocation = GL.GetUniformLocation(mShaders[shader].ShaderProgramID, "uLight[" + newLight.getLightingID() + "].SpecularLight");
                GL.Uniform3(uSpecularLightLocation, newLight.GetColour());

                //sets the type of light
                int uLightTypeLocation = GL.GetUniformLocation(mShaders[shader].ShaderProgramID, "uLight[" + newLight.getLightingID() + "].LightType");
                GL.Uniform1(uLightTypeLocation, lightType);
            }
        }

        protected void CreateStartLightPositions()
        {
            for (int i = 0; i < mLights.Count; i++)
            {
                foreach(string shader in mShaders.Keys) //Loops through the shaders
                {
                    GL.UseProgram(mShaders[shader].ShaderProgramID);
                    int uLightPositionLocation = GL.GetUniformLocation(mShaders[shader].ShaderProgramID, "uLight[" + mLights[i].getLightingID() + "].Position");
                    Vector4 ulightPosition = mLights[i].getPosition();
                    ulightPosition = Vector4.Transform(ulightPosition, mCurrentView1);
                    GL.Uniform4(uLightPositionLocation, ulightPosition);
                }
            }
            currentShader = "default";//sets back the shader to default
            GL.UseProgram(mShaders[currentShader].ShaderProgramID);
        }

        protected void ChangeLightPositions()
        {
            //Changes the light position after the initial start of the program
            for(int i = 0; i < mLights.Count; i++)
            {

                if(mLights[i].isCone)
                {
                    foreach(string shader in mShaders.Keys)
                    {
                        GL.UseProgram(mShaders[shader].ShaderProgramID);
                        int LightPositionLocation = GL.GetUniformLocation(mShaders[shader].ShaderProgramID, "uLight[" + mLights[i].getLightingID() + "].Position");
                        Vector4 lightPosition = mLights[i].getPosition();
                        lightPosition = Vector4.Transform(lightPosition, mCurrentView1);
                        GL.Uniform4(LightPositionLocation, lightPosition);
                    }
                    
                }
                else
                {

                    foreach (string shader in mShaders.Keys)
                    {
                        GL.UseProgram(mShaders[shader].ShaderProgramID);
                        int LightPositionLocation = GL.GetUniformLocation(mShaders[shader].ShaderProgramID, "uLight[" + mLights[i].getLightingID() + "].Position");
                        Vector4 lightPosition = mLights[i].getPosition();
                        lightPosition = Vector4.Transform(lightPosition, mCurrentView1);
                        GL.Uniform4(LightPositionLocation, lightPosition);
                    }
                }
            }
            
        }

        //Changes the light to off or on
        protected void ChangeLightStatus(int lightID)
        {
            if(mLights[lightID].isON)
            {
                mLights[lightID].isON = false;
                //Setting the light colour
                Vector3 colour = new Vector3(0, 0, 0);

                foreach (string shader in mShaders.Keys)//loop through each shader
                {
                    GL.UseProgram(mShaders[shader].ShaderProgramID);
                    int uAmbientLightLocation = GL.GetUniformLocation(mShaders[shader].ShaderProgramID, "uLight[" + mLights[lightID].getLightingID() + "].AmbientLight");
                    GL.Uniform3(uAmbientLightLocation, colour);

                    int uDiffuseLightLocation = GL.GetUniformLocation(mShaders[shader].ShaderProgramID, "uLight[" + mLights[lightID].getLightingID() + "].DiffuseLight");
                    GL.Uniform3(uDiffuseLightLocation, colour);

                    int uSpecularLightLocation = GL.GetUniformLocation(mShaders[shader].ShaderProgramID, "uLight[" + mLights[lightID].getLightingID() + "].SpecularLight");
                    GL.Uniform3(uSpecularLightLocation, colour);
                }
            }
            else
            {
                mLights[lightID].isON = true;

                foreach (string shader in mShaders.Keys)
                {
                    //Setting the light colour
                    GL.UseProgram(mShaders[shader].ShaderProgramID);
                    int uAmbientLightLocation = GL.GetUniformLocation(mShaders[shader].ShaderProgramID, "uLight[" + mLights[lightID].getLightingID() + "].AmbientLight");
                    GL.Uniform3(uAmbientLightLocation, mLights[lightID].GetColour());

                    int uDiffuseLightLocation = GL.GetUniformLocation(mShaders[shader].ShaderProgramID, "uLight[" + mLights[lightID].getLightingID() + "].DiffuseLight");
                    GL.Uniform3(uDiffuseLightLocation, mLights[lightID].GetColour());

                    int uSpecularLightLocation = GL.GetUniformLocation(mShaders[shader].ShaderProgramID, "uLight[" + mLights[lightID].getLightingID() + "].SpecularLight");
                    GL.Uniform3(uSpecularLightLocation, mLights[lightID].GetColour());
                }   
            }
        }

        // Both functions load a texture, one with a file path and the other with a bitmage image
        protected int LoadImage(string filename)
        {
            try
            {
                Bitmap file = new Bitmap(filename);
                return LoadImageTexture(file);
            }
            catch (FileNotFoundException e)
            {
                return -1;
            }
        }      
        protected int LoadImageTexture(Bitmap image)
        {
            int texID;
            image.RotateFlip(RotateFlipType.RotateNoneFlipY);

            BitmapData TextureData = image.LockBits(
            new System.Drawing.Rectangle(0, 0, image.Width,
            image.Height), ImageLockMode.ReadOnly,
            System.Drawing.Imaging.PixelFormat.Format32bppRgb);

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.GenTextures(1, out texID);
            GL.BindTexture(TextureTarget.Texture2D, texID);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, TextureData.Width, TextureData.Height, 0,
                OpenTK.Graphics.OpenGL.PixelFormat.Bgra,PixelType.UnsignedByte, TextureData.Scan0);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter,
            (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter,
            (int)TextureMagFilter.Linear);
            image.UnlockBits(TextureData);

            return texID;
        }

        protected override void OnLoad(EventArgs e)
        {
            // Set the state of the frame
            GL.ClearColor(Color4.DarkRed); // Sets background to dark red
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.CullFace);

            //Loading in the shaders
            mShader = new ShaderUtility(@"ACW\Shaders\vPassThrough.vert", @"ACW\Shaders\fLighting.frag");
            mShaders.Add("default", mShader);
            ShaderUtility newShader = new ShaderUtility(@"ACW\Shaders\vTexture.vert", @"ACW\Shaders\fTexture.frag");
            mShaders.Add("texture", newShader);
            currentShader = "default";
            //Starts the current shader
            GL.UseProgram(mShaders[currentShader].ShaderProgramID);

            //Loads in marble floor texture, was going to have more textures but didnt implement
            string filepath = @"ACW\grass.jpg";
            mTextureIDs.Add(LoadImage(filepath));
          

            //Camera
            #region
            MoveCamera camera1 = new MoveCamera(Matrix4.CreateTranslation(0, -3.0f, 0));
            StationaryCamera camera2 = new StationaryCamera(Matrix4.CreateTranslation(-10, -3.0f, 0));
            mCameras.Add(camera1);
            camera2.RotateY(-0.9f);
            mCameras.Add(camera2); // Stationary camera at different position to moving camera
            ChangeCamera(0);//Setting moving camera as initial camera
            #endregion

            //Getting the position and normal from the shaders
            int vPositionLocation = GL.GetAttribLocation(mShaders[currentShader].ShaderProgramID, "vPosition");
            int vNormalLocation = GL.GetAttribLocation(mShaders[currentShader].ShaderProgramID, "vNormal");

            //Generating the arrays
            GL.GenVertexArrays(mVAO_IDs.Length, mVAO_IDs);
            GL.GenBuffers(mVBO_IDs.Length, mVBO_IDs);

            //Floor with marble texture
            Floor(vPositionLocation, vNormalLocation);

            //wall
            float[] vertices = new float[] {-10, 10, -5,0,1,0, 0,0,
                                             -10, 0, -5,0,1,0, 1,0,
                                             10, 0, -5,0,1,0, 1,1,
                                             10, 10, -5,0,1,0, 0,1};

            //Adds the cubes to the onLoad function
            Matrix4 cubePosition = Matrix4.CreateTranslation(0, 1, -4);
            CubeGenerate(vPositionLocation, vNormalLocation, cubePosition, "grey");

            cubePosition = Matrix4.CreateTranslation(0, 2, -4);
            CubeGenerate(vPositionLocation, vNormalLocation, cubePosition, "yellow");

            //Two armadillos are added to the scene
            Matrix4 armadilloPos = Matrix4.CreateTranslation(0, 3, -5f);
            GenerateExistingModel(vPositionLocation, vNormalLocation, armadilloPos, "green", "Armadillo");

            armadilloPos = Matrix4.CreateTranslation(5, 1.5f, -5f);
            GenerateExistingModel(vPositionLocation, vNormalLocation, armadilloPos, "grey", "Armadillo");

            //Adds a pedestal to the scene 
            Matrix4 cylinderPos = Matrix4.CreateTranslation(0, 1, -5f);
            GenerateExistingModel(vPositionLocation, vNormalLocation, cylinderPos, "black", "cylinder");      

            GL.BindVertexArray(0);

            //Deals with lighting in scene
            for (int i = 0; i < 3; i++)
            {
                Vector4 lightPosition = new Vector4(0, 3, -6, 1);
                Vector3 colour = new Vector3(0.8f, 0.8f, 0.8f); //white
                switch (i)
                {
                    case 0:
                        {
                            lightPosition = new Vector4(-4, 4, -4, 1);
                            colour = new Vector3(1.0f, 1.0f, 1.0f); //White
                            LightGenerate(lightPosition, colour, i);
                            break;
                        }
                    case 1:
                        {
                            lightPosition = new Vector4(0, 400, 0, 1);
                            colour = new Vector3(0.5f, 0.5f, 0.5f); //Blue
                            GenerateDirectionalLight(lightPosition, colour, i);
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }

            }

            CreateStartLightPositions();//Sets the initial positions according to the initial view
            base.OnLoad(e);
        }
        
        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            base.OnKeyPress(e);
            if(mCurrentCamera.canMove())
            {
                if (e.KeyChar == 'w')//moves forward
                {
                    mCurrentCamera.moveForward();
                    mCurrentView1 = mCurrentCamera.getPosition();

                    ChangeLightPositions();

                    mCurrentEyePosition = Vector4.Transform(mCurrentEyePosition, mCurrentView1);
                    int uEyePositionLocation = GL.GetUniformLocation(mShaders[currentShader].ShaderProgramID, "uEyePosition");
                    GL.Uniform4(uEyePositionLocation, ref mCurrentEyePosition);

                    int uView = GL.GetUniformLocation(mShaders[currentShader].ShaderProgramID, "uView");
                    GL.UniformMatrix4(uView, true, ref mCurrentView1);
                }
                if (e.KeyChar == 'a')//When a is pressed, the camera turns left
                {
                    mCurrentCamera.turnLeft();
                    mCurrentView1 = mCurrentCamera.getPosition();

                    ChangeLightPositions();

                    int uView = GL.GetUniformLocation(mShaders[currentShader].ShaderProgramID, "uView");
                    GL.UniformMatrix4(uView, true, ref mCurrentView1);
                }
                if (e.KeyChar == 's')//When as is pressed, the camera moves back
                {
                    mCurrentCamera.moveBack();
                    mCurrentView1 = mCurrentCamera.getPosition();

                    ChangeLightPositions();

                    mCurrentEyePosition = Vector4.Transform(mCurrentEyePosition, mCurrentView1);
                    int uEyePositionLocation = GL.GetUniformLocation(mShaders[currentShader].ShaderProgramID, "uEyePosition");
                    GL.Uniform4(uEyePositionLocation, ref mCurrentEyePosition);

                    int uView = GL.GetUniformLocation(mShaders[currentShader].ShaderProgramID, "uView");
                    GL.UniformMatrix4(uView, true, ref mCurrentView1);
                }
                if (e.KeyChar == 'd') ///When d is pressed, the camera turns right
                {
                    mCurrentCamera.turnRight();
                    mCurrentView1 = mCurrentCamera.getPosition();

                    ChangeLightPositions();

                    int uView = GL.GetUniformLocation(mShaders[currentShader].ShaderProgramID, "uView");
                    GL.UniformMatrix4(uView, true, ref mCurrentView1);
                } 
                if(e.KeyChar == 'q') // When q is pressed, the camera moves up
                {
                    mCurrentCamera.goUp();
                    mCurrentView1 = mCurrentCamera.getPosition();

                    ChangeLightPositions();

                    int uView = GL.GetUniformLocation(mShaders[currentShader].ShaderProgramID, "uView");
                    GL.UniformMatrix4(uView, true, ref mCurrentView1);
                }
                if (e.KeyChar == 'e') // When e is pressed, the camera moves down
                {
                    mCurrentCamera.goDown();
                    mCurrentView1 = mCurrentCamera.getPosition();

                    ChangeLightPositions();

                    int uView = GL.GetUniformLocation(mShaders[currentShader].ShaderProgramID, "uView");
                    GL.UniformMatrix4(uView, true, ref mCurrentView1);
                }
            }


            #region CameraSwitch
            //Was going to make more than one camera but was not sure how to implement
            if (e.KeyChar == 'y')//moving camera
            {
                ChangeCamera(0);
            }

            //Changes to a fixed camera above the models facing down
            if (e.KeyChar == 't')
            {
                ChangeCamera(1);
            }
            #endregion

            #region lightsOn/Off
            if (e.KeyChar == 'x')//turns the directional light on and off
            {
                ChangeLightStatus(1);
            }

            if (e.KeyChar == 'z') //turns the specular light on and off
            {
                ChangeLightStatus(0);
            }
            #endregion

        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(this.ClientRectangle);
            if (mShaders[currentShader] != null)
            {
                int uProjectionLocation = GL.GetUniformLocation(mShaders[currentShader].ShaderProgramID, "uProjection");
                Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(1, (float)ClientRectangle.Width / ClientRectangle.Height, 0.5f, 25);
                GL.UniformMatrix4(uProjectionLocation, true, ref projection);
            }
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
 	        base.OnUpdateFrame(e);
            for(int i = 0; i < mModels.Count; i++)
            {
                //Get the models list and applies a type of movmement to them
                if(mModels[i].getIndices() != null)
                {
                    //Different indices have different models with different types of movement applied to them
                    if(i == 1 || i == 2)
                    {
                        mModels[i].RotateY(-0.01f);
                    }
                    else if (i == 3)
                    {
                        mModels[i].RotateY(0.01f);
                    }
                    else if (i == 4 || i == 6)
                    {
                        mModels[i].RotateY(-0.025f);
                    }
                }
            }
            int uView = GL.GetUniformLocation(mShaders[currentShader].ShaderProgramID, "uView");
            GL.UniformMatrix4(uView, true, ref mCurrentView1);
        }

       

        protected void DrawAllModels()
        {
            for(int i = 0; i< mModels.Count; i++)
            {
                int uModel;
                if (mModels[i].isTextured)
                {
                    GL.UseProgram(mShaders["texture"].ShaderProgramID);
                    currentShader = "texture";
                    uModel = GL.GetUniformLocation(mShaders[currentShader].ShaderProgramID, "uModel");

                    int uView = GL.GetUniformLocation(mShaders[currentShader].ShaderProgramID, "uView");
                    GL.UniformMatrix4(uView, true, ref mCurrentView1);

                    int uProjection = GL.GetUniformLocation(mShaders[currentShader].ShaderProgramID, "uProjection");
                    Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(1, (float)ClientRectangle.Width / ClientRectangle.Height, 0.5f, 25);
                    GL.UniformMatrix4(uProjection, true, ref projection);

                    ChangeLightPositions(); //used to make sure light positions stay the same throughout the draw
                }
                else
                {
                    GL.UseProgram(mShaders["default"].ShaderProgramID);
                    currentShader = "default";
                    uModel = GL.GetUniformLocation(mShaders[currentShader].ShaderProgramID, "uModel"); 
                }

                //Loading up the models material properties
                int uAmbientReflectivityLocation = GL.GetUniformLocation(mShaders[currentShader].ShaderProgramID, "uMaterial.AmbientReflectivity");
                GL.Uniform3(uAmbientReflectivityLocation, mModels[i].getAmbientColour());

                int uDiffuseReflectivityLocation = GL.GetUniformLocation(mShaders[currentShader].ShaderProgramID, "uMaterial.DiffuseReflectivity");
                GL.Uniform3(uDiffuseReflectivityLocation, mModels[i].getDiffuseColour());

                int uSpecularReflectivityLocation = GL.GetUniformLocation(mShaders[currentShader].ShaderProgramID, "uMaterial.SpecularReflectivity");
                GL.Uniform3(uSpecularReflectivityLocation, mModels[i].getSpecularColour());

                int uShininessLocation = GL.GetUniformLocation(mShaders[currentShader].ShaderProgramID, "uMaterial.Shininess");
                GL.Uniform1(uShininessLocation, mModels[i].getShininess());

                if (i == 0)
                { 
                    GL.UniformMatrix4(uModel, true, ref mGroundPosition);

                    //Ground model gets done first as other models are done in relation to it
                }
                else
                {
                    //Rest of the models position is calculated
                    Matrix4 m = mModels[i].getPosition() * mGroundPosition;
                    GL.UniformMatrix4(uModel, true, ref m);
                    
                }
                GL.BindVertexArray(mModels[i].getVAOid());
                mModels[i].Draw(); //Draws the model in the list, loops through and draws the next model
                
            }
            //sets the current shader to default
            GL.UseProgram(mShaders["default"].ShaderProgramID);
            currentShader = "default";
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            DrawAllModels();
            GL.BindVertexArray(0);
            this.SwapBuffers();
        }

        protected override void OnUnload(EventArgs e)
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            GL.BindVertexArray(0);
            GL.DeleteBuffers(mVBO_IDs.Length, mVBO_IDs);
            GL.DeleteVertexArrays(mVAO_IDs.Length, mVAO_IDs);
            mTextureIDs.Clear();
            GL.DeleteTexture(0);
            GL.DeleteTexture(1);
            mShaders.Clear();
            mShader.Delete();
            base.OnUnload(e);
        }
    }
}
