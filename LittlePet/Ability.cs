using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public Ability(string name, PokemonType type, int power)
        {
            Name = name;
            Type = type;
            Power = power;
        }
        public abstract void UsingAbility(Pokemon pokemon, Pokemon enemy);
    }
    public class HealingAbility : Ability
    {
        public HealingAbility(string name, PokemonType type, int power) : base(name, type, power)
        {
        }
        public override void UsingAbility(Pokemon pokemon, Pokemon enemy)
        {
            pokemon.Heal(Power);
        }
    }
    public class VampAbility : Ability
    {
        public VampAbility(string name, PokemonType type, int power) : base(name, type, power)
        {
        }
        public override void UsingAbility(Pokemon pokemon, Pokemon enemy)
        {
            pokemon.Heal((int)Power / 2);
            enemy.TakeDamage((int)Power / 2);
        }
    }
    public class AttakAbility : Ability
    {
        public AttakAbility(string name, PokemonType type, int power) : base(name, type, power)
        {
        }
        public override void UsingAbility(Pokemon pokemon, Pokemon enemy)
        {
            enemy.TakeDamage(Power);
        }
    }


}