using Labs.Utility;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;

namespace Labs.ACW
{
    public class PrimShape : Model
    {

        protected int mTextureCoordsLocation;
        protected int mTextureID;
        public PrimShape(Matrix4 pPosition, Vector3 pAmbientColour, Vector3 pDiffuseColour, Vector3 pSpecularColour, float pShininess, int pVAO_id, List<int> pVBO_ids,float[] pVertices, int pTextureLocation, int pTextureID) : base(pPosition, pAmbientColour, pDiffuseColour, pSpecularColour, pShininess, pVAO_id, pVBO_ids)
        {
            isTextured = true;
            setVertices(pVertices);
            SetTextureLocation(pTextureLocation);
            mTextureID = pTextureID;
        }

        public override void Draw()
        {
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, this.mTextureID);
            GL.DrawArrays(PrimitiveType.TriangleFan, 0, 4);
        }

        public override void GenerateBufferObjects(int vPositionLocation, int vNormalLocation)
        {
            GL.BindVertexArray(getVAOid());
            GL.BindBuffer(BufferTarget.ArrayBuffer, getVBOids()[0]);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(getVertices().Length * sizeof(float)), getVertices(), BufferUsageHint.StaticDraw);

            int size;
            GL.GetBufferParameter(BufferTarget.ArrayBuffer, BufferParameterName.BufferSize, out size);
            if (getVertices().Length * sizeof(float) != size)
            {
                throw new ApplicationException("Vertex data not loaded onto graphics card correctly");
            }

            //positionEnable
            GL.EnableVertexAttribArray(vPositionLocation);
            GL.VertexAttribPointer(vPositionLocation, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 0);

            //normalEnable
            GL.EnableVertexAttribArray(vNormalLocation);
            GL.VertexAttribPointer(vNormalLocation, 3, VertexAttribPointerType.Float, true, 8 * sizeof(float), 3 * sizeof(float));

            //textureCoordsEnable
            GL.EnableVertexAttribArray(mTextureCoordsLocation);
            GL.VertexAttribPointer(mTextureCoordsLocation, 2, VertexAttribPointerType.Float, false, 8 * sizeof(float), 6 * sizeof(float));
        }

        public void SetTextureLocation(int pTextureLocation)
        {
            mTextureCoordsLocation = pTextureLocation;
        }

        public override int getTextureLocation()
        {
            return mTextureCoordsLocation;
            
        }

        
    }
}
