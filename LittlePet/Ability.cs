using System;
using System.Collections.Generic;

namespace LittlePet
{
    public enum PokemonType
    {
        fire, water, normal
    }

    public abstract class Ability
    {
        public string Name { get; set; }
        public PokemonType Type { get; set; }
        public int Power { get; set; }

        protected Ability() { }

        protected Ability(string name, PokemonType type, int power)
        {
            Name = name;
            Type = type;
            Power = power;
        }

        public abstract void UsingAbility(Pokemon pokemon, Pokemon enemy);

        // Фабричный метод для создания способностей
        public static Ability Create(string abilityType, string name, PokemonType type, int power)
        {
            return abilityType switch
            {
                nameof(HealingAbility) => new HealingAbility(name, type, power),
                nameof(VampAbility) => new VampAbility(name, type, power),
                nameof(AttakAbility) => new AttakAbility(name, type, power),
                _ => throw new ArgumentException($"Unknown ability type: {abilityType}")
            };
        }
    }

    public class HealingAbility : Ability
    {
        public HealingAbility() { }
        public HealingAbility(string name, PokemonType type, int power) : base(name, type, power) { }

        public override void UsingAbility(Pokemon pokemon, Pokemon enemy)
        {
            pokemon.Heal(Power);
        }
    }

    public class VampAbility : Ability
    {
        public VampAbility() { }
        public VampAbility(string name, PokemonType type, int power) : base(name, type, power) { }

        public override void UsingAbility(Pokemon pokemon, Pokemon enemy)
        {
            pokemon.Heal(Power / 2);
            enemy.TakeDamage(Power / 2);
        }
    }

    public class AttakAbility : Ability
    {
        public AttakAbility() { }
        public AttakAbility(string name, PokemonType type, int power) : base(name, type, power) { }

        public override void UsingAbility(Pokemon pokemon, Pokemon enemy)
        {
            enemy.TakeDamage(Power);
        }
    }
}