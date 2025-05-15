using Game.Runtime;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace Game.Multiplayer
{
    public class MultiplayerManager : MonoBehaviourPunCallbacks
    {
        public static MultiplayerManager Instance;

        [SerializeField] private string gameVersion = "0.1";

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);

            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            ConnectToPhoton();
        }

        private void ConnectToPhoton()
        {
            PhotonNetwork.AutomaticallySyncScene = true;
            PhotonNetwork.GameVersion = gameVersion;
            PhotonNetwork.ConnectUsingSettings();
        }

        public void CreateRoom(string roomCode)
        {
            RoomOptions options = new RoomOptions
            {
                MaxPlayers = 2,
                IsVisible = false, // solo se entra con código
                IsOpen = true
            };

            PhotonNetwork.CreateRoom(roomCode, options, TypedLobby.Default);
        }

        public void JoinRoom(string roomCode)
        {
            PhotonNetwork.JoinRoom(roomCode);
        }

        // --- Callbacks ---

        public override void OnConnectedToMaster()
        {
            Debug.Log("Conectado a Photon.");
        }

        public override void OnCreatedRoom()
        {
            Debug.Log($"Sala creada: {PhotonNetwork.CurrentRoom.Name}");
        }

        public override void OnJoinRoomFailed(short returnCode, string message)
        {
            Debug.LogError($"Falló al unirse: {message}");
        }

        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            Debug.LogError($"Falló al crear la sala: {message}");
        }
        
        public override void OnJoinedRoom()
        {
            Debug.Log($"Unido a sala: {PhotonNetwork.CurrentRoom.Name}");
            FindObjectOfType<GameManager>()?.StartGame();
        }

    }
}