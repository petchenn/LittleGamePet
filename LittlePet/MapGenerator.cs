using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LittlePet
{
    class SmallMapGenerator : IMapGenerator
    {
        public PlayMap GenerateMap(int width, int height, int cellSize, Texture2D floorTexture, Texture2D wallTexture, Texture2D enemyTexture, Texture2D healTexture)
        {
            PlayMap map = new PlayMap(width, height, cellSize);
            map.cells = new Cell[width][];
            for (int x = 0; x < width; x++)
            {
                map.cells[x] = new Cell[height];
                for (int y = 0; y < height; y++)
                {
                    map.cells[x][y] = new FloorCell(new Vector2(x * cellSize, y * cellSize), floorTexture);
                }
            }

            // Добавляем стены и врагов
            map.cells[1][1] = new WallCell(new Vector2(1 * cellSize, 1 * cellSize), wallTexture);
            map.cells[2][1] = new WallCell(new Vector2(2 * cellSize, 1 * cellSize), wallTexture);
            map.cells[3][1] = new WallCell(new Vector2(3 * cellSize, 1 * cellSize), wallTexture);

            map.cells[1][4] = new EnemyCell(new Vector2(1 * cellSize, 4 * cellSize), enemyTexture);
            map.cells[3][4] = new EnemyCell(new Vector2(3 * cellSize, 4 * cellSize), enemyTexture);
            map.cells[2][4] = new HealCell(new Vector2(2 * cellSize, 4 * cellSize), healTexture);

            return map;
        }
    }

    class MediumMapGenerator : IMapGenerator
    {
        public PlayMap GenerateMap(int width, int height, int cellSize, Texture2D floorTexture, Texture2D wallTexture, Texture2D enemyTexture, Texture2D healTexture)
        {
            PlayMap map = new PlayMap(width, height, cellSize);
            map.cells = new Cell[width][];
            for (int x = 0; x < width; x++)
            {
                map.cells[x] = new Cell[height];
                for (int y = 0; y < height; y++)
                {
                    map.cells[x][y] = new FloorCell(new Vector2(x * cellSize, y * cellSize), floorTexture);
                }
            }

            // Другая конфигурация стен и врагов для средней карты
            map.cells[1][1] = new WallCell(new Vector2(1 * cellSize, 1 * cellSize), wallTexture);
            map.cells[2][1] = new WallCell(new Vector2(2 * cellSize, 1 * cellSize), wallTexture);
            map.cells[3][1] = new WallCell(new Vector2(3 * cellSize, 1 * cellSize), wallTexture);
            map.cells[4][1] = new WallCell(new Vector2(4 * cellSize, 1 * cellSize), wallTexture);

            map.cells[1][4] = new EnemyCell(new Vector2(1 * cellSize, 4 * cellSize), enemyTexture);
            map.cells[3][4] = new EnemyCell(new Vector2(3 * cellSize, 4 * cellSize), enemyTexture);
            map.cells[5][5] = new EnemyCell(new Vector2(5 * cellSize, 5 * cellSize), enemyTexture);
            map.cells[2][4] = new HealCell(new Vector2(2 * cellSize, 4 * cellSize), healTexture);
            map.cells[6][2] = new HealCell(new Vector2(6 * cellSize, 2 * cellSize), healTexture);

            return map;
        }
    }

    class LargeMapGenerator : IMapGenerator
    {
        public PlayMap GenerateMap(int width, int height, int cellSize, Texture2D floorTexture, Texture2D wallTexture, Texture2D enemyTexture, Texture2D healTexture)
        {
            PlayMap map = new PlayMap(width, height, cellSize);
            map.cells = new Cell[width][];
            for (int x = 0; x < width; x++)
            {
                map.cells[x] = new Cell[height];
                for (int y = 0; y < height; y++)
                {
                    map.cells[x][y] = new FloorCell(new Vector2(x * cellSize, y * cellSize), floorTexture);
                }
            }

            // Еще одна конфигурация стен и врагов для большой карты
            map.cells[1][1] = new WallCell(new Vector2(1 * cellSize, 1 * cellSize), wallTexture);
            map.cells[2][1] = new WallCell(new Vector2(2 * cellSize, 1 * cellSize), wallTexture);
            map.cells[3][1] = new WallCell(new Vector2(3 * cellSize, 1 * cellSize), wallTexture);
            map.cells[4][1] = new WallCell(new Vector2(4 * cellSize, 1 * cellSize), wallTexture);
            map.cells[5][1] = new WallCell(new Vector2(5 * cellSize, 1 * cellSize), wallTexture);

            map.cells[1][4] = new EnemyCell(new Vector2(1 * cellSize, 4 * cellSize), enemyTexture);
            map.cells[3][4] = new EnemyCell(new Vector2(3 * cellSize, 4 * cellSize), enemyTexture);
            map.cells[5][5] = new EnemyCell(new Vector2(5 * cellSize, 5 * cellSize), enemyTexture);
            map.cells[7][6] = new EnemyCell(new Vector2(7 * cellSize, 6 * cellSize), enemyTexture);
            map.cells[2][4] = new HealCell(new Vector2(2 * cellSize, 4 * cellSize), healTexture);
            map.cells[6][2] = new HealCell(new Vector2(6 * cellSize, 2 * cellSize), healTexture);
            map.cells[8][7] = new HealCell(new Vector2(8 * cellSize, 7 * cellSize), healTexture);

            return map;
        }
    }
}
