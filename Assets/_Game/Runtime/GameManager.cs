using System;
using System.Collections;
using Game.Core;
using UnityEngine;
using Game.Runtime.Events;
using Game.UI;
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
        [SerializeField] private UILife lifeUI;
        
        [SerializeField] CardView  cardPrefab; 
        
        [SerializeField] private UIRoundResult roundResultUI;
        
        [SerializeField] private UIGameOver gameOverUI;




        public CardPlayedEvent onCardPlayed;   
        public Action<int, int> onHpChanged; 
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
            p1 = new Player("Player");
            p2 = new Player("AI");
            
            
            DeckUtils.FillRandom(p1.Deck, deckSize);
            DeckUtils.FillRandom(p2.Deck, deckSize);
            
            DrawInitialHand(p1, handP1Root, true);
            DrawInitialHand(p2, handP2Root, false);
        }
        
       
        
        void Start()
        {
            
            rounds = new RoundService(p1, p2);
            rounds.OnRoundResolved += HandleRoundResolved;
           
            if (lifeUI == null)
                lifeUI = FindObjectOfType<UILife>();

            
            onHpChanged += lifeUI.UpdateHp;

          
            onHpChanged?.Invoke(p1.Hp, p2.Hp);
        }

        
        

        void Update()
        {
            if (rounds == null) return;
            timer -= Time.deltaTime;
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

            roundResultUI.Show(c1, c2, res);
            
            bool p1SinCartas = p1.Deck.Count == 0 && p1.Hand.Count == 0;
            bool p2SinCartas = p2.Deck.Count == 0 && p2.Hand.Count == 0;
            bool sinCartasAmbos = p1SinCartas && p2SinCartas;
            bool alguienMurio = p1.Hp <= 0 || p2.Hp <= 0;

            if (sinCartasAmbos || alguienMurio)
            {
                roundResultUI.Hide();
                gameOverUI.Show(p1.Hp, p2.Hp);
                return;
            }
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

               
                var view = Instantiate(cardPrefab, uiRoot);
                view.Init(type, faceUp, OnCardClicked);
            }
        }
        
        private void OnCardClicked(CardView view)
        {
            
            if (playerHasPlayed || view.transform.parent != handP1Root) return;

            playerHasPlayed = true;
            pendingPlayerCard = view;

            PlayCardFromHand(p1, view.Type);  
            MoveCardToTable(view, faceUp:true);

            DrawOneAndRefresh(p1, handP1Root, faceUp:true);

            
            PlayAITurn();
        }
        
        private void PlayAITurn()
        {
            int idx = Random.Range(0, handP2Root.childCount);
            pendingAICard = handP2Root.GetChild(idx).GetComponent<CardView>();

            PlayCardFromHand(p2, pendingAICard.Type);
            MoveCardToTable(pendingAICard, faceUp:false);

            DrawOneAndRefresh(p2, handP2Root, faceUp:false);

            ResolveRound();  
            DrawOneAndRefresh(p1, handP1Root, faceUp: true);
        }
        
        private void MoveCardToTable(CardView view, bool faceUp)
        {
            view.transform.SetParent(tableRoot, worldPositionStays:false);
            view.ShowFace(faceUp);
        }

        private void DrawOneAndRefresh(Player pl, Transform uiRoot, bool faceUp)
        {
            if (pl.Hand.Count >= handSize) return;
            if (pl.Deck.Count == 0) return;

            var type = pl.Deck[0];
            pl.Deck.RemoveAt(0);
            pl.Hand.Add(type);

            Instantiate(cardPrefab, uiRoot)
                .Init(type, faceUp, OnCardClicked);
        }
        
        private void ResolveRound()
        {
            pendingAICard.ShowFace(true); 
            rounds.Resolve();           
            StartCoroutine(ClearTableAndNextTurn());
            Debug.Log($"Jugador seleccionó: {p1.SelectedCard}, IA seleccionó: {p2.SelectedCard}");

        }
        
        private void PlayCardFromHand(Player player, CardType type)
        {
            if (!player.Hand.Remove(type))
            {
                Debug.LogError($"Card {type} not found in hand!");
                return;
            }

            player.Select(type);
        }
        
        IEnumerator ClearTableAndNextTurn()
        {
            Debug.Log("🟡 Entró en ClearTableAndNextTurn");
            yield return new WaitForSeconds(0.1f);
            
            Debug.Log($"P1 - Deck: {p1.Deck.Count}, Hand: {p1.Hand.Count}");
            Debug.Log($"P2 - Deck: {p2.Deck.Count}, Hand: {p2.Hand.Count}");

            Destroy(pendingPlayerCard.gameObject);
            Destroy(pendingAICard.gameObject);

            pendingPlayerCard = null;
            pendingAICard = null;
            playerHasPlayed = false;
            bool p1SinCartas = p1.Deck.Count == 0 && p1.Hand.Count == 0;
            bool p2SinCartas = p2.Deck.Count == 0 && p2.Hand.Count == 0;
            bool sinCartasAmbos = p1SinCartas && p2SinCartas;
            bool alguienMurio = p1.Hp <= 0 || p2.Hp <= 0;

            if (sinCartasAmbos || alguienMurio)
            {
                roundResultUI.Hide();
                gameOverUI.Show(p1.Hp, p2.Hp);
                yield break;
            }
        }

    }
}
