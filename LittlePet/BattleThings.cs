
using LittlePet;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System;

public class BattleManager
{
    private ContentManager _content;
    private SpriteBatch _spriteBatch;
    private SpriteFont _font;
    private Texture2D _playerTexture;
    private Texture2D _enemyTexture;

    public Pokemon EnemyPokemon { get; set; }
    public bool BattleOver { get; set; }
    public bool PlayerWon { get; set; }

    public BattleManager(ContentManager content, SpriteBatch spriteBatch, SpriteFont font, Texture2D playerTexture, Texture2D enemyTexture)
    {
        _content = content;
        _spriteBatch = spriteBatch;
        _font = font;
        _playerTexture = playerTexture;
        _enemyTexture = enemyTexture;
        BattleOver = false;
        PlayerWon = false;
    }

    public void StartBattle(Pokemon playerPokemon)
    {

        EnemyPokemon = new Pokemon("Wild Pidgey", _enemyTexture, _enemyTexture, 3, 30, new List<Ability>() { new Ability("Tackle", PokemonType.normal, 30) }, PokemonType.normal, 30, 20);
        Console.WriteLine($"Вы напали на {EnemyPokemon.Name}!");
        BattleOver = false;
        PlayerWon = false;
    }

    public void Update(GameTime gameTime, Pokemon playerPokemon)
    {
    }


    public void PlayerAttack(int abilityIndex, Pokemon playerPokemon)
    {
        if (playerPokemon.Health <= 0 || EnemyPokemon.Health <= 0) return;

        Ability ability = playerPokemon.Abilities[abilityIndex];
        playerPokemon.Attack(EnemyPokemon, ability);

        if (EnemyPokemon.Health <= 0)
        {
            Console.WriteLine($"{EnemyPokemon.Name} побежден!");
            BattleOver = true;
            PlayerWon = true;
            return;
        }

        EnemyAttack(playerPokemon);
    }

    public void EnemyAttack(Pokemon playerPokemon)
    {
        if (EnemyPokemon.Health <= 0) return;

        EnemyPokemon.Attack(playerPokemon, EnemyPokemon.Abilities[0]);

        if (playerPokemon.Health <= 0)
        {
            Console.WriteLine($"{playerPokemon.Name} был побежден!");
            BattleOver = true;
            PlayerWon = false;
        }
    }

    public void Draw(SpriteBatch spriteBatch, Pokemon currentPokemon)
    {
        GraphicsDevice graphicsDevice = spriteBatch.GraphicsDevice;
        graphicsDevice.Clear(Color.Gray);

        // Отображаем спрайты покемонов (позиции нужно настроить)
        currentPokemon.Sprite.setPocition(new Vector2(100, 300));
        _spriteBatch.Draw(currentPokemon.Sprite.texture, currentPokemon.Sprite.Rect, Color.White); // Спрайт игрока
        EnemyPokemon.Sprite.setPocition(new Vector2(500, 100));
        _spriteBatch.Draw(EnemyPokemon.Sprite.texture, EnemyPokemon.Sprite.Rect, Color.White); // Спрайт врага

        // Отображаем имена и здоровье
        _spriteBatch.DrawString(_font, $"{currentPokemon.Name} (Lv.{currentPokemon.Level})", new Vector2(100, 250), Color.White);
        _spriteBatch.DrawString(_font, $"HP: {currentPokemon.Health}/{currentPokemon.MaxHealth}", new Vector2(100, 270), Color.White);

        _spriteBatch.DrawString(_font, $"{EnemyPokemon.Name} (Lv.{EnemyPokemon.Level})", new Vector2(500, 50), Color.White);
        _spriteBatch.DrawString(_font, $"HP: {EnemyPokemon.Health}/{EnemyPokemon.MaxHealth}", new Vector2(500, 70), Color.White);

        // Отображаем доступные способности
        for (int i = 0; i < currentPokemon.Abilities.Count; i++)
        {
            _spriteBatch.DrawString(_font, $"{i + 1}: {currentPokemon.Abilities[i].Name} ({currentPokemon.Abilities[i].Type})", new Vector2(100, 400 + i * 20), Color.White);
        }

        // Инструкции
        _spriteBatch.DrawString(_font, "Press 1 or 2 to attack, 3 to do nothing", new Vector2(100, 500), Color.White);
    }
}