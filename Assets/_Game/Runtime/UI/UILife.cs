using UnityEngine;
using TMPro;
using Game.Core;


public class UILife : MonoBehaviour
{
 [SerializeField] private TextMeshProUGUI playerLifeText;
 [SerializeField] private TextMeshProUGUI enemyLifeText;

 public void UpdateHp(int playerHp, int enemyHp)
 {
  playerLifeText.text = $"Vida: {playerHp}";
  enemyLifeText.text = $"Enemigo: {enemyHp}";
 }
}
