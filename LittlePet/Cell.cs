using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Text.Json.Serialization;

// Базовый класс для всех ячеек
[JsonConverter(typeof(CellConverter))]
public abstract class Cell
{
    public bool isDied { get; set; }
    public bool isUsed { get; set; }
    [JsonConverter(typeof(Vector2Converter))] public Vector2 Position { get; set; }  // Координаты ячейки на поле

    public Color Color { get; set; } //  Цвет по умолчанию
    [JsonIgnore] public Texture2D Texture { get; set; }

    [JsonConstructor]
    public Cell() {
        isDied = true;
        isUsed = true;
    }

    public Cell(Vector2 position, Texture2D texture)
    {
        Position = position;
        Texture = texture;
    }

    public abstract void Update();
    public abstract void Draw(SpriteBatch spriteBatch);
}

// Остальные классы остаются без изменений
public class FloorCell : Cell
{
    public FloorCell(Vector2 position, Texture2D texture) : base(position, texture)
    {
        Color = Color.Gray;
    }
    public FloorCell() { }

    public override void Update()
    {
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(Texture, Position, Color);
    }
}

public class WallCell : Cell
{
    public WallCell() { }

    public WallCell(Vector2 position, Texture2D texture) : base(position, texture)
    {
        Color = Color.Brown;
    }
    public override void Update()
    {
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(Texture, Position, Color);
    }
}

public class EnemyCell : Cell
{
    public EnemyCell() {
        isDied = false;
    }

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
    public HealCell() {
        isUsed = false;
    }

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