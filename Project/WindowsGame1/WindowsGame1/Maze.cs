using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GetMoneyGame
{
    class Maze
    {
        #region Fields
        public const int mazeWidth = 20;
        public const int mazeHeight = 20;
        GraphicsDevice device;
        VertexBuffer floorBuffer;
        Color[] floorColors = new Color[2] { Color.White, Color.Gray };

        private Random rand = new Random();
        public MazeCell[,] MazeCells = new MazeCell[mazeWidth, mazeHeight];

        VertexBuffer wallBuffer;
        Vector3[] wallPoints = new Vector3[8];
        Color[] wallColors = new Color[4] {
        Color.Green, Color.Orange, Color.Green, Color.Orange};
        #endregion

        #region Constructor
        public Maze(GraphicsDevice device)
        {
            this.device = device;
            BuildFloorBuffer();

            // Maze
            for (int x = 0; x < mazeWidth; x++)
                for (int z = 0; z < mazeHeight; z++)
                {
                    MazeCells[x, z] = new MazeCell();
                }
            GenerateMaze(); // Here we build the maze
            RemoveRandomWalls(200); // We remove random walls to open up the maze


            wallPoints[0] = new Vector3(0, 1, 0);
            wallPoints[1] = new Vector3(0, 1, 1);
            wallPoints[2] = new Vector3(0, 0, 0);
            wallPoints[3] = new Vector3(0, 0, 1);
            wallPoints[4] = new Vector3(1, 1, 0);
            wallPoints[5] = new Vector3(1, 1, 1);
            wallPoints[6] = new Vector3(1, 0, 0);
            wallPoints[7] = new Vector3(1, 0, 1);
            BuildWallBuffer();
        }
        #endregion
        #region The Floor
        private void BuildFloorBuffer()
        {
            // Floor
            List<VertexPositionColor> vertexList =
                new List<VertexPositionColor>();
            int counter = 0;
            for (int x = 0; x < mazeWidth; x++)
            {
                counter++;
                for (int z = 0; z < mazeHeight; z++)
                {
                    counter++;
                    foreach (VertexPositionColor vertex in
                        FloorTile(x, z, floorColors[counter % 2]))
                    {
                        vertexList.Add(vertex);
                    }
                }
            }
            floorBuffer = new VertexBuffer(
                device,
                VertexPositionColor.VertexDeclaration,
                vertexList.Count,
                BufferUsage.WriteOnly);
            floorBuffer.SetData<VertexPositionColor>(vertexList.
            ToArray());


        }

        private List<VertexPositionColor> FloorTile(
            int xOffset,
            int zOffset,
            Color tileColor)
        {
            List<VertexPositionColor> vList =
            new List<VertexPositionColor>();
            vList.Add(new VertexPositionColor(
                new Vector3(0 + xOffset, 0, 0 + zOffset), tileColor));
            vList.Add(new VertexPositionColor(
                new Vector3(1 + xOffset, 0, 0 + zOffset), tileColor));
            vList.Add(new VertexPositionColor(
                new Vector3(0 + xOffset, 0, 1 + zOffset), tileColor));
            vList.Add(new VertexPositionColor(
                new Vector3(1 + xOffset, 0, 0 + zOffset), tileColor));
            vList.Add(new VertexPositionColor(
                new Vector3(1 + xOffset, 0, 1 + zOffset), tileColor));
            vList.Add(new VertexPositionColor(
                new Vector3(0 + xOffset, 0, 1 + zOffset), tileColor));
            return vList;
        }
        #endregion

        #region Draw
        public void Draw(Camera camera, BasicEffect effect)
        {
            effect.VertexColorEnabled = true;
            effect.TextureEnabled = false;  // Had to add this when we added our cube since we are switching between vertexcolor and texture
            effect.World = Matrix.Identity; // Identity matrix is just like multiplying something by 1
            effect.View = camera.View;
            effect.Projection = camera.projection;
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();   // Start the pass
                device.SetVertexBuffer(floorBuffer);  // Give the vertex buffer to the gfx card
                device.DrawPrimitives(
                    PrimitiveType.TriangleList, // The type of primitive we are drawing
                    0,  // Starting with element 0
                    floorBuffer.VertexCount / 3);   // Going all the way to count/3 since each triangle has 3 vertices

                device.SetVertexBuffer(wallBuffer);  // Drawing the walls
                device.DrawPrimitives(
                    PrimitiveType.TriangleList,
                    0,
                    wallBuffer.VertexCount / 3);
            }
        }
        #endregion


        #region Maze Generation 
        public void GenerateMaze()
        {
            for (int x = 0; x < mazeWidth; x++)
                for (int z = 0; z < mazeHeight; z++)
                {
                    // All cells starts  with walls around them
                    MazeCells[x, z].Walls[0] = true;
                    MazeCells[x, z].Walls[1] = true;
                    MazeCells[x, z].Walls[2] = true;
                    MazeCells[x, z].Walls[3] = true;
                    MazeCells[x, z].Visited = false; // Have the generator visited this cell?
                }
            MazeCells[0, 0].Visited = true;
            EvaluateCell(new Vector2(0, 0)); // Start at 0,0 (upper left corner if you will)
        }

        private void EvaluateCell(Vector2 cell) // Does the real work of generating the cell walls
        {
            List<int> neighborCells = new List<int>();
            neighborCells.Add(0);
            neighborCells.Add(1);
            neighborCells.Add(2);
            neighborCells.Add(3);

            while (neighborCells.Count > 0)
            {
                int pick = rand.Next(0, neighborCells.Count);  // Pick a random neighbor cell (0 to 3)
                int selectedNeighbor = neighborCells[pick];
                neighborCells.RemoveAt(pick); 
                Vector2 neighbor = cell;  

                switch (selectedNeighbor)  // Take our last position and add a vector corresponding to the neightbour we chose
                {
                    case 0: neighbor += new Vector2(0, -1);
                    break;
                    case 1: neighbor += new Vector2(1, 0);
                    break;
                    case 2: neighbor += new Vector2(0, 1);
                    break;
                    case 3: neighbor += new Vector2(-1, 0);
                    break;
                }

                if ((neighbor.X >= 0) && (neighbor.X < mazeWidth) &&  // We check we are inside the bounds of the maze
                    (neighbor.Y >= 0) && (neighbor.Y <mazeHeight)) 
                {
                    if (!MazeCells[(int)neighbor.X, (int)neighbor.Y].Visited)  // Visit a cell and do work on it
                    {
                        MazeCells[(int) neighbor.X, (int) neighbor.Y].Visited = true;
                        MazeCells[(int)cell.X, (int)cell.Y].Walls[selectedNeighbor] = false;

                        // Getting the opposite side walls for the neighbour cell
                        // using modulus 4 to wrap around from 4 to 0, this way the oposing side is allways
                        // the current side + 2.   ex: (2+2)%4 = 0, (3+2)%4 = 1
                        MazeCells[(int)neighbor.X, (int)neighbor.Y].Walls[(selectedNeighbor + 2) % 4] = false;
                        EvaluateCell(neighbor);
                    }
                }
            }
        }
        #endregion


        #region Walls
        private void BuildWallBuffer()
        {
            List<VertexPositionColor> wallVertexList = new
                List<VertexPositionColor>();
            for (int x = 0; x < mazeWidth; x++)
            {
                for (int z = 0; z < mazeHeight; z++)
                {
                    foreach (VertexPositionColor vertex in BuildMazeWall(x, z))
                    {
                        wallVertexList.Add(vertex);
                    }
                }
            }
            wallBuffer = new VertexBuffer(
                device,
                VertexPositionColor.VertexDeclaration,
                wallVertexList.Count,
                BufferUsage.WriteOnly);
            wallBuffer.SetData<VertexPositionColor>(wallVertexList.ToArray());
        }

        private List<VertexPositionColor> BuildMazeWall(int x, int z)
        {
            List<VertexPositionColor> triangles = new
            List<VertexPositionColor>();
            if (MazeCells[x, z].Walls[0])
            {
                triangles.Add(CalcPoint(0, x, z, wallColors[0]));
                triangles.Add(CalcPoint(4, x, z, wallColors[0]));
                triangles.Add(CalcPoint(2, x, z, wallColors[0]));
                triangles.Add(CalcPoint(4, x, z, wallColors[0]));
                triangles.Add(CalcPoint(6, x, z, wallColors[0]));
                triangles.Add(CalcPoint(2, x, z, wallColors[0]));
            }
            if (MazeCells[x, z].Walls[1])
            {
                triangles.Add(CalcPoint(4, x, z, wallColors[1]));
                triangles.Add(CalcPoint(5, x, z, wallColors[1]));
                triangles.Add(CalcPoint(6, x, z, wallColors[1]));
                triangles.Add(CalcPoint(5, x, z, wallColors[1]));
                triangles.Add(CalcPoint(7, x, z, wallColors[1]));
                triangles.Add(CalcPoint(6, x, z, wallColors[1]));
            }
            if (MazeCells[x, z].Walls[2])
            {
                triangles.Add(CalcPoint(5, x, z, wallColors[2]));
                triangles.Add(CalcPoint(1, x, z, wallColors[2]));
                triangles.Add(CalcPoint(7, x, z, wallColors[2]));
                triangles.Add(CalcPoint(1, x, z, wallColors[2]));
                triangles.Add(CalcPoint(3, x, z, wallColors[2]));
                triangles.Add(CalcPoint(7, x, z, wallColors[2]));
            }
            if (MazeCells[x, z].Walls[3])  
            {
                triangles.Add(CalcPoint(1, x, z, wallColors[3]));
                triangles.Add(CalcPoint(0, x, z, wallColors[3]));
                triangles.Add(CalcPoint(3, x, z, wallColors[3]));
                triangles.Add(CalcPoint(0, x, z, wallColors[3]));
                triangles.Add(CalcPoint(2, x, z, wallColors[3]));
                triangles.Add(CalcPoint(3, x, z, wallColors[3]));
            }
            return triangles;
        }

        private VertexPositionColor CalcPoint(
            int wallPoint, int xOffset, int zOffset, Color color)
        {
            return new VertexPositionColor(
                wallPoints[wallPoint] + new Vector3(xOffset, 0, zOffset),
                color);
        }

        private BoundingBox BuildBoundingBox(
            int x,
            int z,
            int point1,
            int point2) 
        {
            BoundingBox thisBox = new BoundingBox(
                wallPoints[point1],
                wallPoints[point2]);
            thisBox.Min.X += x;
            thisBox.Min.Z += z;
            thisBox.Max.X += x;
            thisBox.Max.Z += z;
            thisBox.Min.X -= 0.1f;
            thisBox.Min.Z -= 0.1f;
            thisBox.Max.X += 0.1f;
            thisBox.Max.Z += 0.1f;
            return thisBox;
        }
        public List<BoundingBox> GetBoundsForCell(int x, int z)
        {
            List<BoundingBox> boxes = new List<BoundingBox>();
            try
            {
                if (MazeCells[x, z].Walls[0])
                    boxes.Add(BuildBoundingBox(x, z, 2, 4));
                if (MazeCells[x, z].Walls[1])
                    boxes.Add(BuildBoundingBox(x, z, 6, 5));
                if (MazeCells[x, z].Walls[2])
                    boxes.Add(BuildBoundingBox(x, z, 3, 5));
                if (MazeCells[x, z].Walls[3])
                    boxes.Add(BuildBoundingBox(x, z, 2, 1));
            }
            catch (Exception) 
            {
                // "The user dont wanna see this"
                // This solves a 'index out of bounds' error that occurs sometimes
                // when stupidly running into the outer maze walls
            }
            return boxes;
        }

        #endregion

        #region Manipulate maze

        public void RemoveRandomWalls(int howMany)
        {
            int removed = 0;
            while(removed <= howMany)
            {
                int x = rand.Next(0,mazeHeight);
                int y = rand.Next(0,mazeWidth);
                int wall = rand.Next(0,3);
                if(MazeCells[x, y].Walls[wall]) // if there's a wall
                {

                    Vector2 neighbor = new Vector2(x,y);

                    switch (wall)  // Find the neighbor
                    {
                        case 0: neighbor += new Vector2(0, -1);
                            break;
                        case 1: neighbor += new Vector2(1, 0);
                            break;
                        case 2: neighbor += new Vector2(0, 1);
                            break;
                        case 3: neighbor += new Vector2(-1, 0);
                            break;
                    }

                    if ((neighbor.X >= 0) && (neighbor.X < mazeWidth) &&  // We check we are inside the bounds of the maze
                        (neighbor.Y >= 0) && (neighbor.Y < mazeHeight))
                    {
                            // Remove the wall we chose
                            MazeCells[x, y].Walls[wall] = false;
                            // And corresponding neighbor wall
                            MazeCells[(int)neighbor.X, (int)neighbor.Y].Walls[(wall + 2) % 4] = false;
                            removed += 1;
                    }
                }
            }
        }

        #endregion


    }
}
