using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

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
    private int _currentPokemonIndex = 0;
    private Pokemon CurrentPokemon => _playerTeam[_currentPokemonIndex];

    private enum GameState { Map, Battle, ChoosingPokemon, GameOver, PlayerWin }
    private GameState _currentGameState = GameState.ChoosingPokemon;

    private List<Pokemon> _availablePokemon = new List<Pokemon>();
    private int _selectedPokemonIndex = 0;

    private SpriteFont _font;

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

        _availablePokemon.Add(new Pokemon("Charmander", Content.Load<Texture2D>("pok1"), Content.Load<Texture2D>("evpok1"), 5, 50, new List<Ability>() { new AttakAbility("Ember", PokemonType.fire, 10), new HealingAbility("Holy Fire", PokemonType.fire, 10) }, PokemonType.fire, 60, 40));
        _availablePokemon.Add(new Pokemon("Squirtle", Content.Load<Texture2D>("pok2"), Content.Load<Texture2D>("evpok2"), 5, 55, new List<Ability>() { new AttakAbility("Water Gun", PokemonType.water, 15), new AttakAbility("A lot of water", PokemonType.normal, 80) }, PokemonType.water, 45, 65));
        _availablePokemon.Add(new Pokemon("Bulbasaur", Content.Load<Texture2D>("pok3"), Content.Load<Texture2D>("evpok3"), 5, 60, new List<Ability>() { new AttakAbility("Vine Whip", PokemonType.normal, 10), new VampAbility("Vamp Whip", PokemonType.normal, 50) }, PokemonType.normal, 50, 50));

        _battleManager = new BattleManager(Content, _spriteBatch, _font, PlayerTexture, EnemyTexture);
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
            case GameState.GameOver:
                break;
            case GameState.PlayerWin:
                break;
            default:
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
        _battleManager.Update(gameTime, CurrentPokemon);

        if (_battleManager.BattleOver)
        {
            if (_battleManager.PlayerWon)
            {
                Debug.WriteLine($"{_battleManager.EnemyPokemon.Name} побежден!");
                _battleManager.EnemyPokemon = null;
                CurrentPokemon.GainExp(10);
                if(_playMap.GetEnemyCount() <= 0) { _currentGameState = GameState.PlayerWin; }
                else _currentGameState = GameState.Map;
            }
            else
            {
                Debug.WriteLine($"{CurrentPokemon.Name} был побежден!");
                ChangePokemon();
            }
            _battleManager.BattleOver = false;
        }

        // Обработка ввода в бою (выбор способности, и т.д.)
        if (!wPressed && Keyboard.GetState().IsKeyDown(Keys.D1))
        {
            wPressed = true;
            _battleManager.PlayerAttack(0, CurrentPokemon);
        }
        if (Keyboard.GetState().IsKeyUp(Keys.D1))
        {
            wPressed = false;
        }
        if (!sPressed && Keyboard.GetState().IsKeyDown(Keys.D2) && CurrentPokemon.Abilities.Count > 1)
        {
            sPressed = true;
            Debug.WriteLine("Нажата кнопка 2.");
            _battleManager.PlayerAttack(1, CurrentPokemon);
        }
        if (Keyboard.GetState().IsKeyUp(Keys.D2))
        {
            sPressed = false;
        }
        if (!aPressed && Keyboard.GetState().IsKeyDown(Keys.D3))
        {
            aPressed = true;
            _battleManager.EnemyAttack(CurrentPokemon);
        }
        if (Keyboard.GetState().IsKeyUp(Keys.D3))
        {
            aPressed = false;
        }
    }

    private void ChangePokemon()
    {
        // Ищем следующего живого покемона
        int nextPokemonIndex = -1;
        for (int i = 0; i < _playerTeam.Count; i++)
        {
            int index = (_currentPokemonIndex + i + 1) % _playerTeam.Count; // Перебираем по кругу
            if (_playerTeam[index].Health > 0)
            {
                nextPokemonIndex = index;
                break;
            }
        }

        if (nextPokemonIndex != -1)
        {
            _currentPokemonIndex = nextPokemonIndex;
            Console.WriteLine($"Выбран следующий покемон: {CurrentPokemon.Name}");
            //_currentGameState = GameState.Map; // Возвращаемся на карту, если есть живые покемоны
            _battleManager.StartBattle(CurrentPokemon); // Продолжаем бой новым покемоном

        }
        else
        {
            Debug.WriteLine("Все покемоны в команде мертвы. Game Over!");
            _currentGameState = GameState.GameOver;
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
                //_currentPokemon = _playerTeam[0]; //Инициализация текущего покемона перенесена в свойство CurrentPokemon
                Debug.WriteLine("Команда сформирована!");
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
        if (currentCell is HealCell && !currentCell.isUsed)
        {
            foreach (Pokemon pok in _playerTeam)
            {
                pok.Heal(pok.MaxHealth);
            }
            currentCell.isUsed = true;
        }
    }

    private void StartBattle()
    {
        Debug.WriteLine("Начался бой!");
        _currentGameState = GameState.Battle;

        _battleManager.StartBattle(CurrentPokemon);
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
            case GameState.GameOver:
                DrawGameOver();
                break;
            case GameState.PlayerWin:
                    DrawPlayerWin();
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
        _battleManager.Draw(_spriteBatch, CurrentPokemon);
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

    private void DrawGameOver()
    {
        GraphicsDevice.Clear(Color.Black);
        _spriteBatch.DrawString(_font, "Game Over! All your Pokemon fainted.", new Vector2(100, 100), Color.White);
        _spriteBatch.DrawString(_font, "Press Esc to exit.", new Vector2(100, 150), Color.White);
    }

    private void DrawPlayerWin()
    {
        GraphicsDevice.Clear(Color.BurlyWood);
        _spriteBatch.DrawString(_font, "You are win! You defided all enemies.", new Vector2(100, 100), Color.Black);
        _spriteBatch.DrawString(_font, "Press Esc to exit.", new Vector2(100, 150), Color.Black);

    }
}