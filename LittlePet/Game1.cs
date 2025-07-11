using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;

namespace LittlePet;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    Texture2D Maptexture;

    private PlayMap _playMap;

    // Размеры игрового поля (в клетках)
    private const int GridWidth = 5;
    private const int GridHeight = 5;

    // Размер одной клетки
    private const int CellSize = 64;

    bool dPressed = false;
    bool aPressed = false;
    bool sPressed = false;
    bool wPressed = false;

    MovingSprite _EshSprite;
    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here

        _graphics.IsFullScreen = false;
        _graphics.PreferredBackBufferWidth = 650;
        _graphics.PreferredBackBufferHeight = 650;
        _graphics.ApplyChanges();

        _playMap = new PlayMap(GridWidth, GridHeight, CellSize);
        

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        // TODO: use this.Content to load your game content here

        Texture2D texture = Content.Load<Texture2D>("player");
        _EshSprite = new MovingSprite(texture, Vector2.Zero);

        Maptexture = Content.Load<Texture2D>("Tile");
        _playMap.GenerateMap(Maptexture, Maptexture, Maptexture);
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        if (!dPressed && Keyboard.GetState().IsKeyDown(Keys.D))
        {
            dPressed = true;
            Debug.WriteLine("D");
            _EshSprite.MoveRight();
        }
        if (Keyboard.GetState().IsKeyUp(Keys.D))
        {
            dPressed = false;
        }

        if (!aPressed && Keyboard.GetState().IsKeyDown(Keys.A))
        {
            aPressed = true;
            Debug.WriteLine("A");
            _EshSprite.MoveLeft();
        }
        if (Keyboard.GetState().IsKeyUp(Keys.A))
        {
            aPressed = false;
        }

        if (!wPressed && Keyboard.GetState().IsKeyDown(Keys.W))
        {
            wPressed = true;
            Debug.WriteLine("W");
            _EshSprite.MoveUp();
        }
        if (Keyboard.GetState().IsKeyUp(Keys.W))
        {
            wPressed = false;
        }

        if (!sPressed && Keyboard.GetState().IsKeyDown(Keys.S))
        {
            sPressed = true;
            Debug.WriteLine("S");
            _EshSprite.MoveDown();
        }
        if (Keyboard.GetState().IsKeyUp(Keys.S))
        {
            sPressed = false;
        }

        // TODO: Add your update logic here

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        // TODO: Add your drawing code here

        _spriteBatch.Begin(samplerState: SamplerState.PointClamp);

        _playMap.Draw(_spriteBatch);
        _spriteBatch.Draw(_EshSprite.texture, _EshSprite.Rect, Color.White);

        _spriteBatch.End();

        base.Draw(gameTime);
    }
}
