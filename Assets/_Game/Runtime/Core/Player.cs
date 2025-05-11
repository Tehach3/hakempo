using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Core
{
    public class Player
    {
        public string Name { get; }
        public int Hp { get; private set; }
        public List<CardType> Deck { get; } = new();
        public List<CardType> Hand { get; } = new();
        public CardType? SelectedCard { get; private set; }

        public Player(string name, int startingHp = 10)
        {
            Name = name; Hp = startingHp;
        }

        public void TakeDamage(int dmg) => Hp = Math.Max(0, Hp - dmg);
        public void Select(CardType card) => SelectedCard = card;
        public void ClearSelection() => SelectedCard = null;
    
    }
    
}

