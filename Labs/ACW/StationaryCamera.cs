using Labs.Utility;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System;

namespace Labs.ACW
{
    public class StationaryCamera
    {
        protected Matrix4 mView;
        public StationaryCamera(Matrix4 pView)
        {
            setPosition(pView);
        }

        public void setPosition(Matrix4 pView)
        {
            mView = pView;
        }

        public Matrix4 getPosition()
        {
            return mView;
        }

        public virtual bool canMove()
        {
            return false;
        }

        public virtual void turnLeft()
        {

        }

        public virtual void turnRight()
        {

        }

        public virtual void moveForward()
        {

        }

        public virtual void moveBack()
        {

        }

        public virtual void goUp()
        {

        }
        public virtual void goDown()
        {

        }

        public void RotateY(float rotateAmount)
        {
            Vector3 t = getPosition().ExtractTranslation();
            Matrix4 translation = Matrix4.CreateTranslation(t);
            Matrix4 inverseTranslation = Matrix4.CreateTranslation(-t);
            setPosition(getPosition() * inverseTranslation * Matrix4.CreateRotationY(rotateAmount) * translation);
        }

        public void RotateX(float rotateAmount)
        {
            Vector3 t = getPosition().ExtractTranslation();
            Matrix4 translation = Matrix4.CreateTranslation(t);
            Matrix4 inverseTranslation = Matrix4.CreateTranslation(-t);
            setPosition(getPosition() * inverseTranslation * Matrix4.CreateRotationX(rotateAmount) * translation);
        }

    }
}
