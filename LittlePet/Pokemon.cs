using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using static System.Net.Mime.MediaTypeNames;

namespace LittlePet
{
    public class Pokemon
    {
        public string Name { get; set; }
        public int Level { get; set; }
        public int Health { get; set; }
        public int MaxHealth { get; set; }
        public List<Ability> Abilities { get; set; }
        public PokemonType Type { get; set; }
        public int AttackStat { get; set; }
        public int DefenseStat { get; set; }
        public ScaledSprite Sprite { get; set; }
        public Texture2D evolveTexture { get; set; }

        public Pokemon(string name, Texture2D textureFirst, Texture2D textureEvolve, int level, int maxHealth, List<Ability> abilities, PokemonType type, int attackStat, int defenseStat)
        {
            Name = name;
            Level = level;
            Health = maxHealth;
            MaxHealth = maxHealth;
            Abilities = abilities;
            Type = type;
            AttackStat = attackStat;
            DefenseStat = defenseStat;
            Sprite = new ScaledSprite(textureFirst);
            evolveTexture = textureEvolve;
        }

        public void Attack(Pokemon target, Ability ability)
        {
            Debug.WriteLine($"{Name} uses {ability.Name}!");

            int damage = CalculateDamage(target, ability);
            target.TakeDamage(damage);
        }

        private int CalculateDamage(Pokemon target, Ability ability)
        {
            float typeModifier = GetTypeModifier(ability.Type, target.Type);
            int damage = (int)(((2 * Level / 5.0 + 2) * ability.Power * AttackStat / target.DefenseStat / 50 + 2) * typeModifier);
            return Math.Max(1, damage); // Минимум 1 урона
        }

        // Таблица эффективности типов
        private float GetTypeModifier(PokemonType attackType, PokemonType targetType)
        {
            if (attackType == PokemonType.fire && targetType == PokemonType.water) return 0.5f;
            if (attackType == PokemonType.water && targetType == PokemonType.fire) return 2.0f;
            return 1.0f; // В остальных случаях нейтральный урон
        }

        public void TakeDamage(int damage)
        {
            Health -= damage;
            Health = Math.Max(0, Health); // Здоровье не может быть меньше 0
            Debug.WriteLine($"{Name} takes {damage} damage! Health: {Health}"); // Вывод в консоль
        }

        public void Heal(int amount)
        {
            Debug.WriteLine($"{Name} healing! Health: {Health}"); // Вывод в консоль
            Health += amount;
            Health = Math.Min(MaxHealth, Health);
        }

        public void GainExp(int amount)
        {
            Level += amount;
            Debug.WriteLine($"{Name} gained {amount} levels! New level: {Level}");
            // Можно добавить логику для увеличения характеристик
            if (Level >= 10) Evolve();
        }

        public void Evolve()
        {
            Debug.WriteLine($"{Name} is trying to evolve!");
            Sprite.texture = evolveTexture;
            Name = "THE " + Name;
            MaxHealth *= 2;
            Health = MaxHealth;
            AttackStat *= 2;
            DefenseStat *= 2;
            foreach(Ability ability in Abilities)
            {
                ability.Power += 20;
            }
        }
    }
}