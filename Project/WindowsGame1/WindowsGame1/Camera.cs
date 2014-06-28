using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GetMoneyGame
{
    class Camera
    {
        #region Fields
        public Vector3 position = Vector3.Zero;
        private Vector3 target;
        private Vector3 up;
        private float moveSpeed = 1f;
        public float leftrightRot;
        public float updownRot;
        private Vector3 baseCameraReference = new Vector3(0, 0, 1);

        #endregion

        #region Properties
        public Matrix projection;
        public Matrix View;

        #endregion

        #region Constructor
        public Camera(Vector3 position, Vector3 up, float aspectRatio, float nearPlane, float farPlane)
        {
            this.position = position;
            this.leftrightRot = 0;
            this.updownRot = 0;
            this.up = up;
            projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4 * 1.3f,
                aspectRatio, nearPlane, farPlane);
            UpdateViewMatrix();
        }

        #endregion

        #region Helper Methods
        public void UpdateViewMatrix()
        {
            Matrix cameraRotation = Matrix.CreateRotationX(updownRot) * Matrix.CreateRotationY(leftrightRot);
            Vector3 cameraOriginalTarget = new Vector3(0, 0, -1);
            Vector3 cameraOriginalUpVector = new Vector3(0, 1, 0);
            Vector3 cameraRotatedTarget = Vector3.Transform(cameraOriginalTarget, cameraRotation);
            target = position + cameraRotatedTarget;
            up = Vector3.Transform(cameraOriginalUpVector, cameraRotation);
            View = Matrix.CreateLookAt(position, target, up);
        }

        public void AddToCameraPosition(Vector3 vectorToAdd)
        {
            Matrix cameraRotation = Matrix.CreateRotationX(updownRot) * Matrix.CreateRotationY(leftrightRot);
            Vector3 rotatedVector = Vector3.Transform(vectorToAdd, cameraRotation);
            position += moveSpeed * rotatedVector;
            UpdateViewMatrix();
        }
        public void MoveTo(Vector3 position) 
        {
            this.position = position;
            UpdateViewMatrix();
        }

        // Used for checking moves against our terrain 
        public Vector3 PreviewMove(float scale)
        {
            Matrix rotate = Matrix.CreateRotationY(leftrightRot);
            Vector3 forward = new Vector3(0, 0, scale);
            forward = Vector3.Transform(forward, rotate);
            return (position + forward);
        }
        public Vector3 PreviewMoveLeftRight(float scale) 
        {
            Matrix rotate = Matrix.CreateRotationY(leftrightRot);
            Vector3 forward = new Vector3(scale, 0, 0);
            forward = Vector3.Transform(forward, rotate);
            return (position + forward);
        }
        public void MoveForward(float scale) // Move back and forth
        {
            MoveTo(PreviewMove(scale));
        }
        public void MoveSide(float scale) //Move left-right
        {
            MoveTo(PreviewMoveLeftRight(scale));
        }

        #endregion
    }
}
