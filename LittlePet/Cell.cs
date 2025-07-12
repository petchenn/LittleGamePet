using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

// Базовый класс для всех ячеек
public abstract class Cell
{
    public Vector2 Position { get; set; }  // Координаты ячейки на поле
    public Color Color { get; set; } = Color.White; //  Цвет по умолчанию
    public Texture2D Texture { get; set; }

    public Cell(Vector2 position, Texture2D texture)
    {
        Position = position;
        Texture = texture;
    }

    public abstract void Update(GameTime gameTime);
    public abstract void Draw(SpriteBatch spriteBatch);
    public abstract void Collision();

}

public class FloorCell : Cell
{
    public FloorCell(Vector2 position, Texture2D texture) : base(position, texture)
    {
        Color = Color.Gray;
    }

    public override void Update(GameTime gameTime)
    {
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(Texture, Position, Color);
    }

    public override void Collision()
    {
        //целое ничего
    }
}

// Класс для стены
public class WallCell : Cell
{
    public WallCell(Vector2 position, Texture2D texture) : base(position, texture)
    {
        Color = Color.Brown; //  Например, коричневый цвет для стены
    }
    public override void Update(GameTime gameTime)
    {
        // Логика обновления для стены (если нужна)
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(Texture, Position, Color);
    }

    public override void Collision()
    {
        //откидывание назад
    }
}

// Класс для врага
public class EnemyCell : Cell
{
    public bool isDied = false;

    public EnemyCell(Vector2 position, Texture2D texture) : base(position, texture)
    {
        Color = Color.Red;
    }

    public override void Update(GameTime gameTime)
    {
        if (isDied) Color = Color.White;
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(Texture, Position, Color);
    }
    public override void Collision()
    {
        //битва
    }
}