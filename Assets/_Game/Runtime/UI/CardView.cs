using System;
using UnityEngine;
using UnityEngine.UI;
using Game.Core;       

/// <summary>
/// Representación visual de una carta: asigna el sprite correcto
/// según el <see cref="CardType"/> que reciba en Init().
/// </summary>
public class CardView : MonoBehaviour
{
    // ───────────────────────────────────────── UI refs
    [Header("UI")]
    [SerializeField] private Image frontImage; 


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
    public void Init(CardType type, bool faceUp, Action<CardView> onClick)
    {
        Type = type;
        ShowFace(faceUp);

        var btn = GetComponent<Button>();
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(() => onClick?.Invoke(this));
    }
    
    public void OnClick() 
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
        CardType.Pikeman  => pikerSprite,    
        CardType.Assassin => assassinSprite,
        _                 => archerSprite  
    };
    
    
    
}
