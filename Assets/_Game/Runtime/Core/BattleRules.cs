using System;
using System.Collections.Generic;

namespace Game.Core
{
    public enum CardType   { Knight, Wizard, Assassin, Archer, Pikeman }
    public enum BattleResult { Win, Lose, Draw }
    
    public static class BattleRules
    {
        private static readonly Dictionary<(CardType, CardType), BattleResult> Map =
            new Dictionary<(CardType, CardType), BattleResult>
        {
            // Knight
            { (CardType.Knight,  CardType.Knight),   BattleResult.Draw },
            { (CardType.Knight,  CardType.Wizard),   BattleResult.Lose },
            { (CardType.Knight,  CardType.Assassin), BattleResult.Win  },
            { (CardType.Knight,  CardType.Archer),   BattleResult.Win  },
            { (CardType.Knight,  CardType.Pikeman),  BattleResult.Lose },

            // Wizard
            { (CardType.Wizard,  CardType.Knight),   BattleResult.Win  },
            { (CardType.Wizard,  CardType.Wizard),   BattleResult.Draw },
            { (CardType.Wizard,  CardType.Assassin), BattleResult.Lose },
            { (CardType.Wizard,  CardType.Archer),   BattleResult.Lose },
            { (CardType.Wizard,  CardType.Pikeman),  BattleResult.Win  },

            // Assassin
            { (CardType.Assassin,CardType.Knight),   BattleResult.Lose },
            { (CardType.Assassin,CardType.Wizard),   BattleResult.Win  },
            { (CardType.Assassin,CardType.Assassin), BattleResult.Draw },
            { (CardType.Assassin,CardType.Archer),   BattleResult.Win  },
            { (CardType.Assassin,CardType.Pikeman),  BattleResult.Lose },

            // Archer
            { (CardType.Archer,  CardType.Knight),   BattleResult.Lose },
            { (CardType.Archer,  CardType.Wizard),   BattleResult.Win  },
            { (CardType.Archer,  CardType.Assassin), BattleResult.Lose },
            { (CardType.Archer,  CardType.Archer),   BattleResult.Draw },
            { (CardType.Archer,  CardType.Pikeman),  BattleResult.Win  },

            // Pikeman
            { (CardType.Pikeman, CardType.Knight),   BattleResult.Win  },
            { (CardType.Pikeman, CardType.Wizard),   BattleResult.Lose },
            { (CardType.Pikeman, CardType.Assassin), BattleResult.Win  },
            { (CardType.Pikeman, CardType.Archer),   BattleResult.Lose },
            { (CardType.Pikeman, CardType.Pikeman),  BattleResult.Draw },
        };

        public static BattleResult Compare(CardType attacker, CardType defender)
            => Map[(attacker, defender)];
    }


    public static class DeckUtils
    {
        private static readonly Random Rng = new Random();

        public static void FillRandom(List<CardType> deck, int size)
        {
            deck.Clear();
          
            var values = (CardType[])Enum.GetValues(typeof(CardType));

            for (int i = 0; i < size; i++)
                deck.Add(values[Rng.Next(values.Length)]);

            Shuffle(deck);
        }

        public static void Shuffle(List<CardType> deck)
        {
            for (int i = deck.Count - 1; i > 0; i--)
            {
                int j = Rng.Next(i + 1);
                (deck[i], deck[j]) = (deck[j], deck[i]);
            }
        }
    }
}
