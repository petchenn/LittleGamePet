using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

// Базовый класс для всех ячеек
public abstract class Cell
{
    public bool isDied = true;
    public bool isUsed = true;
    public Vector2 Position { get; set; }  // Координаты ячейки на поле
    public Color Color { get; set; } = Color.White; //  Цвет по умолчанию
    public Texture2D Texture { get; set; }

    public Cell(Vector2 position, Texture2D texture)
    {
        Position = position;
        Texture = texture;
    }

    public abstract void Update();
    public abstract void Draw(SpriteBatch spriteBatch);

}

public class FloorCell : Cell
{
    public FloorCell(Vector2 position, Texture2D texture) : base(position, texture)
    {
        Color = Color.Gray;
    }

    public override void Update()
    {
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(Texture, Position, Color);
    }

}

// Класс для стены
public class WallCell : Cell
{
    public WallCell(Vector2 position, Texture2D texture) : base(position, texture)
    {
        Color = Color.Brown; //  Например, коричневый цвет для стены
    }
    public override void Update()
    {
        // Логика обновления для стены (если нужна)
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(Texture, Position, Color);
    }

}

// Класс для врага
public class EnemyCell : Cell
{
    public EnemyCell(Vector2 position, Texture2D texture) : base(position, texture)
    {
        Color = Color.Red;
        isDied = false;
    }

    public override void Update()
    {
        if (isDied) Color = Color.White;
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(Texture, Position, Color);
    }
}

public class HealCell : Cell
{
    public HealCell(Vector2 position, Texture2D texture) : base(position, texture)
    {
        Color = Color.Green;
        isUsed = false;
    }
    public override void Update()
    {
        if (isUsed) Color = Color.White;
    }
    public override void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(Texture, Position, Color);
    }
}