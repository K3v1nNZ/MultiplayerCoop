using Steamworks;
using Steamworks.Data;
using TMPro;
using UnityEngine;

namespace Game.MenuUI
{
    public class LobbyPrefabScript : MonoBehaviour
    {
        [SerializeField] private TMP_Text lobbyName;
        [SerializeField] private TMP_Text lobbyPlayers;
        private Lobby _lobby;
        
        public void SetLobby(Lobby lobby)
        {
            _lobby = lobby;
            lobbyName.text = lobby.GetData("OwnerName");
            lobbyPlayers.text = $"{lobby.MemberCount}/{lobby.MaxMembers}";
        }
        
        public async void JoinLobby()
        {
            await SteamMatchmaking.JoinLobbyAsync(_lobby.Id);
        }
    }
}