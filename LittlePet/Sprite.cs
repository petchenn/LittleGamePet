using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LittlePet
{
    public class Sprite
    {
        public Texture2D texture;
        public Vector2 position;

        public Sprite(Texture2D texture, Vector2 position)
        {
            this.texture = texture;
            this.position = position;
        }

        public void setPocition(Vector2 newPos)
        {
            position = newPos;
        }
        public virtual void Update() { }
    }
}
