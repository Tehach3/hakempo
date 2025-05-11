using System.Collections.Generic;
using UnityEngine;

public class GodGameManager : MonoBehaviour
{
    
    //private DrawPhase drawPhase;
    
    
    //public static GameManager Instance;
    public bool autoSimulate = true;
    private float autoTurnDelay = 1.5f; // segundos entre turnos
    private float autoTurnTimer = 0f;
    public enum CardType 
    {
        knight, 
        wizzard, 
        assassin, 
        archer, 
        pikeman
    };

    public enum BattleResult
    {
        Win,
        Lose,
        Draw
    };
    
    public float targetTimeDrop = 30.0f;//timer para colocar una carta en el tablero(incluso puede ser menos)
    public float targetTimeDraft = 60.0f;//timer para cuando se tengamos la logica del draft de las cartas con el random
    private float timerDrop;
    private bool roundInProgress = false;
    private int cardsPlayed = 0; // Conteo de rondas (hasta 12)

    private const int MAX_CARDS_PER_PLAYER = 12;
    private const int HAND_SIZE = 3;
    
    
    [System.Serializable]
    public class Player
    {
        public string name;
        public int HP = 10;
        public CardType? selectedCard;
        public List<CardType> deck = new List<CardType>();
        public List<CardType> hand = new List<CardType>();
    }
    private void CheckGameOver()
    {
        
        if (player1.HP == player2.HP )
        {
            Debug.Log("¡Draw!");
            EndGame();
        }
        else if (player1.HP <= 0)
        {
            Debug.Log($"{player2.name} Won the game 🎉");
            EndGame();
        }
        else if (player2.HP <= 0)
        {
            Debug.Log($"{player1.name} Won the game 🎉");
            EndGame();
        }
        else
        {
            player1.selectedCard = null;
            player2.selectedCard = null;
        }
    }
    
    private void EndGame()
    {
        Debug.Log("Game Over.");
    }
    
    public Player player1 = new Player { name = "Real Player" };
    public Player player2 = new Player { name = "IA Player" };
    
    private Dictionary<CardType, List<CardType>> winMap;
    
    private void Start()
    {
        //drawPhase = new DrawPhase();
        
        //drawPhase.ShowCard= (text)=>
        {
            //Debug.Log($"ShowCard {player1.name}: {text}");
        };
        
       // if (Instance == null) Instance = this;
       // else Destroy(gameObject);

        InitBattleMap();
        InitPlayerDeckAndHand(player1);
        InitPlayerDeckAndHand(player2);

        Debug.Log("Iniciando simulación automática...");
        StartNewTurn();
    }

    
    private void Update()
    {
        if (!roundInProgress || !autoSimulate) return;

        timerDrop -= Time.deltaTime;
        autoTurnTimer -= Time.deltaTime;

        if (timerDrop <= 0)
        {
            AutoPlayIfNeeded(player1);
            AutoPlayIfNeeded(player2);
            ResolveRound();
        }
        else if (autoTurnTimer <= 0 && player1.selectedCard == null && player2.selectedCard == null)
        {
            // Simula que cada jugador elige carta aleatoriamente antes de que acabe el timer
            AutoPlayIfNeeded(player1);
            AutoPlayIfNeeded(player2);
            ResolveRound();
            autoTurnTimer = autoTurnDelay;
        }
    }

    
    private void StartNewTurn()
    {
        if (cardsPlayed >= MAX_CARDS_PER_PLAYER)
        {
            DetermineGameResultByHP();
            return;
        }
   // drawPhase.Init();
        timerDrop = targetTimeDrop;
        autoTurnTimer = autoTurnDelay;
        roundInProgress = true;

        player1.selectedCard = null;
        player2.selectedCard = null;

        Debug.Log($"---------------- Turno {cardsPlayed + 1} ----------------");
        Debug.Log($"Cartas en mano: {player1.name} [{string.Join(", ", player1.hand)}], {player2.name} [{string.Join(", ", player2.hand)}]");
    }
    
    private void ResolveRound()
    {
        roundInProgress = false;

        if (!player1.selectedCard.HasValue || !player2.selectedCard.HasValue)
        {
            Debug.LogError("One of the players did not play a card");
            return;
        }

        DetermineRoundWinner(player1.selectedCard.Value, player2.selectedCard.Value);
        cardsPlayed++;

        if (player1.HP <= 0 || player2.HP <= 0)
        {
            CheckGameOver();
        }
        else
        {
            StartNewTurn();
        }
    }
    private void DetermineGameResultByHP()
    {
        if (player1.HP > player2.HP)
            Debug.Log($"{player1.name} Won for more HP.");
        else if (player2.HP > player1.HP)
            Debug.Log($"{player2.name}  Won for more HP.");
        else
            Debug.Log("Tie! Both ended with the same amount of HP.");

        EndGame();
    }
    
    
    
