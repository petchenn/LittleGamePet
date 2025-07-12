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
    Texture2D PlayerTexture; // Текстура игрока

    private PlayMap _playMap;

    // Размеры игрового поля (в клетках)
    private const int GridWidth = 6;
    private const int GridHeight = 6;

    // Размер одной клетки
    private const int CellSize = 64;

    bool dPressed = false;
    bool aPressed = false;
    bool sPressed = false;
    bool wPressed = false;

    MovingSprite _EshSprite;
    private Vector2 _playerGridPosition; // Позиция игрока на сетке (в координатах сетки)

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
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

        PlayerTexture = Content.Load<Texture2D>("player"); // Загрузка текстуры игрока
        _EshSprite = new MovingSprite(PlayerTexture, Vector2.Zero);

        Maptexture = Content.Load<Texture2D>("Tile");
        _playMap.GenerateMap(Maptexture, Maptexture, Maptexture);
        _playerGridPosition = Vector2.Zero; // Начальная позиция игрока на сетке (0, 0)
        _EshSprite.position = _playMap.GetCellPosition(_playerGridPosition); // Устанавливаем позицию спрайта в соответствии с сеткой
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        // Обработка ввода и перемещения
        HandleInput();

        // Обновление позиции спрайта (если перемещение произошло)
        _EshSprite.position = _playMap.GetCellPosition(_playerGridPosition);


        // Проверка коллизий и событий (бой)
        CheckCollisions();

        base.Update(gameTime);
    }

    private void HandleInput()
    {
        Vector2 newPosition = _playerGridPosition;

        if (!dPressed && Keyboard.GetState().IsKeyDown(Keys.D))
        {
            dPressed = true;
            newPosition.X++;
        }
        if (Keyboard.GetState().IsKeyUp(Keys.D))
        {
            dPressed = false;
        }

        if (!aPressed && Keyboard.GetState().IsKeyDown(Keys.A))
        {
            aPressed = true;
            newPosition.X--;
        }
        if (Keyboard.GetState().IsKeyUp(Keys.A))
        {
            aPressed = false;
        }

        if (!wPressed && Keyboard.GetState().IsKeyDown(Keys.W))
        {
            wPressed = true;
            newPosition.Y--;
        }
        if (Keyboard.GetState().IsKeyUp(Keys.W))
        {
            wPressed = false;
        }

        if (!sPressed && Keyboard.GetState().IsKeyDown(Keys.S))
        {
            sPressed = true;
            newPosition.Y++;
        }
        if (Keyboard.GetState().IsKeyUp(Keys.S))
        {
            sPressed = false;
        }

        // Проверка на выход за границы карты и на стены
        if (_playMap.IsCellWalkable(newPosition))
        {
            _playerGridPosition = newPosition;

            //_EshSprite.Move(_playMap.GetCellPosition(_playerGridPosition));
        }
    }

    private void CheckCollisions()
    {
        Cell currentCell = _playMap.GetCell(_playerGridPosition);

        if (currentCell is EnemyCell)
        {
            Debug.WriteLine("Начался бой!");
            //тут может быть ваш бой
        }
    }


    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        _spriteBatch.Begin(samplerState: SamplerState.PointClamp);

        _playMap.Draw(_spriteBatch);
        _spriteBatch.Draw(_EshSprite.texture, _EshSprite.Rect, Color.White);


        _spriteBatch.End();

        base.Draw(gameTime);
    }
}