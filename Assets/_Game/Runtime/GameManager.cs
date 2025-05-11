using System;
using System.Collections;
using Game.Core;
using UnityEngine;
using Game.Runtime.Events;
using Random = UnityEngine.Random;

namespace Game.Runtime
{
    public sealed class GameManager : MonoBehaviour
    {
        [SerializeField] float turnTimeout = 30f;
        [SerializeField] int   deckSize    = 12;
        [SerializeField] int   handSize    = 3;
        [Header("UI Roots")]
        [SerializeField] Transform handP1Root;
        [SerializeField] Transform handP2Root;
        [SerializeField] Transform tableRoot; 
        
        [SerializeField] CardView  cardPrefab;   


        public CardPlayedEvent onCardPlayed;   
        public HpChangedEvent  onHpChanged; 
        private bool playerHasPlayed;      
        private CardView pendingPlayerCard;
        private CardView pendingAICard;

        private Player       p1, p2;
        private RoundService rounds;
        private float        timer;
        public bool faceUp = false;

        void Awake()
        {
            
            if (handP1Root == null)
                handP1Root = GameObject.Find("HandP1Root")?.transform;

            if (handP1Root == null)
                Debug.LogError("HandP1Root is not assigned!");
            // 1) Crear jugadores
            p1 = new Player("Player");
            p2 = new Player("AI");

            // 2) Generar y barajar mazos
            DeckUtils.FillRandom(p1.Deck, deckSize);
            DeckUtils.FillRandom(p2.Deck, deckSize);

            // 3) Dibujar manos
            DrawInitialHand(p1, handP1Root, true);
            DrawInitialHand(p2, handP2Root, false);
        }
        
        

        void Update()
        {
            if (rounds == null) return;
            timer -= Time.deltaTime;

           /** if (timer <= 0f)
            {
                AutoPlayIfNeeded(p1);
                AutoPlayIfNeeded(p2);
                rounds.Resolve();         
                timer = turnTimeout;
            }**/
        }



        private void DrawInitialHands(Player pl)
        {
            for (int i = 0; i < handSize && pl.Deck.Count > 0; i++)
            {
                pl.Hand.Add(pl.Deck[0]);
                pl.Deck.RemoveAt(0);
            }
        }

        private void AutoPlayIfNeeded(Player pl)
        {
            if (pl.SelectedCard is null && pl.Hand.Count > 0)
            {
                int idx = Random.Range(0, pl.Hand.Count);
                var card = pl.Hand[idx];
                pl.Select(card);
                pl.Hand.RemoveAt(idx);
            }
        }

       
        private void HandleRoundResolved(CardType c1, CardType c2, BattleResult res)
        {
            onCardPlayed?.Invoke(c1, c2, res);
            onHpChanged?.Invoke(p1.Hp, p2.Hp);

            if (!rounds.CanStartNextRound(deckSize))
                Debug.Log("Game Over");
        }
        private void DrawInitialHand(Player pl, Transform uiRoot, bool faceUp = true)
        {
            foreach (Transform c in uiRoot) Destroy(c.gameObject);
            pl.Hand.Clear();

            for (int i = 0; i < handSize && pl.Deck.Count > 0; i++)
            {
                var type = pl.Deck[0];
                pl.Deck.RemoveAt(0);
                pl.Hand.Add(type);

                //  Instantiate(cardPrefab, uiRoot).Init(type, faceUp);
                var view = Instantiate(cardPrefab, uiRoot);
                view.Init(type, faceUp, OnCardClicked);
            }
        }
        
        private void OnCardClicked(CardView view)
        {
            // ignora clics si ya jugó o si la carta es del rival
            if (playerHasPlayed || view.transform.parent != handP1Root) return;

            playerHasPlayed = true;
            pendingPlayerCard = view;

            PlayCardFromHand(p1, view.Type);   // lógica core
            MoveCardToTable(view, faceUp:true);

            DrawOneAndRefresh(p1, handP1Root, faceUp:true);

            // Turno IA
            PlayAITurn();
        }
        
        private void PlayAITurn()
        {
            int idx = Random.Range(0, handP2Root.childCount);
            pendingAICard = handP2Root.GetChild(idx).GetComponent<CardView>();

            PlayCardFromHand(p2, pendingAICard.Type);
            MoveCardToTable(pendingAICard, faceUp:false);

            DrawOneAndRefresh(p2, handP2Root, faceUp:false);

            ResolveRound();   // ambas cartas ya en la mesa
        }
        
        private void MoveCardToTable(CardView view, bool faceUp)
        {
            view.transform.SetParent(tableRoot, worldPositionStays:false);
            view.ShowFace(faceUp);
        }

        private void DrawOneAndRefresh(Player pl, Transform uiRoot, bool faceUp)
        {
            if (pl.Deck.Count == 0) return;

            var type = pl.Deck[0];
            pl.Deck.RemoveAt(0);
            pl.Hand.Add(type);

            Instantiate(cardPrefab, uiRoot)
                .Init(type, faceUp, OnCardClicked);

            // Re-posiciona si quieres: layout.RefreshLayout();
        }
        
        private void ResolveRound()
        {
            var result = BattleRules.Compare(pendingPlayerCard.Type,
                pendingAICard.Type);

            // Muestra la carta de IA
            pendingAICard.ShowFace(true);

            // Log, HP, eventos…
            Debug.Log($"Result: {result}");

            // Limpia mesa y prepara siguiente turno
            StartCoroutine(ClearTableAndNextTurn());
        }
        
        private void PlayCardFromHand(Player player, CardType type)
        {
            // 1. quita la carta de la mano
            if (!player.Hand.Remove(type))
            {
                Debug.LogError($"Card {type} not found in hand!");
                return;
            }

            // 2. marca como seleccionada (para RoundService)
            player.Select(type);   // o: player.SelectedCard = type;

            // 3. roba otra si queda mazo
            if (player.Deck.Count > 0)
            {
                var newCard = player.Deck[0];
                player.Deck.RemoveAt(0);
                player.Hand.Add(newCard);
            }
        }

        IEnumerator ClearTableAndNextTurn()
        {
            yield return new WaitForSeconds(1.5f);  // deja ver las cartas
            Destroy(pendingPlayerCard.gameObject);
            Destroy(pendingAICard.gameObject);

            playerHasPlayed = false;
        }
    }
}
