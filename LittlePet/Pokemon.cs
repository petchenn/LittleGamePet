using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json.Serialization;

namespace LittlePet
{
    public class Pokemon
    {
        public string Name { get; set; }
        public int Level { get; set; }
        public int Health { get; set; }
        public int MaxHealth { get; set; }
        [JsonIgnore] public List<Ability> Abilities { get; set; }
        public PokemonType Type { get; set; }
        public int AttackStat { get; set; }
        public int DefenseStat { get; set; }
        [JsonIgnore] public ScaledSprite Sprite { get; set; }
        [JsonIgnore] public Pokemon NextEvolution { get; set; }
        public int EvolutionLevel { get; set; }

        public Pokemon() { }

        public Pokemon(string name, Texture2D texture, int level, int maxHealth, List<Ability> abilities, PokemonType type, int attackStat, int defenseStat)
        {
            Name = name;
            Level = level;
            Health = maxHealth;
            MaxHealth = maxHealth;
            Abilities = abilities;
            Type = type;
            AttackStat = attackStat;
            DefenseStat = defenseStat;
            Sprite = new ScaledSprite(texture);
            NextEvolution = null;
            EvolutionLevel = 0;
        }

        public void SetEvolution(Pokemon nextEvolution, int evolutionLevel)
        {
            NextEvolution = nextEvolution;
            EvolutionLevel = evolutionLevel;
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
        private float GetTypeModifier(PokemonType attackType, PokemonType targetType)
        {
            if (attackType == PokemonType.fire && targetType == PokemonType.water) return 0.5f;
            if (attackType == PokemonType.water && targetType == PokemonType.fire) return 2.0f;
            return 1.0f; // В остальных случаях нейтральный урон
        }

        public void TakeDamage(int damage)
        {
            Health -= damage;
            Health = Math.Max(0, Health); 
            Debug.WriteLine($"{Name} takes {damage} damage! Health: {Health}");
        }

        public void Heal(int amount)
        {
            Debug.WriteLine($"{Name} healing! Health: {Health}");
            Health += amount;
            Health = Math.Min(MaxHealth, Health);
        }

        public void GainExp(int amount)
        {
            Level += amount;
            Debug.WriteLine($"{Name} gained {amount} levels! New level: {Level}");
            if (NextEvolution != null && Level >= EvolutionLevel)
            {
                Evolve();
            }
        }

        public void Evolve()
        {
            if (NextEvolution == null)
            {
                Debug.WriteLine($"{Name} cannot evolve any further!");
                return;
            }

            Debug.WriteLine($"{Name} is evolving!");

            Name = NextEvolution.Name;
            Sprite = NextEvolution.Sprite;
            MaxHealth = NextEvolution.MaxHealth;
            Health = MaxHealth; 
            AttackStat = NextEvolution.AttackStat;
            DefenseStat = NextEvolution.DefenseStat;
            Abilities = NextEvolution.Abilities;
            EvolutionLevel = NextEvolution.EvolutionLevel;
            NextEvolution = NextEvolution.NextEvolution;


            Debug.WriteLine($"{Name} evolved!");
        }
    }
}