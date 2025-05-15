// SimulationRunner.cs  (Editor-only)
using UnityEditor;
using UnityEngine;
using Game.Core;

namespace Game.EditorTools
{
    
#if UNITY_EDITOR
     public static class SimulationRunner
    {
        private const int DeckSize = 12;
        private const int HandSize = 3;
        private const float LogDelay = 0.2f;   // Para leer los logs paso a paso

        [MenuItem("Tools/Card Game/Run Console Simulation %#r")]  // Ctrl/Cmd + Shift + R
        public static void Run()
        {
            // ── 1. Crear jugadores y barajar mazos ───────────────────────────────
            var p1 = new Player("Player");
            var p2 = new Player("AI");

            DeckUtils.FillRandom(p1.Deck, DeckSize);
            DeckUtils.FillRandom(p2.Deck, DeckSize);

            DrawInitialHand(p1);
            DrawInitialHand(p2);

            var rounds = new RoundService(p1, p2);
            rounds.OnRoundResolved += LogRound;

            Debug.Log("<b>---  Console-only Simulation  ---</b>");
            Debug.Log($"P1 Deck: {string.Join(", ", p1.Deck)}");
            Debug.Log($"P2 Deck: {string.Join(", ", p2.Deck)}");

            // ── 2. Bucle de turnos (máx. 12 por jugador) ────────────────────────
            while (rounds.CanStartNextRound(DeckSize))
            {
                AutoPlay(p1);
                AutoPlay(p2);
                rounds.Resolve();
            }

            // ── 3. Resultado final ─────────────────────────────────────────────
            if      (p1.Hp > p2.Hp) Debug.Log($"<color=green>{p1.Name} WINS</color>");
            else if (p2.Hp > p1.Hp) Debug.Log($"<color=green>{p2.Name} WINS</color>");
            else                    Debug.Log("<color=yellow>Draw!</color>");
        }

        // ----------------------------------------------------------------------

        private static void LogRound(CardType atk, CardType def, BattleResult res)
        {
            Debug.Log($"{atk} vs {def}  →  <b>{res}</b>");
            EditorApplication.delayCall += () => { };          // refresca consola
            System.Threading.Thread.Sleep((int)(LogDelay * 1000));
        }

        private static void DrawInitialHand(Player pl)
        {
            for (int i = 0; i < HandSize && pl.Deck.Count > 0; i++)
            {
                pl.Hand.Add(pl.Deck[0]);
                pl.Deck.RemoveAt(0);
            }
        }

        private static void AutoPlay(Player pl)
        {
            if (pl.SelectedCard is null && pl.Hand.Count > 0)
            {
                var rnd  = new System.Random();
                int idx  = rnd.Next(pl.Hand.Count);
                var card = pl.Hand[idx];

                pl.Select(card);
                pl.Hand.RemoveAt(idx);

                // re-robar si queda mazo
                if (pl.Deck.Count > 0)
                {
                    pl.Hand.Add(pl.Deck[0]);
                    pl.Deck.RemoveAt(0);
                }
            }
        }
    }

#endif
   
}
