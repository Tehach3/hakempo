using System;

namespace Game.Core
{
    public sealed class RoundService
    {
        public event Action<CardType, CardType, BattleResult>? OnRoundResolved;

        private readonly Player _p1, _p2;
        private int _cardsPlayed;

        public RoundService(Player p1, Player p2) { _p1 = p1; _p2 = p2; }

        public bool CanStartNextRound(int limit) => _cardsPlayed < limit && _p1.Hp > 0 && _p2.Hp > 0;

        public void Resolve()
        {
            if (!_p1.SelectedCard.HasValue || !_p2.SelectedCard.HasValue)
                throw new InvalidOperationException("Both players must select.");

            var res = BattleRules.Compare(_p1.SelectedCard.Value, _p2.SelectedCard.Value);
            ApplyDamage(res);
            OnRoundResolved?.Invoke(_p1.SelectedCard.Value, _p2.SelectedCard.Value, res);

            _p1.ClearSelection();
            _p2.ClearSelection();
            _cardsPlayed++;
        }

        private void ApplyDamage(BattleResult res)
        {
            if      (res == BattleResult.Win)  _p2.TakeDamage(1);
            else if (res == BattleResult.Lose) _p1.TakeDamage(1);
        }
    }
}