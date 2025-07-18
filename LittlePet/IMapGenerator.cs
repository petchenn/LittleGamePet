using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LittlePet
{
    interface IMapGenerator
    {
        PlayMap GenerateMap(int width, int height, int cellSize, Texture2D floorTexture, Texture2D wallTexture, Texture2D enemyTexture, Texture2D healTexture);
    }
}
