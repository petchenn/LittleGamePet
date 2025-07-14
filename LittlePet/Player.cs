using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LittlePet
{
    public class Player
    {
        public Vector2 GridPosition { get; set; }
        public List<Pokemon> _team = new List<Pokemon>();
        public int _currentPokemonIndex = 0;

        public Pokemon CurrentPokemon => _team.Count > 0 ? _team[_currentPokemonIndex] : null;
        public int TeamSize => _team.Count;

        public void AddPokemon(Pokemon pokemon)
        {
            _team.Add(pokemon);
        }

        public void ChangePokemon()
        {
            // Ищем следующего живого покемона
            for (int i = 1; i <= _team.Count; i++)
            {
                int index = (_currentPokemonIndex + i) % _team.Count;
                if (_team[index].Health > 0)
                {
                    _currentPokemonIndex = index;
                    Debug.WriteLine($"Выбран следующий покемон: {CurrentPokemon.Name}");
                    return;
                }
            }

            // Если не нашли живых покемонов
            _currentPokemonIndex = -1;
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
