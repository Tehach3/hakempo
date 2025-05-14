using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Game.Core;

namespace Game.UI
{
    public class UIRoundResult : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI resultText;
        [SerializeField] private TextMeshProUGUI playerCardText;
        [SerializeField] private TextMeshProUGUI enemyCardText;
        [SerializeField] private GameObject panel;

        public void Show(CardType playerCard, CardType enemyCard, BattleResult result)
        {
            panel.SetActive(true);
            playerCardText.text = $"You: {playerCard}";
            enemyCardText.text = $"Enemy: {enemyCard}";

            resultText.text = result switch
            {
                BattleResult.Win => "<color=green>¡Win!</color>",
                BattleResult.Lose => "<color=red>Lose</color>",
                _ => "<color=yellow>draw</color>"
            };
        }

        public void Hide()
        {
            panel.SetActive(false);
        }
    }
}