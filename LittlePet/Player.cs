using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LittlePet
{
    public class Player
    {
        //public Vector2 GridPosition { get; set; }
        public int XGridPosition { get; set; }
        public int YGridPosition { get; set; }

        public List<Pokemon> _team { get; set; }

        public int currentPokemonIndex { get; set; }

        public Player()
        {
            currentPokemonIndex = 0;
            _team = new List<Pokemon>();
        }

        public Pokemon CurrentPokemon()
        {
            if (_team.Count > 0 && currentPokemonIndex < _team.Count && currentPokemonIndex >=0) { return _team[currentPokemonIndex]; } else return null;

        }

        public int TeamSize() { return _team.Count; }

        public void AddPokemon(Pokemon pokemon)
        {
            _team.Add(pokemon);
        }

        public void ChangePokemon()
        {
            // Ищем следующего живого покемона
            for (int i = 1; i <= _team.Count; i++)
            {
                int index = (currentPokemonIndex + i) % _team.Count;
                if (_team[index].Health > 0)
                {
                    currentPokemonIndex = index;
                    Debug.WriteLine($"Выбран следующий покемон: {CurrentPokemon().Name}");
                    return;
                }
            }

            // Если не нашли живых покемонов
            currentPokemonIndex = -1;
        }

        public void HealAllPokemon()
        {
            foreach (var pokemon in _team)
            {
                pokemon.Heal(pokemon.MaxHealth);
            }
        }
    }
}
