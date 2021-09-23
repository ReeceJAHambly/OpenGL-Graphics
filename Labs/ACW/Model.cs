using Labs.Utility;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;

namespace Labs.ACW
{
    public class Model
    {
        public bool isTextured = false;
        protected int mVAO_id;
        protected float mShininess;
        protected List<int> mVBO_ids = new List<int>();//models with indices need a second vertex buffer object
        protected Matrix4 mPosition;
        public float[] mVertices;
        protected int[] mIndices;
        protected Vector3 mAmbientColour, mDiffuseColour, mSpecularColour;
        protected Vector3 mMoveAmountX = new Vector3(0.04f, 0, 0);
        protected Matrix4 mScaleAmount = Matrix4.CreateScale(0.9f);

        public Model(Matrix4 pPosition, Vector3 pAmbientColour, Vector3 pDiffuseColour, Vector3 pSpecularColour, float pShininess, int pVAO_id, List<int> pVBO_ids)
        {
            setPosition(pPosition);
            setMaterial(pAmbientColour, pDiffuseColour, pSpecularColour, pShininess);
            setVAOid(pVAO_id);
            setVBOids(pVBO_ids);
        }

        public void setPosition(Matrix4 pPosition)
        {
            mPosition = pPosition;
        }
        public Matrix4 getPosition()
        {
            return mPosition;
        }

        public void setMaterial(Vector3 pAmbientColour, Vector3 pDiffuseColour, Vector3 pSpecularColour, float pShininess)
        {
            mAmbientColour = pAmbientColour;
            mDiffuseColour = pDiffuseColour;
            mSpecularColour = pSpecularColour;
            mShininess = pShininess;
        }

        public Vector3 getAmbientColour()
        {
            return mAmbientColour;
        }

        public Vector3 getDiffuseColour()
        {
            return mDiffuseColour;
        }

        public Vector3 getSpecularColour()
        {
            return mSpecularColour;
        }

        public float getShininess()
        {
            return mShininess;
        }

        public void setVertices(float[] pVertices)
        {
            mVertices = pVertices;
        }

        public void setIndices(int[] pIndices)
        {
            mIndices = pIndices;
        }

        public float[] getVertices()
        {
            return mVertices;
        }

        public int[] getIndices()
        {
            return mIndices;
        }

        public void setVAOid(int pVAO_id)
        {
            mVAO_id = pVAO_id;
        }

        public int getVAOid()
        {
            return mVAO_id;
        }

        public void setVBOids(List<int> pVBO_ids)
        {
            mVBO_ids = pVBO_ids;
        }

        public List<int> getVBOids()
        {
            return mVBO_ids;
        }

        public virtual void Draw()
        {
            
        }

        public virtual int getTextureLocation()
        {
            return -1;
        }

        public virtual void GenerateBufferObjects(int vPositionLocation, int vNormalLocation)
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

            GL.EnableVertexAttribArray(vPositionLocation);
            GL.VertexAttribPointer(vPositionLocation, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);

            GL.EnableVertexAttribArray(vNormalLocation);
            GL.VertexAttribPointer(vNormalLocation, 3, VertexAttribPointerType.Float, true, 6 * sizeof(float), 3 * sizeof(float));
        }

        public void RotateY(float rotateValue)
        {
            Vector3 t = getPosition().ExtractTranslation();
            Matrix4 translation = Matrix4.CreateTranslation(t);
            Matrix4 inverseTranslation = Matrix4.CreateTranslation(-t);
            setPosition(getPosition() * inverseTranslation * Matrix4.CreateRotationY(rotateValue) * translation);
        }

        public void rotateX(float rotateValue)
        {
            Vector3 t = getPosition().ExtractTranslation();
            Matrix4 translation = Matrix4.CreateTranslation(t);
            Matrix4 inverseTranslation = Matrix4.CreateTranslation(-t);
            setPosition(getPosition() * inverseTranslation * Matrix4.CreateRotationX(rotateValue) * translation);
        }

        public void rotateZ(float rotateValue)
        {
            Vector3 t = getPosition().ExtractTranslation();
            Matrix4 translation = Matrix4.CreateTranslation(t);
            Matrix4 inverseTranslation = Matrix4.CreateTranslation(-t);
            setPosition(getPosition() * inverseTranslation * Matrix4.CreateRotationZ(rotateValue) * translation);
        }

        public void spin(float rotateValue) //rotates around the origin of the program, not it's position
        {
            setPosition(getPosition() * Matrix4.CreateRotationY(rotateValue));
        }

        public void moveX()
        {
            
            Vector3 t = getPosition().ExtractTranslation();
            if(t.X > 5)
            {
                mMoveAmountX = Vector3.Multiply(mMoveAmountX, -1);
            }
            else if(t.X < -5)
            {
                mMoveAmountX = Vector3.Multiply(mMoveAmountX, -1);
            }

            setPosition(getPosition() * Matrix4.CreateTranslation(mMoveAmountX));
        }

        public void scale()
        {
            Vector3 t = getPosition().ExtractTranslation();
            if (t.Y > 10)
            {
                mScaleAmount = Matrix4.CreateScale(0.9f);
            }
            else if(t.Y < 0.5)
            {
                mScaleAmount = Matrix4.CreateScale(1.1f);
            }

            setPosition(getPosition() * mScaleAmount);
        }


    }
}
