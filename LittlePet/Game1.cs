using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace LittlePet;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    Texture2D Maptexture;
    Texture2D PlayerTexture;
    Texture2D EnemyTexture;

    private PlayMap _playMap;

    private const int GridWidth = 6;
    private const int GridHeight = 6;
    private const int CellSize = 64;

    bool dPressed = false;
    bool aPressed = false;
    bool sPressed = false;
    bool wPressed = false;
    bool EnterPressed = false;

    MovingSprite _EshSprite;
    private Vector2 _playerGridPosition;

    private List<Pokemon> _playerTeam = new List<Pokemon>();
    private Pokemon _currentPokemon;

    //private Pokemon _enemyPokemon;  //Удаляем отсюда

    private enum GameState { Map, Battle, ChoosingPokemon }
    private GameState _currentGameState = GameState.ChoosingPokemon;

    private List<Pokemon> _availablePokemon = new List<Pokemon>();
    private int _selectedPokemonIndex = 0;

    private SpriteFont _font;

    // Добавляем экземпляр класса BattleManager
    private BattleManager _battleManager;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        _graphics.IsFullScreen = false;
        _graphics.PreferredBackBufferWidth = 800;
        _graphics.PreferredBackBufferHeight = 650;
        _graphics.ApplyChanges();

        _playMap = new PlayMap(GridWidth, GridHeight, CellSize);

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        PlayerTexture = Content.Load<Texture2D>("player");
        EnemyTexture = Content.Load<Texture2D>("enemy");
        _EshSprite = new MovingSprite(PlayerTexture, Vector2.Zero);

        Maptexture = Content.Load<Texture2D>("Tile");
        _playMap.GenerateMap(Maptexture, Maptexture, Maptexture);
        _playerGridPosition = Vector2.Zero;
        _EshSprite.position = _playMap.GetCellPosition(_playerGridPosition);

        _font = Content.Load<SpriteFont>("Font");

        _availablePokemon.Add(new Pokemon("Charmander", 5, 50, new List<Ability>() { new Ability("Ember", PokemonType.fire, 40) }, PokemonType.fire, 60, 40));
        _availablePokemon.Add(new Pokemon("Squirtle", 5, 55, new List<Ability>() { new Ability("Water Gun", PokemonType.water, 40) }, PokemonType.water, 45, 65));
        _availablePokemon.Add(new Pokemon("Bulbasaur", 5, 60, new List<Ability>() { new Ability("Vine Whip", PokemonType.normal, 35) }, PokemonType.normal, 50, 50));

        // Инициализация BattleManager
        _battleManager = new BattleManager(Content, _spriteBatch, _font, PlayerTexture, EnemyTexture); // Передаем необходимые зависимости
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        switch (_currentGameState)
        {
            case GameState.Map:
                UpdateMap(gameTime);
                break;
            case GameState.Battle:
                UpdateBattle(gameTime);
                break;
            case GameState.ChoosingPokemon:
                UpdateChoosingPokemon(gameTime);
                break;
        }

        base.Update(gameTime);
    }

    private void UpdateMap(GameTime gameTime)
    {
        HandleInput();
        _EshSprite.position = _playMap.GetCellPosition(_playerGridPosition);
        CheckCollisions();
    }

    private void UpdateBattle(GameTime gameTime)
    {
        _battleManager.Update(gameTime, _currentPokemon);

        if (_battleManager.BattleOver)
        {
            if (_battleManager.PlayerWon)
            {
                Console.WriteLine($"{_battleManager.EnemyPokemon.Name} побежден!");
                _currentPokemon.GainExp(10);
                _currentGameState = GameState.Map;
            }
            else
            {
                Console.WriteLine($"{_currentPokemon.Name} был побежден!");
                //TODO:  Смена покемона в команде, если он проиграл. Если все проиграли - Game Over
                _currentGameState = GameState.Map;
            }
            _battleManager.BattleOver = false;
        }

        // Обработка ввода в бою (выбор способности, и т.д.)
        if (!wPressed && Keyboard.GetState().IsKeyDown(Keys.D1))
        {
            wPressed = true;
            _battleManager.PlayerAttack(0, _currentPokemon);
        }
        if (Keyboard.GetState().IsKeyUp(Keys.D1))
        {
            wPressed = false;
        }
        else if (!sPressed && Keyboard.GetState().IsKeyDown(Keys.D2) && _currentPokemon.Abilities.Count > 1)
        {
            sPressed = true;
            _battleManager.PlayerAttack(1, _currentPokemon);
        }
        if (Keyboard.GetState().IsKeyUp(Keys.D2))
        {
            sPressed = false;
        }
    }

    private void UpdateChoosingPokemon(GameTime gameTime)
    {
        if (!dPressed && Keyboard.GetState().IsKeyDown(Keys.D))
        {
            dPressed = true;
            _selectedPokemonIndex = (_selectedPokemonIndex + 1) % _availablePokemon.Count;
        }
        if (Keyboard.GetState().IsKeyUp(Keys.D))
        {
            dPressed = false;
        }
        if (!aPressed && Keyboard.GetState().IsKeyDown(Keys.A))
        {
            aPressed = true;
            _selectedPokemonIndex = (_selectedPokemonIndex - 1 + _availablePokemon.Count) % _availablePokemon.Count;
        }
        if (Keyboard.GetState().IsKeyUp(Keys.A))
        {
            aPressed = false;
        }

        if (!EnterPressed && Keyboard.GetState().IsKeyDown(Keys.Enter))
        {
            EnterPressed = true;
            _playerTeam.Add(_availablePokemon[_selectedPokemonIndex]);
            _availablePokemon.RemoveAt(_selectedPokemonIndex);
            _selectedPokemonIndex = 0;

            if (_playerTeam.Count == 3)
            {
                _currentGameState = GameState.Map;
                _currentPokemon = _playerTeam[0];
                Console.WriteLine("Команда сформирована!");
            }
        }
        if (Keyboard.GetState().IsKeyUp(Keys.Enter))
        {
            EnterPressed = false;
        }
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

        if (_playMap.IsCellWalkable(newPosition))
        {
            _playerGridPosition = newPosition;
        }
    }

    private void CheckCollisions()
    {
        Cell currentCell = _playMap.GetCell(_playerGridPosition);

        if (currentCell is EnemyCell && !currentCell.isDied)
        {
            StartBattle();
            currentCell.isDied = true;
        }
    }

    private void StartBattle()
    {
        Console.WriteLine("Начался бой!");
        _currentGameState = GameState.Battle;

        // Инициализируем вражеского покемона через BattleManager
        _battleManager.StartBattle(_currentPokemon);  // Теперь BattleManager сам создает врага
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        _spriteBatch.Begin(samplerState: SamplerState.PointClamp);

        switch (_currentGameState)
        {
            case GameState.Map:
                DrawMap();
                break;
            case GameState.Battle:
                DrawBattle();
                break;
            case GameState.ChoosingPokemon:
                DrawChoosingPokemon();
                break;
        }

        _spriteBatch.End();

        base.Draw(gameTime);
    }

    private void DrawMap()
    {
        _playMap.Draw(_spriteBatch);
        _spriteBatch.Draw(_EshSprite.texture, _EshSprite.Rect, Color.White);
    }

    private void DrawBattle()
    {
        _battleManager.Draw(_spriteBatch, _currentPokemon);
    }

    private void DrawChoosingPokemon()
    {
        GraphicsDevice.Clear(Color.LightBlue);

        _spriteBatch.DrawString(_font, "Choose your Pokemon (3 total):", new Vector2(100, 50), Color.Black);

        // Отображаем доступных покемонов
        for (int i = 0; i < _availablePokemon.Count; i++)
        {
            Color color = (i == _selectedPokemonIndex) ? Color.Yellow : Color.White; // Выделяем выбранного

            _spriteBatch.DrawString(_font, _availablePokemon[i].Name, new Vector2(100, 100 + i * 30), color);
        }

        _spriteBatch.DrawString(_font, "Use Left/Right to select, Enter to choose", new Vector2(100, 500), Color.Black);
        _spriteBatch.DrawString(_font, $"Team size: {_playerTeam.Count}/3", new Vector2(100, 530), Color.Black);
    }
}
