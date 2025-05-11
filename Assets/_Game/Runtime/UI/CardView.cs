using UnityEngine;
using UnityEngine.UI;
using Game.Core;        // <- contiene el enum CardType

/// <summary>
/// Representación visual de una carta: asigna el sprite correcto
/// según el <see cref="CardType"/> que reciba en Init().
/// </summary>
public class CardView : MonoBehaviour
{
    // ───────────────────────────────────────── UI refs
    [Header("UI")]
    [SerializeField] private Image frontImage;   // La Image donde se pinta la carta

    // (opcional) Si planeas girar la carta y ocultarla,
    // puedes exponer un sprite de reverso o una segunda Image.
    [SerializeField] private Sprite backSprite;

    // ───────────────────────────────────────── Sprites por tipo
    [Header("Sprites por tipo")]
    [SerializeField] private Sprite archerSprite;
    [SerializeField] private Sprite knightSprite;
    [SerializeField] private Sprite wizardSprite;
    [SerializeField] private Sprite pikerSprite;
    [SerializeField] private Sprite assassinSprite;

    /// <summary>El tipo de carta que representa esta vista.</summary>
    public CardType Type { get; private set; }
    public System.Action<CardView> OnCardSelected;

    // ───────────────────────────────────────── API pública
    /// <summary>
    /// Inicializa la carta con su tipo.  
    /// Llama a este método justo después de Instanciar el prefab.
    /// </summary>
    public void Init(CardType t, bool faceUp = true,
        System.Action<CardView> onSelect = null)
    {
        Type = t;
        OnCardSelected = onSelect;
        ShowFace(faceUp);
    }
    
    public void OnClick()   // asignado al Button
    {
        OnCardSelected?.Invoke(this);
    }

    /// <summary>Cambia entre frente y reverso (útil para flip).</summary>
    public void ShowFace(bool faceUp)
    {
        if (backSprite == null) return;      // no hay reverso asignado
        frontImage.sprite = faceUp ? SpriteFor(Type) : backSprite;
    }

    // ───────────────────────────────────────── helpers
    private Sprite SpriteFor(CardType t) => t switch
    {
        CardType.Archer   => archerSprite,
        CardType.Knight   => knightSprite,
        CardType.Wizard   => wizardSprite,
        CardType.Pikeman  => pikerSprite,      // “Pikeman” en tu enum
        CardType.Assassin => assassinSprite,
        _                 => archerSprite      // Fallback
    };
    
    
    
}
