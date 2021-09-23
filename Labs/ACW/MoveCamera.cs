using Labs.Utility;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System;

namespace Labs.ACW
{
    public class MoveCamera : StationaryCamera
    {
        public MoveCamera(Matrix4 pView) : base(pView)
        {
            mView = pView;
        }

        public override bool canMove()
        {
            return true;
        }
        public override void goUp()
        {
            setPosition(mView * Matrix4.CreateTranslation(0,-0.2f,0));
        }

        public override void goDown()
        {
            setPosition(mView * Matrix4.CreateTranslation(0, 0.2f, 0));
        }

        public override void turnLeft()
        {
            setPosition(mView * Matrix4.CreateRotationY(-0.025f));
        }

        public override void turnRight()
        {
            setPosition(mView * Matrix4.CreateRotationY(0.025f));
        }

        public override void moveForward()
        {
            setPosition(mView * Matrix4.CreateTranslation(0.0f, 0.0f, 0.05f));
        }

        public override void moveBack()
        {
            setPosition(mView * Matrix4.CreateTranslation(0.0f, 0.0f, -0.05f));
        }
    }
}
