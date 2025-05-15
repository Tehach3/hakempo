using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Game.Multiplayer;

public class SimpleLobbyUI : MonoBehaviour
{
    [SerializeField] private TMP_InputField roomCodeInput;
    [SerializeField] private Button createButton;
    [SerializeField] private Button joinButton;

    private void Start()
    {
        createButton.onClick.AddListener(OnCreateClicked);
        joinButton.onClick.AddListener(OnJoinClicked);
    }

    private void OnCreateClicked()
    {
        var code = roomCodeInput.text.ToUpper().Trim();
        if (!string.IsNullOrEmpty(code))
        {
            MultiplayerManager.Instance.CreateRoom(code);
        }
    }

    private void OnJoinClicked()
    {
        var code = roomCodeInput.text.ToUpper().Trim();
        if (!string.IsNullOrEmpty(code))
        {
            MultiplayerManager.Instance.JoinRoom(code);
        }
    }
}