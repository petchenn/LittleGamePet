using System;
using System.Collections.Generic;

namespace LittlePet
{
    class Pokemon
    {
        public string Name { get; set; }
        public int Level { get; set; }
        public int Health { get; set; }
        public int MaxHealth { get; set; }
        public List<Ability> Abilities { get; set; }
        public PokemonType Type { get; set; } // Добавляем тип покемона
        public int AttackStat { get; set; } // Добавляем стат атаки
        public int DefenseStat { get; set; } // Добавляем стат защиты

        public Pokemon(string name, int level, int maxHealth, List<Ability> abilities, PokemonType type, int attackStat, int defenseStat)
        {
            Name = name;
            Level = level;
            Health = maxHealth;
            MaxHealth = maxHealth;
            Abilities = abilities;
            Type = type;
            AttackStat = attackStat;
            DefenseStat = defenseStat;
        }

        public void Attack(Pokemon target, Ability ability)
        {
            Console.WriteLine($"{Name} uses {ability.Name}!"); // Вывод в консоль (замените на отображение в игре)

            // Урон учитывает атаку, защиту и силу способности
            int damage = CalculateDamage(target, ability);
            target.TakeDamage(damage);
        }

        private int CalculateDamage(Pokemon target, Ability ability)
        {
            // Простая формула урона (нужно балансировать)
            float typeModifier = GetTypeModifier(ability.Type, target.Type);
            int damage = (int)(((2 * Level / 5.0 + 2) * ability.Power * AttackStat / target.DefenseStat / 50 + 2) * typeModifier);
            return Math.Max(1, damage); // Минимум 1 урона
        }

        // Таблица эффективности типов (пример)
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
            Console.WriteLine($"{Name} takes {damage} damage! Health: {Health}"); // Вывод в консоль
        }

        public void Heal(int amount)
        {
            Health += amount;
            Health = Math.Min(MaxHealth, Health); // Здоровье не может быть больше MaxHealth
        }

        public void GainExp(int amount)
        {
            // Простая реализация опыта (доработать)
            Level += amount;
            Console.WriteLine($"{Name} gained {amount} levels! New level: {Level}");
            // Можно добавить логику для увеличения характеристик при повышении уровня
        }

        public void Evolve()
        {
            // Реализация эволюции (зависит от конкретных покемонов)
            Console.WriteLine($"{Name} is trying to evolve!");
            // Добавьте здесь логику эволюции (смена имени, характеристик, спрайта и т.д.)
        }
    }
}