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
    class Ability
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
    }
}