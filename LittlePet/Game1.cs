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

    private const int DefaultGridWidth = 6;
    private const int DefaultGridHeight = 6;
    private int _gridWidth = DefaultGridWidth;
    private int _gridHeight = DefaultGridHeight;
    private const int CellSize = 64;

    bool dPressed = false;
    bool aPressed = false;
    bool sPressed = false;
    bool wPressed = false;
    bool EnterPressed = false;

    private Player _player;
    private MovingSprite _EshSprite;

    private enum GameState { Map, Battle, ChoosingPokemon, GameOver, PlayerWin, MainMenu, ChoosingMapSize }
    private GameState _currentGameState = GameState.MainMenu;

    public List<Pokemon> _availablePokemon = new List<Pokemon>();
    public int selectedPokemonIndex = 0;
    public int selectedField = 0;
    public int selectedMapSizeIndex = 0;

    private Texture2D CharmanderTexture, CharmeleonTexture, CharizardTexture;
    private Texture2D SquirtleTexture, WartortleTexture, BlastoiseTexture;
    private Texture2D BulbasaurTexture, IvysaurTexture, VenusaurTexture;

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

        _font = Content.Load<SpriteFont>("Font");

        //Load Pokemon Textures
        CharmanderTexture = Content.Load<Texture2D>("pok1");
        CharmeleonTexture = Content.Load<Texture2D>("evpok1");
        CharizardTexture = CharmeleonTexture;
        SquirtleTexture = Content.Load<Texture2D>("pok2");
        WartortleTexture = Content.Load<Texture2D>("evpok2");
        BlastoiseTexture = WartortleTexture;
        BulbasaurTexture = Content.Load<Texture2D>("pok3");
        IvysaurTexture = Content.Load<Texture2D>("evpok3");
        VenusaurTexture = IvysaurTexture;

        CreatePlayersPokemons();

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
            case GameState.ChoosingMapSize:
                UpdateChoosingMapSize();
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
                _player.XGridPosition = 0;
                _player.YGridPosition = 0;
                _EshSprite.position = _playMap.GetCellPosition(new Vector2(_player.XGridPosition, _player.YGridPosition));
                _currentGameState = GameState.Map;
                Debug.WriteLine("Команда сформирована!");
            }
        }
        if (Keyboard.GetState().IsKeyUp(Keys.Enter))
        {
            EnterPressed = false;
        }
    }

    private void CreatePlayersPokemons()
    {
        // Create abilities
        var charmanderAbilities = new List<Ability>
        {
            Ability.Create(nameof(AttakAbility), "Ember", PokemonType.fire, 10),
            Ability.Create(nameof(HealingAbility), "Scratch", PokemonType.normal, 5)
        };

        var charmeleonAbilities = new List<Ability>
        {
            Ability.Create(nameof(AttakAbility), "Ember", PokemonType.fire, 20),
            Ability.Create(nameof(HealingAbility), "Scratch", PokemonType.normal, 10)
        };

        var charizardAbilities = new List<Ability>
        {
            Ability.Create(nameof(AttakAbility), "Flamethrower", PokemonType.fire, 40),
            Ability.Create(nameof(HealingAbility), "Fly", PokemonType.flying, 20)
        };

        var squirtleAbilities = new List<Ability>
        {
            Ability.Create(nameof(AttakAbility), "Water Gun", PokemonType.water, 15),
            Ability.Create(nameof(HealingAbility), "Tackle", PokemonType.normal, 5)
        };

        var wartortleAbilities = new List<Ability>
        {
            Ability.Create(nameof(AttakAbility), "Water Gun", PokemonType.water, 25),
            Ability.Create(nameof(HealingAbility), "Tackle", PokemonType.normal, 10)
        };

        var blastoiseAbilities = new List<Ability>
        {
            Ability.Create(nameof(AttakAbility), "Hydro Pump", PokemonType.water, 50),
            Ability.Create(nameof(HealingAbility), "Protect", PokemonType.normal, 20)
        };

        var bulbasaurAbilities = new List<Ability>
        {
            Ability.Create(nameof(AttakAbility), "Vine Whip", PokemonType.grass, 10),
            Ability.Create(nameof(VampAbility), "Leech Seed", PokemonType.grass, 10)
        };

        var ivysaurAbilities = new List<Ability>
        {
            Ability.Create(nameof(AttakAbility), "Razor Leaf", PokemonType.grass, 20),
            Ability.Create(nameof(VampAbility), "Leech Seed", PokemonType.grass, 20)
        };

        var venusaurAbilities = new List<Ability>
        {
            Ability.Create(nameof(AttakAbility), "Solar Beam", PokemonType.grass, 50),
            Ability.Create(nameof(VampAbility), "Leech Seed", PokemonType.grass, 30)
        };

        // Создаем базовую форму покемона
        Pokemon charmander = new Pokemon("Charmander", CharmanderTexture, 5, 50, charmanderAbilities, PokemonType.fire, 60, 40);
        Pokemon charmeleon = new Pokemon("Charmeleon", CharmeleonTexture, 16, 80, charmeleonAbilities, PokemonType.fire, 80, 60);
        Pokemon charizard = new Pokemon("Charizard", CharizardTexture, 36, 120, charizardAbilities, PokemonType.fire, 120, 80);
        charmander.SetEvolution(charmeleon, 16);
        charmeleon.SetEvolution(charizard, 36);
        _availablePokemon.Add(charmander);


        Pokemon squirtle = new Pokemon("Squirtle", SquirtleTexture, 5, 50, squirtleAbilities, PokemonType.water, 45, 65);
        Pokemon wartortle = new Pokemon("Wartortle", WartortleTexture, 16, 80, wartortleAbilities, PokemonType.water, 65, 80);
        Pokemon blastoise = new Pokemon("Blastoise", BlastoiseTexture, 36, 120, blastoiseAbilities, PokemonType.water, 80, 120);
        squirtle.SetEvolution(wartortle, 16);
        wartortle.SetEvolution(blastoise, 36);
        _availablePokemon.Add(squirtle);

        Pokemon bulbasaur = new Pokemon("Bulbasaur", BulbasaurTexture, 5, 60, bulbasaurAbilities, PokemonType.grass, 50, 50);
        Pokemon ivysaur = new Pokemon("Ivysaur", IvysaurTexture, 16, 90, ivysaurAbilities, PokemonType.grass, 70, 70);
        Pokemon venusaur = new Pokemon("Venusaur", VenusaurTexture, 36, 130, venusaurAbilities, PokemonType.grass, 120, 100);
        bulbasaur.SetEvolution(ivysaur, 16);
        ivysaur.SetEvolution(venusaur, 36);
        _availablePokemon.Add(bulbasaur);
    }

    private void UpdateChoosingMapSize()
    {
        if (!dPressed && Keyboard.GetState().IsKeyDown(Keys.D))
        {
            dPressed = true;
            selectedMapSizeIndex = (selectedMapSizeIndex + 1) % 3;
        }
        if (Keyboard.GetState().IsKeyUp(Keys.D))
        {
            dPressed = false;
        }
        if (!aPressed && Keyboard.GetState().IsKeyDown(Keys.A))
        {
            aPressed = true;
            selectedMapSizeIndex = (selectedMapSizeIndex - 1 + 3) % 3;
        }
        if (Keyboard.GetState().IsKeyUp(Keys.A))
        {
            aPressed = false;
        }

        if (!EnterPressed && Keyboard.GetState().IsKeyDown(Keys.Enter))
        {
            EnterPressed = true;

            switch (selectedMapSizeIndex)
            {
                case 0: // Small
                    _gridWidth = 6;
                    _gridHeight = 6;
                    break;
                case 1: // Medium
                    _gridWidth = 8;
                    _gridHeight = 8;
                    break;
                case 2: // Large
                    _gridWidth = 10;
                    _gridHeight = 10;
                    break;
            }

            _playMap = CreateMap(_gridWidth, _gridHeight, selectedMapSizeIndex);
            _player.XGridPosition = 0;
            _player.YGridPosition = 0;
            _EshSprite.position = _playMap.GetCellPosition(new Vector2(_player.XGridPosition, _player.YGridPosition));
            _currentGameState = GameState.ChoosingPokemon;
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
                    _player = new Player();
                    CreatePlayersPokemons();
                    _currentGameState = GameState.ChoosingMapSize;
                    break;
                case 1:
                    Save(_player, _playMap);
                    break;
                case 2:
                    _player = LoadPlayer();
                    _playMap = LoadMap();
                    _EshSprite.position = _playMap.GetCellPosition(new Vector2(_player.XGridPosition, _player.YGridPosition));
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
            case GameState.ChoosingMapSize:
                DrawChoosingMapSize();
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
        GraphicsDevice.Clear(Color.BurlyWood);

        _spriteBatch.DrawString(_font, "Choose your Pokemon (3 total):", new Vector2(100, 50), Color.Black);

        for (int i = 0; i < _availablePokemon.Count; i++)
        {
            Color color = (i == selectedPokemonIndex) ? Color.Red : Color.Black; // Выделяем выбранного

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
        GraphicsDevice.Clear(Color.BurlyWood);

        _spriteBatch.DrawString(_font, "Welcime to the Pokemon Game!", new Vector2(100, 50), Color.Black);


        Color color = (0 == selectedField) ? Color.Red : Color.Black;
        _spriteBatch.DrawString(_font, "Start New Game", new Vector2(100, 100 + 0 * 30), color);
        color = (1 == selectedField) ? Color.Red : Color.Black;
        _spriteBatch.DrawString(_font, "Save", new Vector2(100, 100 + 1 * 30), color);
        color = (2 == selectedField) ? Color.Red : Color.Black;
        _spriteBatch.DrawString(_font, "Load", new Vector2(100, 100 + 2 * 30), color);


        _spriteBatch.DrawString(_font, "Use Left/Right to select, Enter to choose", new Vector2(100, 500), Color.Black);
    }

    private void DrawChoosingMapSize()
    {
        GraphicsDevice.Clear(Color.BurlyWood);

        _spriteBatch.DrawString(_font, "Choose map size:", new Vector2(100, 50), Color.Black);

        string[] sizes = { "Small", "Medium", "Large" };

        for (int i = 0; i < sizes.Length; i++)
        {
            Color color = (i == selectedMapSizeIndex) ? Color.Red : Color.Black;
            _spriteBatch.DrawString(_font, sizes[i], new Vector2(100, 100 + i * 30), color);
        }

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
            Converters = { new Vector2Converter() }
        };

        serialisedText = JsonSerializer.Serialize<PlayMap>(playMap, options);
        Trace.WriteLine(serialisedText);
        File.WriteAllText(PATHMAP, serialisedText);
    }

    private Player LoadPlayer()
    {
        var deserializedData = File.ReadAllText(PATHPLAYER);

        var options = new JsonSerializerOptions
        {
            IncludeFields = true
        };

        Player newPlayer = JsonSerializer.Deserialize<Player>(deserializedData, options);

        foreach (Pokemon pokemon in newPlayer._team)
        {
            if (pokemon.Name == "Charmander")
            {
                pokemon.Sprite = new ScaledSprite(CharmanderTexture);
            }
            if (pokemon.Name == "Charmeleon")
            {
                pokemon.Sprite = new ScaledSprite(CharmeleonTexture);
            }
            if (pokemon.Name == "Charizard")
            {
                pokemon.Sprite = new ScaledSprite(CharizardTexture);
            }
            if (pokemon.Name == "Squirtle")
            {
                pokemon.Sprite = new ScaledSprite(SquirtleTexture);
            }
            if (pokemon.Name == "Wartortle")
            {
                pokemon.Sprite = new ScaledSprite(WartortleTexture);
            }
            if (pokemon.Name == "Blastoise")
            {
                pokemon.Sprite = new ScaledSprite(BlastoiseTexture);
            }
            if (pokemon.Name == "Bulbasaur")
            {
                pokemon.Sprite = new ScaledSprite(BulbasaurTexture);
            }
            if (pokemon.Name == "Ivysaur")
            {
                pokemon.Sprite = new ScaledSprite(IvysaurTexture);
            }
            if (pokemon.Name == "Venusaur")
            {
                pokemon.Sprite = new ScaledSprite(VenusaurTexture);
            }
            // Восстанавливаем способности
            var restoredAbilities = new List<Ability>();
            if (pokemon.Name == "Charmander")
            {
                restoredAbilities = new List<Ability>
                {
                    Ability.Create(nameof(AttakAbility), "Ember", PokemonType.fire, 10),
                    Ability.Create(nameof(HealingAbility), "Scratch", PokemonType.normal, 5)
                };
            }
            if (pokemon.Name == "Charmeleon")
            {
                restoredAbilities = new List<Ability>
                {
                    Ability.Create(nameof(AttakAbility), "Ember", PokemonType.fire, 20),
                    Ability.Create(nameof(HealingAbility), "Scratch", PokemonType.normal, 10)
                };
            }
            if (pokemon.Name == "Charizard")
            {
                restoredAbilities = new List<Ability>
                {
                    Ability.Create(nameof(AttakAbility), "Flamethrower", PokemonType.fire, 40),
                    Ability.Create(nameof(HealingAbility), "Fly", PokemonType.flying, 20)
                };
            }
            if (pokemon.Name == "Squirtle")
            {
                restoredAbilities = new List<Ability>
                {
                    Ability.Create(nameof(AttakAbility), "Water Gun", PokemonType.water, 15),
                    Ability.Create(nameof(HealingAbility), "Tackle", PokemonType.normal, 5)
                };
            }
            if (pokemon.Name == "Wartortle")
            {
                restoredAbilities = new List<Ability>
                {
                    Ability.Create(nameof(AttakAbility), "Water Gun", PokemonType.water, 25),
                    Ability.Create(nameof(HealingAbility), "Tackle", PokemonType.normal, 10)
                };
            }
            if (pokemon.Name == "Blastoise")
            {
                restoredAbilities = new List<Ability>
                {
                    Ability.Create(nameof(AttakAbility), "Hydro Pump", PokemonType.water, 50),
                    Ability.Create(nameof(HealingAbility), "Protect", PokemonType.normal, 20)
                };
            }
            if (pokemon.Name == "Bulbasaur")
            {
                restoredAbilities = new List<Ability>
                {
                     Ability.Create(nameof(AttakAbility), "Vine Whip", PokemonType.grass, 10),
                     Ability.Create(nameof(VampAbility), "Leech Seed", PokemonType.grass, 10)
                };
            }
            if (pokemon.Name == "Ivysaur")
            {
                restoredAbilities = new List<Ability>
                {
                    Ability.Create(nameof(AttakAbility), "Razor Leaf", PokemonType.grass, 20),
                    Ability.Create(nameof(VampAbility), "Leech Seed", PokemonType.grass, 20)
                };
            }
            if (pokemon.Name == "Venusaur")
            {
                restoredAbilities = new List<Ability>
                {
                    Ability.Create(nameof(AttakAbility), "Solar Beam", PokemonType.grass, 50),
                    Ability.Create(nameof(VampAbility), "Leech Seed", PokemonType.grass, 30)
                };
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

    private PlayMap CreateMap(int width, int height, int sizeIndex)
    {
        IMapGenerator generator;

        switch (sizeIndex)
        {
            case 0:
                generator = new SmallMapGenerator();
                break;
            case 1:
                generator = new MediumMapGenerator();
                break;
            case 2:
                generator = new LargeMapGenerator();
                break;
            default:
                generator = new SmallMapGenerator();
                break;
        }

        return generator.GenerateMap(width, height, CellSize, Maptexture, Maptexture, Maptexture, Maptexture);
    }
}