    private void AutoPlayIfNeeded(Player player)
    {
        if (!player.selectedCard.HasValue && player.hand.Count > 0)
        {
            int randomIndex = Random.Range(0, player.hand.Count);
            PlayCardFromHand(player, randomIndex);
            Debug.Log($"{player.name} AutoPlay: jugó {player.selectedCard.Value}");

        }
    }
    private Dictionary<(CardType attacker, CardType defender), BattleResult> battleMap;
    private void InitBattleMap()
    {
        battleMap = new Dictionary<(CardType, CardType), BattleResult>
        {
            // knight
            { (CardType.knight, CardType.knight), BattleResult.Draw },
            { (CardType.knight, CardType.wizzard), BattleResult.Lose },
            { (CardType.knight, CardType.assassin), BattleResult.Win },
            { (CardType.knight, CardType.archer), BattleResult.Win },
            { (CardType.knight, CardType.pikeman), BattleResult.Lose },

            // wizzard
            { (CardType.wizzard, CardType.knight), BattleResult.Win },
            { (CardType.wizzard, CardType.wizzard), BattleResult.Draw },
            { (CardType.wizzard, CardType.assassin), BattleResult.Lose },
            { (CardType.wizzard, CardType.archer), BattleResult.Lose },
            { (CardType.wizzard, CardType.pikeman), BattleResult.Win },

            // assassin
            { (CardType.assassin, CardType.knight), BattleResult.Lose },
            { (CardType.assassin, CardType.wizzard), BattleResult.Win },
            { (CardType.assassin, CardType.assassin), BattleResult.Draw },
            { (CardType.assassin, CardType.archer), BattleResult.Win },
            { (CardType.assassin, CardType.pikeman), BattleResult.Lose },

            // archer
            { (CardType.archer, CardType.knight), BattleResult.Lose },
            { (CardType.archer, CardType.wizzard), BattleResult.Win },
            { (CardType.archer, CardType.assassin), BattleResult.Lose },
            { (CardType.archer, CardType.archer), BattleResult.Draw },
            { (CardType.archer, CardType.pikeman), BattleResult.Win },

            // pikeman
            { (CardType.pikeman, CardType.knight), BattleResult.Win },
            { (CardType.pikeman, CardType.wizzard), BattleResult.Lose },
            { (CardType.pikeman, CardType.assassin), BattleResult.Win },
            { (CardType.pikeman, CardType.archer), BattleResult.Lose },
            { (CardType.pikeman, CardType.pikeman), BattleResult.Draw },
        };
    }
    
    private BattleResult CompareCards(CardType attacker, CardType defender)
    {
        return battleMap[(attacker, defender)];
    }
    
    private void DetermineRoundWinner(CardType p1Card, CardType p2Card)
    {
        var result = CompareCards(p1Card, p2Card);

        switch (result)
        {
            case BattleResult.Win:
                player2.HP--;
                Debug.Log($"{player1.name} won the round!");
                break;

            case BattleResult.Lose:
                player1.HP--;
                Debug.Log($"{player2.name} won the round!");
                break;

            case BattleResult.Draw:
                Debug.Log("Draw");
                break;
        }

        CheckGameOver();
        
        
    }
    private void GenerateRandomDeck(Player player, int deckSize = 12)
    {
        player.deck.Clear();
        System.Array cardValues = System.Enum.GetValues(typeof(CardType));
        System.Random random = new System.Random();

        for (int i = 0; i < deckSize; i++)
        {
            CardType randomCard = (CardType)cardValues.GetValue(random.Next(cardValues.Length));
            player.deck.Add(randomCard);
        }

        // Barajar el mazo
        ShuffleDeck(player.deck, random);
    }
    
    private void ShuffleDeck(List<CardType> deck, System.Random random)
    {
        int deckCount = deck.Count;
        while (deckCount > 1)
        {
            deckCount--;
            int k = random.Next(deckCount + 1);
            CardType value = deck[k];
            deck[k] = deck[deckCount];
            deck[deckCount] = value;
        }
    }
    
    private void InitPlayerDeckAndHand(Player player, int deckSize = 12, int handSize = 3)
    {
        player.deck.Clear();
        player.hand.Clear();
        player.selectedCard = null;

        System.Array cardValues = System.Enum.GetValues(typeof(CardType));
        System.Random rng = new System.Random();

        // Generar mazo
        for (int i = 0; i < deckSize; i++)
        {
            CardType randomCard = (CardType)cardValues.GetValue(rng.Next(cardValues.Length));
            player.deck.Add(randomCard);
        }

        ShuffleDeck(player.deck, rng);

        // Robar mano inicial
        for (int i = 0; i < handSize && player.deck.Count > 0; i++)
        {
            player.hand.Add(player.deck[0]);
            player.deck.RemoveAt(0);
        }
    }
    public void PlayCardFromHand(Player player, int handIndex)
    {
        if (handIndex < 0 || handIndex >= player.hand.Count)
        {
            Debug.LogError("Invalid index for hand.");
            return;
        }

        // Seleccionar la carta
        player.selectedCard = player.hand[handIndex];
        player.hand.RemoveAt(handIndex);

        // Robar otra si hay cartas
        if (player.deck.Count > 0)
        {
            player.hand.Add(player.deck[0]);
            player.deck.RemoveAt(0);
        }

         // Comprobar si hay cartas en la mano
        if (player.hand.Count == 0)
        {
            Debug.Log($"{player.name} has no cards in his hand.");
        } 
        
        if (player.hand.Count == 0 && player.deck.Count > 0)
        {
            int cardsToDraw = Mathf.Min(HAND_SIZE, player.deck.Count);
            for (int i = 0; i < cardsToDraw; i++)
            {
                player.hand.Add(player.deck[0]);
                player.deck.RemoveAt(0);
            }
        }
    }
    
}
