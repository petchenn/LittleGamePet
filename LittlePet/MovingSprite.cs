using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LittlePet
{
    public class MovingSprite : ScaledSprite
    {
        private float _speed;
        public MovingSprite(Texture2D texture, Vector2 position, float speed = 64) : base(texture, position)
        {
            this._speed = speed;
        }
        public void MoveRight()
        {
            position.X += _speed;
        }
        public void MoveLeft()
        {
            position.X -= _speed;
        }
        public void MoveUp()
        {
            position.Y -= _speed;
        }
        public void MoveDown()
        {
            position.Y += _speed;
        }
    }
}
