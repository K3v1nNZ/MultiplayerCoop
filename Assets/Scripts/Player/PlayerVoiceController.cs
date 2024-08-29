using System;
using System.Collections.Generic;
using System.IO;
using FishNet.Managing.Logging;
using FishNet.Object;
using FishNet.Transporting;
using Steamworks;
using UnityEngine;

namespace Game.Player
{
    public class PlayerVoiceController : NetworkBehaviour
    {
        private AudioSource _audioSource;
        private MemoryStream _compressedVoiceStream = new();
        private MemoryStream _decompressedVoiceStream = new();
        private Queue<float> _streamingReadQueue = new();

        private void Start()
        {
            _audioSource = GetComponent<AudioSource>();
            _audioSource.clip = AudioClip.Create("SteamVoice", 256, 1, (int)SteamUser.OptimalSampleRate, true, PcmReaderCallback);
            _audioSource.loop = true;
            _audioSource.Play();
        }

        [Client(RequireOwnership = true, Logging = LoggingType.Off)]
        private void Update()
        {
            SteamUser.VoiceRecord = PlayerInputManager.Instance.PlayerInputActions.Player.VoiceChat.IsPressed();
            if (/*SteamUser.HasVoiceData*/PlayerInputManager.Instance.PlayerInputActions.Player.VoiceChat.IsPressed())
            {
                _compressedVoiceStream.Position = 0;
                int numBytesWritten = SteamUser.ReadVoiceData(_compressedVoiceStream);
                VoiceServerRpc(new ArraySegment<byte>(_compressedVoiceStream.GetBuffer(), 0, numBytesWritten));
            }
        }

        [ServerRpc(RequireOwnership = true)]
        private void VoiceServerRpc(ArraySegment<byte> voiceData, Channel channel = Channel.Unreliable)
        {
            VoiceDataObserverRpc(voiceData);
        }

        [ObserversRpc(ExcludeOwner = true)]
        private void VoiceDataObserverRpc(ArraySegment<byte> voiceData, Channel channel = Channel.Unreliable)
        {
            _compressedVoiceStream.Position = 0;
            _compressedVoiceStream.Write(voiceData);
            _compressedVoiceStream.Position = 0;
            _decompressedVoiceStream.Position = 0;
            int numBytesWritten = SteamUser.DecompressVoice(_compressedVoiceStream, voiceData.Count, _decompressedVoiceStream);
            _decompressedVoiceStream.Position = 0;
            while (_decompressedVoiceStream.Position < numBytesWritten)
            {
                byte byte1 = (byte)_decompressedVoiceStream.ReadByte();
                byte byte2 = (byte)_decompressedVoiceStream.ReadByte();
                short pcmShort = (short)((byte2 << 8) | (byte1 << 0));
                float pcmFloat = Convert.ToSingle(pcmShort) / short.MaxValue;
                _streamingReadQueue.Enqueue(pcmFloat);
            }
        }

        private void PcmReaderCallback(float[] data)
        {
            for (int i = 0; i < data.Length; i++)
            {
                if (_streamingReadQueue.TryDequeue(out var sample))
                {
                    data[i] = sample;
                }
                else
                {
                    data[i] = 0.0f;
                }
            }
        }
    }
}