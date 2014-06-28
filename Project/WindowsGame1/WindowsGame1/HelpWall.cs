using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GetMoneyGame
{
    class HelpWall  
    {

        // This will provide a wall with a texture displaying info for the player
        // It will be the first thing the user sees when he starts  the game. And as such will
        // tell him to turn around and start playing
                #region Fields
        private GraphicsDevice device;
        private Texture2D texture;
        private Vector3 location;
        private VertexBuffer cubeVertexBuffer;
        private List<VertexPositionTexture> vertices = new List<VertexPositionTexture>();
        private float rotation = 0f;
        private float zrotation = 0f;
        #endregion

        #region Constructor
        public HelpWall(
        GraphicsDevice graphicsDevice,
        Vector3 playerLocation,
        float minDistance,
        Texture2D texture)
        {
            device = graphicsDevice;
            this.texture = texture;
            PositionCube(playerLocation, minDistance);
            // Create the cube's vertical faces
            //BuildFace(new Vector3(0, 0, 0), new Vector3(0, 1, 1)); 
            BuildFace(new Vector3(0, 0, 1), new Vector3(1, 1, 1));      // only need one of these walls, dont know wich yet
            //BuildFace(new Vector3(1, 0, 1), new Vector3(1, 1, 0));
            //BuildFace(new Vector3(1, 0, 0), new Vector3(0, 1, 0));
            //// Create the cube's horizontal faces
            //BuildFaceHorizontal(new Vector3(0, 1, 0), new Vector3(1, 1, 1));
            //BuildFaceHorizontal(new Vector3(0, 0, 1), new Vector3(1, 0, 0));
            cubeVertexBuffer = new VertexBuffer(
                device,
                VertexPositionTexture.VertexDeclaration,
                vertices.Count,
                BufferUsage.WriteOnly);
            cubeVertexBuffer.SetData<VertexPositionTexture>(vertices.ToArray());
        }
        #endregion

        #region Helper Methods
        private void BuildFace(Vector3 p1, Vector3 p2)
        {
            vertices.Add(BuildVertex(p1.X, p1.Y, p1.Z, 1, 0));
            vertices.Add(BuildVertex(p1.X, p2.Y, p1.Z, 1, 1));
            vertices.Add(BuildVertex(p2.X, p2.Y, p2.Z, 0, 1));
            vertices.Add(BuildVertex(p2.X, p2.Y, p2.Z, 0, 1));
            vertices.Add(BuildVertex(p2.X, p1.Y, p2.Z, 0, 0));
            vertices.Add(BuildVertex(p1.X, p1.Y, p1.Z, 1, 0));
        }

        private VertexPositionTexture BuildVertex(
            float x,
            float y,
            float z,
            float u,
            float v)
        {
            return new VertexPositionTexture(
                new Vector3(x, y, z),
                new Vector2(u, v));
        }
        public void PositionCube(Vector3 playerLocation, float minDistance)
        {
            location = new Vector3(0.5f, 0.5f, 0f); // 0.5 above the floor?
        }
        #endregion

        #region Draw
        public void Draw(Camera camera, BasicEffect effect)
        {
            effect.VertexColorEnabled = false;
            effect.TextureEnabled = true;
            effect.Texture = texture;
            Matrix center = Matrix.CreateTranslation(
                new Vector3(-0.5f, -0.5f, -0.5f));
            Matrix scale = Matrix.CreateScale(0.3f);
            Matrix translate = Matrix.CreateTranslation(location);
            Matrix rot = Matrix.CreateRotationY(rotation);  // Rotation
            Matrix zrot = Matrix.CreateRotationZ(zrotation);// Rotation

            // Since we are tweaking the cube class we need some startup fixes
            Matrix rotateBeforeStart = Matrix.CreateRotationZ(MathHelper.Pi);

            effect.World = center * scale * rotateBeforeStart * rot * zrot * translate;  // The order of operations is important, atleast translate
            effect.View = camera.View;
            effect.Projection = camera.projection;
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                device.SetVertexBuffer(cubeVertexBuffer);
                device.SamplerStates[0] = SamplerState.LinearClamp;  // Since we are using textures of non-ideal sizes
                device.DrawPrimitives(
                    PrimitiveType.TriangleList,
                    0,
                    cubeVertexBuffer.VertexCount / 3);
            }
        }
        #endregion

        #region Update
        public void Update(GameTime gameTime) 
        {
            // Rotate the cube
            rotation = MathHelper.WrapAngle(rotation + 0.05f);
            zrotation = MathHelper.WrapAngle(zrotation + 0.025f);
        }
        #endregion
    }
}
