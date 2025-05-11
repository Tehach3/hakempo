// Assets/_Game/Runtime/Events/GameUnityEvents.cs
using UnityEngine.Events;
using Game.Core;           // Para usar CardType y BattleResult

namespace Game.Runtime.Events
{
    /// <summary>
    /// Emite: carta atacante, carta defensora, resultado (Win/Lose/Draw)
    /// </summary>
    [System.Serializable]
    public class CardPlayedEvent : UnityEvent<CardType, CardType, BattleResult> { }

    /// <summary>
    /// Emite: HP del jugador 1, HP del jugador 2
    /// </summary>
    [System.Serializable]
    public class HpChangedEvent : UnityEvent<int, int> { }
}