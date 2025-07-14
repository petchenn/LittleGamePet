using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;

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

    private Player _player;
    private MovingSprite _EshSprite;

    private enum GameState { Map, Battle, ChoosingPokemon, GameOver, PlayerWin, MainMenu }
    private GameState _currentGameState = GameState.MainMenu;

    public List<Pokemon> _availablePokemon = new List<Pokemon>();
    public int selectedPokemonIndex = 0;
    public int selectedField = 0;

    private Texture2D CharmanderTexture, TheCharmanderTexture;
    private Texture2D SquirtleTexture, TheSquirtleTexture;
    private Texture2D BulbasaurTexture, TheBulbasaurTexture;


    private SpriteFont _font;

    private BattleManager _battleManager;
    private bool tabPressed;
    private const string PATHPLAYER = "savePlayer.json";
    private const string PATHMAP = "saveMap.json";

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
        _player = new Player();

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
        _player.XGridPosition = 0;
        _player.YGridPosition = 0;
        _EshSprite.position = _playMap.GetCellPosition(new Vector2(_player.XGridPosition, _player.YGridPosition));

        _font = Content.Load<SpriteFont>("Font");

        CharmanderTexture = Content.Load<Texture2D>("pok1");
        TheCharmanderTexture = Content.Load<Texture2D>("evpok1");
        SquirtleTexture = Content.Load<Texture2D>("pok2");
        TheSquirtleTexture = Content.Load<Texture2D>("evpok2");
        BulbasaurTexture = Content.Load<Texture2D>("pok3");
        TheBulbasaurTexture = Content.Load<Texture2D>("evpok3");

        // Создаем способности через фабричный метод
        var charmanderAbilities = new List<Ability>
        {
            Ability.Create(nameof(AttakAbility), "Ember", PokemonType.fire, 10),
            Ability.Create(nameof(HealingAbility), "Holy Fire", PokemonType.fire, 10)
        };

        var squirtleAbilities = new List<Ability>
        {
            Ability.Create(nameof(AttakAbility), "Water Gun", PokemonType.water, 15),
            Ability.Create(nameof(AttakAbility), "A lot of water", PokemonType.normal, 80)
        };

        var bulbasaurAbilities = new List<Ability>
        {
            Ability.Create(nameof(AttakAbility), "Vine Whip", PokemonType.normal, 10),
            Ability.Create(nameof(VampAbility), "Vamp Whip", PokemonType.normal, 50)
        };

        _availablePokemon.Add(new Pokemon("Charmander", CharmanderTexture, TheCharmanderTexture, 5, 50, charmanderAbilities, PokemonType.fire, 60, 40));
        _availablePokemon.Add(new Pokemon("Squirtle", SquirtleTexture, TheSquirtleTexture, 5, 55, squirtleAbilities, PokemonType.water, 45, 65));
        _availablePokemon.Add(new Pokemon("Bulbasaur", BulbasaurTexture, TheBulbasaurTexture, 5, 60, bulbasaurAbilities, PokemonType.normal, 50, 50));

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
            case GameState.MainMenu:
                UpdateMainMenu();
                break;
            default:
                break;
        }

        base.Update(gameTime);
    }

    private void UpdateMap(GameTime gameTime)
    {
        HandleInput();
        _EshSprite.position = _playMap.GetCellPosition(new Vector2(_player.XGridPosition, _player.YGridPosition));
        CheckCollisions();
    }

    private void UpdateBattle(GameTime gameTime)
    {
        _battleManager.Update(gameTime, _player.CurrentPokemon());

        if (_battleManager.BattleOver)
        {
            if (_battleManager.PlayerWon)
            {
                Debug.WriteLine($"{_battleManager.EnemyPokemon.Name} побежден!");
                _battleManager.EnemyPokemon = null;
                _player.CurrentPokemon().GainExp(10);
                if (_playMap.GetEnemyCount() <= 0) { _currentGameState = GameState.PlayerWin; }
                else _currentGameState = GameState.Map;
            }
            else
            {
                Debug.WriteLine($"{_player.CurrentPokemon().Name} был побежден!");
                _player.ChangePokemon();
                if (_player.CurrentPokemon() == null)
                {
                    _currentGameState = GameState.GameOver;
                }
                else
                {
                    _battleManager.StartBattle(_player.CurrentPokemon());
                }
            }
            _battleManager.BattleOver = false;
        }

        // Обработка ввода в бою (выбор способности, и т.д.)
        if (!wPressed && Keyboard.GetState().IsKeyDown(Keys.D1))
        {
            wPressed = true;
            _battleManager.PlayerAttack(0, _player.CurrentPokemon());
        }
        if (Keyboard.GetState().IsKeyUp(Keys.D1))
        {
            wPressed = false;
        }
        if (!sPressed && Keyboard.GetState().IsKeyDown(Keys.D2) && _player.CurrentPokemon().Abilities.Count > 1)
        {
            sPressed = true;
            Debug.WriteLine("Нажата кнопка 2.");
            _battleManager.PlayerAttack(1, _player.CurrentPokemon());
        }
        if (Keyboard.GetState().IsKeyUp(Keys.D2))
        {
            sPressed = false;
        }
        if (!aPressed && Keyboard.GetState().IsKeyDown(Keys.D3))
        {
            aPressed = true;
            _battleManager.EnemyAttack(_player.CurrentPokemon());
        }
        if (Keyboard.GetState().IsKeyUp(Keys.D3))
        {
            aPressed = false;
        }
    }

    private void UpdateChoosingPokemon(GameTime gameTime)
    {
        if (!dPressed && Keyboard.GetState().IsKeyDown(Keys.D))
        {
            dPressed = true;
            selectedPokemonIndex = (selectedPokemonIndex + 1) % _availablePokemon.Count;
        }
        if (Keyboard.GetState().IsKeyUp(Keys.D))
        {
            dPressed = false;
        }
        if (!aPressed && Keyboard.GetState().IsKeyDown(Keys.A))
        {
            aPressed = true;
            selectedPokemonIndex = (selectedPokemonIndex - 1 + _availablePokemon.Count) % _availablePokemon.Count;
        }
        if (Keyboard.GetState().IsKeyUp(Keys.A))
        {
            aPressed = false;
        }

        if (!EnterPressed && Keyboard.GetState().IsKeyDown(Keys.Enter))
        {
            EnterPressed = true;
            _player.AddPokemon(_availablePokemon[selectedPokemonIndex]);
            _availablePokemon.RemoveAt(selectedPokemonIndex);
            selectedPokemonIndex = 0;

            if (_player.TeamSize() == 3)
            {
                _currentGameState = GameState.Map;
                Debug.WriteLine("Команда сформирована!");
            }
        }
        if (Keyboard.GetState().IsKeyUp(Keys.Enter))
        {
            EnterPressed = false;
        }
    }

    private void UpdateMainMenu()
    {
        if (!dPressed && Keyboard.GetState().IsKeyDown(Keys.D))
        {
            dPressed = true;
            selectedField = (selectedField + 1) % 3;
        }
        if (Keyboard.GetState().IsKeyUp(Keys.D))
        {
            dPressed = false;
        }
        if (!aPressed && Keyboard.GetState().IsKeyDown(Keys.A))
        {
            aPressed = true;
            selectedField = (selectedField - 1 + _availablePokemon.Count) % 3;
        }
        if (Keyboard.GetState().IsKeyUp(Keys.A))
        {
            aPressed = false;
        }

        if (!EnterPressed && Keyboard.GetState().IsKeyDown(Keys.Enter))
        {
            EnterPressed = true;
            switch (selectedField)
            {
                case 0:
                    _currentGameState = GameState.ChoosingPokemon;
                    break;
                case 1:
                    Save(_player, _playMap);
                    break;
                case 2:
                    _player = LoadPlayer();
                    _playMap = LoadMap();
                    _currentGameState = GameState.Map;
                    break;
            }
        }
        if (Keyboard.GetState().IsKeyUp(Keys.Enter))
        {
            EnterPressed = false;
        }

    }

    private void HandleInput()
    {
        Vector2 newPosition = new Vector2(_player.XGridPosition, _player.YGridPosition);

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
            _player.XGridPosition = (int)newPosition.X;
            _player.YGridPosition = (int)newPosition.Y;

        }

        if (!tabPressed && Keyboard.GetState().IsKeyDown(Keys.Tab))
        {
            tabPressed = true;
            _currentGameState = GameState.MainMenu;
        }
        if (Keyboard.GetState().IsKeyUp(Keys.Tab))
        {
            tabPressed = false;
        }
    }

    private void CheckCollisions()
    {
        Cell currentCell = _playMap.GetCell(new Vector2(_player.XGridPosition, _player.YGridPosition));

        if (currentCell is EnemyCell && !currentCell.isDied)
        {
            StartBattle();
            currentCell.isDied = true;
        }
        if (currentCell is HealCell && !currentCell.isUsed)
        {
            _player.HealAllPokemon();
            currentCell.isUsed = true;
        }
    }

    private void StartBattle()
    {
        Debug.WriteLine("Начался бой!");
        _currentGameState = GameState.Battle;
        _battleManager.StartBattle(_player.CurrentPokemon());
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
            case GameState.MainMenu:
                DrawMainMenu();
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
        _battleManager.Draw(_spriteBatch, _player.CurrentPokemon());
    }

    private void DrawChoosingPokemon()
    {
        GraphicsDevice.Clear(Color.LightBlue);

        _spriteBatch.DrawString(_font, "Choose your Pokemon (3 total):", new Vector2(100, 50), Color.Black);

        // Отображаем доступных покемонов
        for (int i = 0; i < _availablePokemon.Count; i++)
        {
            Color color = (i == selectedPokemonIndex) ? Color.Yellow : Color.White; // Выделяем выбранного

            _spriteBatch.DrawString(_font, _availablePokemon[i].Name, new Vector2(100, 100 + i * 30), color);
        }

        _spriteBatch.DrawString(_font, "Use Left/Right to select, Enter to choose", new Vector2(100, 500), Color.Black);
        _spriteBatch.DrawString(_font, $"Team size: {_player.TeamSize()}/3", new Vector2(100, 530), Color.Black);
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

    private void DrawMainMenu()
    {
        GraphicsDevice.Clear(Color.LightBlue);

        _spriteBatch.DrawString(_font, "Welcime to the Pokemon Game!", new Vector2(100, 50), Color.Black);

        
        Color color = (0 == selectedField) ? Color.Yellow : Color.White; // Выделяем выбранного
        _spriteBatch.DrawString(_font, "Start New Game", new Vector2(100, 100 + 0 * 30), color);
        color = (1 == selectedField) ? Color.Yellow : Color.White;
        _spriteBatch.DrawString(_font, "Save", new Vector2(100, 100 + 1 * 30), color);
        color = (2 == selectedField) ? Color.Yellow : Color.White;
        _spriteBatch.DrawString(_font, "Load", new Vector2(100, 100 + 2 * 30), color);


        _spriteBatch.DrawString(_font, "Use Left/Right to select, Enter to choose", new Vector2(100, 500), Color.Black);
    }

    private void Save(Player player, PlayMap playMap)
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            IncludeFields = true
        };

        string serialisedText = JsonSerializer.Serialize<Player>(player, options);
        Trace.WriteLine(serialisedText);
        File.WriteAllText(PATHPLAYER, serialisedText);

        options = new JsonSerializerOptions
        {
            WriteIndented = true,
            Converters = { new Vector2Converter() } // если у вас есть кастомный конвертер
        };

        serialisedText = JsonSerializer.Serialize<PlayMap>(playMap, options);
        Trace.WriteLine(serialisedText);
        File.WriteAllText(PATHMAP, serialisedText);
    }

    private Player LoadPlayer()
    {
        var deserializedData = File.ReadAllText(PATHPLAYER);

        // Настройки десериализации
        var options = new JsonSerializerOptions
        {
            IncludeFields = true
        };

        Player newPlayer = JsonSerializer.Deserialize<Player>(deserializedData, options);

        // Восстанавливаем способности через фабричный метод
        foreach (Pokemon pokemon in newPlayer._team)
        {
            // Восстанавливаем текстуры
            if (pokemon.Name == "Charmander")
            {
                pokemon.evolveTexture = TheCharmanderTexture;
                pokemon.Sprite = new ScaledSprite(CharmanderTexture);
            }
            if(pokemon.Name == "THE Charmander")
            {
                pokemon.Sprite = new ScaledSprite(TheCharmanderTexture);
            }
            if (pokemon.Name == "Squirtle")
            {
                pokemon.evolveTexture = TheSquirtleTexture;
                pokemon.Sprite = new ScaledSprite( SquirtleTexture);
            }
            if(pokemon.Name == "THE Squirtle")
            {
                pokemon.Sprite = new ScaledSprite(TheSquirtleTexture);
            }
            if (pokemon.Name == "Bulbasaur")
            {
                pokemon.evolveTexture = TheBulbasaurTexture;
                pokemon.Sprite = new ScaledSprite(BulbasaurTexture);
            }
            if(pokemon.Name == "THE Bulbasaur")
            {
                pokemon.Sprite = new ScaledSprite(TheBulbasaurTexture);
            }

            // Восстанавливаем способности
            var restoredAbilities = new List<Ability>();
                if (pokemon.Name == "Charmander" || pokemon.Name == "THE Charmander")
                {
                    var newAbility = Ability.Create(nameof(AttakAbility), "Ember", PokemonType.fire, 10);
                    restoredAbilities.Add(newAbility);
                    newAbility = Ability.Create(nameof(HealingAbility), "Holy Fire", PokemonType.fire, 10);
                    restoredAbilities.Add(newAbility);

                }
                if (pokemon.Name == "Squirtle" || pokemon.Name == "THE Squirtle")
                {
                    var newAbility = Ability.Create(nameof(AttakAbility), "Water Gun", PokemonType.water, 15);
                    restoredAbilities.Add(newAbility);
                    newAbility = Ability.Create(nameof(AttakAbility), "A lot of water", PokemonType.normal, 80);
                    restoredAbilities.Add(newAbility);
                }
                if (pokemon.Name == "Bulbasaur" || pokemon.Name == "THE Bulbasaur")
                {
                    var newAbility = Ability.Create(nameof(AttakAbility), "Vine Whip", PokemonType.normal, 10);
                    restoredAbilities.Add(newAbility);
                    newAbility = Ability.Create(nameof(VampAbility), "Vamp Whip", PokemonType.normal, 50);
                    restoredAbilities.Add(newAbility);
                }
            pokemon.Abilities = restoredAbilities;
        }

        return newPlayer;
    }

    private PlayMap LoadMap()
    {
        var options = new JsonSerializerOptions
        {
            Converters = { new Vector2Converter() }
        };
        var deserializedData = File.ReadAllText(PATHMAP);
        PlayMap playMap = JsonSerializer.Deserialize<PlayMap>(deserializedData, options);
        playMap.UpdateTexture(Maptexture, Maptexture, Maptexture);
        return playMap;
    }
}