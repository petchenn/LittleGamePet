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

        // Ссылка на следующую стадию эволюции.  Если null, то это конечная стадия.
        [JsonIgnore] public Pokemon NextEvolution { get; set; }

        // Уровень, на котором происходит эволюция. Если 0, то эволюция невозможна.
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
            NextEvolution = null; // Изначально эволюции нет
            EvolutionLevel = 0; // Изначально не эволюционирует
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
            Debug.WriteLine($"{Name} healing! Health: {Health}");
            Health += amount;
            Health = Math.Min(MaxHealth, Health);
        }

        public void GainExp(int amount)
        {
            Level += amount;
            Debug.WriteLine($"{Name} gained {amount} levels! New level: {Level}");
            // Можно добавить логику для увеличения характеристик
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

            // Копируем характеристики следующей стадии эволюции
            Name = NextEvolution.Name;
            Sprite = NextEvolution.Sprite;
            MaxHealth = NextEvolution.MaxHealth;
            Health = MaxHealth; // Восстанавливаем здоровье до максимума новой формы
            AttackStat = NextEvolution.AttackStat;
            DefenseStat = NextEvolution.DefenseStat;
            Abilities = NextEvolution.Abilities; // Заменяем способности
            EvolutionLevel = NextEvolution.EvolutionLevel;
            NextEvolution = NextEvolution.NextEvolution;


            Debug.WriteLine($"{Name} evolved!");
        }
    }
}