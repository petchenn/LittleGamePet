using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace LittlePet
{
    class PlayMap
    {
        public Cell[][] cells { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public int cellSize { get; set; }

        public PlayMap(int width, int height, int cellSize)
        {
            this.width = width;
            this.height = height;
            this.cellSize = cellSize;
        }

        public void UpdateTexture(Texture2D Floortexture, Texture2D Walltexture, Texture2D HealTexture)
        {
            if (cells != null)
            {
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        cells[x][y].Texture = Floortexture;
                    }
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (cells != null)
            {
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        cells[x][y].Update();
                        cells[x][y].Draw(spriteBatch);
                    }
                }
            }
        }

        public int GetEnemyCount()
        {
            int count = 0;
            foreach (Cell[] row in cells)
            {
                foreach (Cell cell in row)
                {
                    if (cell is EnemyCell && !cell.isDied)
                    {
                        count++;
                    }
                }
            }
            return count;
        }

        public Vector2 GetCellPosition(Vector2 gridPosition)
        {
            return new Vector2(gridPosition.X * cellSize, gridPosition.Y * cellSize);
        }

        public Cell GetCell(Vector2 gridPosition)
        {
            if (gridPosition.X >= 0 && gridPosition.X < width && gridPosition.Y >= 0 && gridPosition.Y < height)
            {
                return cells[(int)gridPosition.X][(int)gridPosition.Y];
            }
            return null;
        }

        public bool IsCellWalkable(Vector2 gridPosition)
        {
            if (gridPosition.X < 0 || gridPosition.X >= width || gridPosition.Y < 0 || gridPosition.Y >= height)
            {
                return false;
            }

            Cell cell = GetCell(gridPosition);
            return !(cell is WallCell);
        }
    }
}