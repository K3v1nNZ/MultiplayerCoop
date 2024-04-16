using System.IO;
using FishNet.Managing.Logging;
using FishNet.Object;
using Steamworks;
using UnityEngine;

namespace Game.Player
{
    public class PlayerLobbyController : NetworkBehaviour
    {
        private AudioSource _audioSource;
        private MemoryStream _output;
        private MemoryStream _stream;
        private MemoryStream _input;
        private int _optimalRate;
        private int _clipBufferSize;
        private float[] _clipBuffer;
        private int _playbackBuffer;
        private int _dataPosition;
        private int _dataReceived;

        public override void OnStartClient()
        {
            _audioSource = GetComponent<AudioSource>();
            _optimalRate = (int)SteamUser.OptimalSampleRate;
            _clipBufferSize = _optimalRate * 10;
            _clipBuffer = new float[_clipBufferSize];
            _stream = new MemoryStream();
            _output = new MemoryStream();
            _input = new MemoryStream();
            _audioSource.clip = AudioClip.Create("VoiceData", 256, 1, _optimalRate, true, OnAudioRead, null);
            _audioSource.loop = true;
            _audioSource.Play();
        }

        [Client(RequireOwnership = true, Logging = LoggingType.Off)]
        public override void OnStopClient()
        {
            base.Despawn();
        }

        [Client(RequireOwnership = true, Logging = LoggingType.Off)]
        private void Update()
        {
            SteamUser.VoiceRecord = Input.GetKey(KeyCode.V);
            if (SteamUser.HasVoiceData)
            {
                int compressedWritten = SteamUser.ReadVoiceData(_stream);
                _stream.Position = 0;
                VoiceServerRpc(_stream.GetBuffer(), compressedWritten);
            }
        }

        [ServerRpc]
        private void VoiceServerRpc(byte[] compressed, int bytesWritten)
        {
            VoiceDataObserverRpc(compressed, bytesWritten);
        }

        [ObserversRpc(ExcludeOwner = true)]
        private void VoiceDataObserverRpc(byte[] compressed, int bytesWritten)
        {
            _input.Write(compressed, 0, bytesWritten);
            _input.Position = 0;
            int uncompressedWritten = SteamUser.DecompressVoice(_input, bytesWritten, _output);
            _input.Position = 0;
            byte[] outputBuffer = _output.GetBuffer();
            WriteToClip(outputBuffer, uncompressedWritten);
            _output.Position = 0;
        }

        [Client]
        private void OnAudioRead(float[] data)
        {
            for (int i = 0; i < data.Length; ++i)
            {
                data[i] = 0;
                if (_playbackBuffer > 0)
                {
                    _dataPosition = (_dataPosition + 1) % _clipBufferSize;
                    data[i] = _clipBuffer[_dataPosition];
                    _playbackBuffer--;
                }
            }
        }

        [Client]
        private void WriteToClip(byte[] uncompressed, int iSize)
        {
            for (int i = 0; i < iSize; i += 2)
            {
                float converted = (short)(uncompressed[i] | uncompressed[i + 1] << 8) / 32767.0f;
                _clipBuffer[_dataReceived] = converted;
                _dataReceived = (_dataReceived + 1) % _clipBufferSize;
                _playbackBuffer++;
            }
        }
    }
}