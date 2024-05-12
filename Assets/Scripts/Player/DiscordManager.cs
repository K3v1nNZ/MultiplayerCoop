using System;
using Discord;
using UnityEngine;

namespace Game.Player
{
    public class DiscordManager : MonoBehaviour
    {
        public static DiscordManager Instance;
        [SerializeField] private bool isEnabled;
        [SerializeField] private long clientId;
        [SerializeField] private string largeImageKey;
        [SerializeField] private string largeImageText;
        private Discord.Discord _discord;
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(this);
            }
        }

        private void Start()
        {
            if (!isEnabled) Destroy(gameObject);
            try
            {
                _discord = new Discord.Discord(clientId, (ulong) CreateFlags.NoRequireDiscord);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
            
            Activity activity = new()
            {
                Assets =
                {
                    LargeImage = largeImageKey,
                    LargeText = largeImageText
                },
                Timestamps =
                {
                    Start = DateTimeOffset.Now.ToUnixTimeSeconds()
                }
            };
            _discord.GetActivityManager().UpdateActivity(activity, result =>
            {
                if (result == Result.Ok)
                {
                    Debug.Log("Discord Rich Presence is now active.");
                }
                else
                {
                    Debug.LogError("Discord Rich Presence failed to activate.");
                }
            });
        }

        private void Update()
        {
            _discord.RunCallbacks();
        }
    }
}
