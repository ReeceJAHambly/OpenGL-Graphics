using Labs.Utility;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;

namespace Labs.ACW
{
    public class CubePrim : PrimShape
    {
        public CubePrim(Matrix4 pPosition, Vector3 pAmbientColour, Vector3 pDiffuseColour, Vector3 pSpecularColour, 
            float pShininess, int pVAO_id, List<int> pVBO_ids, float[] pVertices, int pTextureLocation, int pTextureID) : 
            base(pPosition, pAmbientColour, pDiffuseColour, pSpecularColour, pShininess, pVAO_id, pVBO_ids, pVertices, pTextureLocation, pTextureID)
        {

        }

        public override void Draw()
        {
            if(isTextured)
            {
                GL.ActiveTexture(TextureUnit.Texture0);
                GL.BindTexture(TextureTarget.Texture2D, this.mTextureID);
            }
            GL.DrawElements(PrimitiveType.Triangles, getIndices().Length, DrawElementsType.UnsignedInt, 0);
        }

        public override void GenerateBufferObjects(int vPositionLocation, int vNormalLocation)
        {
            //Binding vertices
            GL.BindVertexArray(getVAOid());
            GL.BindBuffer(BufferTarget.ArrayBuffer, getVBOids()[0]);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(getVertices().Length * sizeof(float)), getVertices(), BufferUsageHint.StaticDraw);

            int size;
            GL.GetBufferParameter(BufferTarget.ArrayBuffer, BufferParameterName.BufferSize, out size);
            if (getVertices().Length * sizeof(float) != size)
            {
                throw new ApplicationException("Vertex data not loaded onto graphics card correctly");
            }

            //binding indices
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, getVBOids()[1]);
            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(getIndices().Length * sizeof(float)), getIndices(), BufferUsageHint.StaticDraw);

            GL.GetBufferParameter(BufferTarget.ElementArrayBuffer, BufferParameterName.BufferSize, out size);


            GL.GetBufferParameter(BufferTarget.ElementArrayBuffer, BufferParameterName.BufferSize, out size);
            if (getIndices().Length * sizeof(float) != size)
            {
                throw new ApplicationException("Index data not loaded onto graphics card correctly");
            }

            GL.EnableVertexAttribArray(vPositionLocation);
            GL.VertexAttribPointer(vPositionLocation, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);

            GL.EnableVertexAttribArray(vNormalLocation);
            GL.VertexAttribPointer(vNormalLocation, 3, VertexAttribPointerType.Float, true, 6 * sizeof(float), 3 * sizeof(float));

            if(isTextured)
            {
                //textureCoords
                GL.EnableVertexAttribArray(mTextureCoordsLocation);
                GL.VertexAttribPointer(mTextureCoordsLocation, 2, VertexAttribPointerType.Float, false, 8 * sizeof(float), 6 * sizeof(float));
            }  
        }
    }
}
