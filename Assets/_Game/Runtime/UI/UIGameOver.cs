using TMPro;
using UnityEngine;

namespace Game.UI
{
    public class UIGameOver : MonoBehaviour
    {
        [SerializeField] private GameObject panel;
        [SerializeField] private TextMeshProUGUI resultText;

        public void Show(int hpPlayer, int hpEnemy)
        {
            panel.SetActive(true);

            if (hpPlayer > hpEnemy)
                resultText.text = "<color=green>¡You won!</color>\nYour life is greater than the enemy's";
            else if (hpPlayer < hpEnemy)
                resultText.text = "<color=red>You Lose</color>\nYour life is less than the enemy's";
            else
                resultText.text = "<color=yellow>Draw</color>\nThey both have the same life";
        }
    }
}