using Labs.Utility;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System;

namespace Labs.ACW
{
    public class Lighting
    {
        protected float mCutoffAngle;
        protected Vector3 mSpotDirection;
        public bool isCone = false;
        public bool isON = true;
        protected Vector4 mPosition;
        protected int mLightingID;
        protected Vector3 mColour;
        public Lighting(Vector4 pPosition, Vector3 pColour, int pLightID)
        {
            setPosition(pPosition);
            setLightingColour(pColour);
            setLightingID(pLightID);
        }

        public void setPosition(Vector4 pPosition)
        {
            mPosition = pPosition;
        }
        public Vector4 getPosition()
        {
            return mPosition;
        }
        public void setLightingID(int pLightingID)
        {
            mLightingID = pLightingID;
        }
        public int getLightingID()
        {
            return mLightingID;
        }
        public void setLightingColour(Vector3 pColour)
        {
            mColour = pColour;
        }

        public Vector3 GetColour()
        {
            return mColour;
        }

        public virtual void setCutoffAngle(float pCutoffAngle)
        {
            mCutoffAngle = pCutoffAngle;
        }
        public virtual float GetCutoffAngle()
        {
            return mCutoffAngle;
        }


    }
}
