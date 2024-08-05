using System;
using System.Collections.Generic;
using FishNet;
using Game.Networking;
using Game.Player;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Environment
{
    public class InformantBase : MonoBehaviour
    {
        public static InformantBase Instance;
        public Transform playerCamHolder;
        public Transform mapCamPos;
        public Transform laptopCamPos;
        public Transform boardCamPos;
        [Space(10)]
        [Header("Camera Board")]
        public RenderTexture cameraRenderTexture;
        public SecurityCamera activeCamera;
        [SerializeField] private GameObject cameraButtonContainer;
        [SerializeField] private GameObject cameraButtonPrefab;
        [SerializeField] private List<SecurityCamera> securityCameras;
        [Space(10)] 
        [Header("Map Board")] 
        [SerializeField] private Transform mapUpper;
        [SerializeField] private Transform mapLower;
        [SerializeField] private RectTransform mapTransform;
        [SerializeField] private Texture2D playerIcon;
        private Dictionary<RawImage, PlayerController> _playerImages = new();
        private bool _mapSetup;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }

            foreach (SecurityCamera securityCamera in securityCameras)
            {
                GameObject cameraButton = Instantiate(cameraButtonPrefab, cameraButtonContainer.transform);
                InformantCameraButton informantCameraButton = cameraButton.GetComponent<InformantCameraButton>();
                SecurityCamera camera1 = securityCamera;
                informantCameraButton.SetupButton(securityCamera.cameraName, () => camera1.ToggleCamera(true));
            }
        }

        private void Update()
        {
            if (PlayerController.Instance == null)
            {
                return;
            }
            if (PlayerController.Instance.playerRole == PlayerController.PlayerRole.Informant && !_mapSetup && InstanceFinder.ClientManager.Clients.Count == SpawnerManager.Instance._playersConnected.Count)
            {
                _mapSetup = true;
                MapSetup();
            }
            if (!_mapSetup)
            {
                return;
            }

            foreach (KeyValuePair<RawImage, PlayerController> playerImage in _playerImages)
            {
                playerImage.Key.rectTransform.anchoredPosition = FindInterfacePoint(playerImage.Value.transform);
            }
        }

        private void MapSetup()
        {
            foreach (PlayerController player in SpawnerManager.Instance._playersConnected.Values)
            {
                if (player.playerRole == PlayerController.PlayerRole.Informant)
                {
                    continue;
                }
                
                RawImage playerImage = new GameObject("Player Icon", typeof(RectTransform), typeof(RawImage)).GetComponent<RawImage>();
                playerImage.transform.SetParent(mapTransform);
                playerImage.texture = playerIcon;
                playerImage.rectTransform.sizeDelta = new Vector2(150, 150);
                playerImage.rectTransform.pivot = new Vector2(0.5f, 0.5f);
                playerImage.rectTransform.localScale = Vector3.one;
                playerImage.rectTransform.localPosition = Vector3.zero;
                _playerImages.Add(playerImage, player);
            }
        }

        private Vector2 FindInterfacePoint(Transform playerPosition)
        {
            Vector2 normalizedPosition = FindNormalizedPosition(playerPosition.position);
            return Rect.NormalizedToPoint(mapTransform.rect, normalizedPosition);
        }
        
        private Vector2 FindNormalizedPosition(Vector3 position)
        {
            float xPosition = Mathf.InverseLerp(mapLower.position.x, mapUpper.position.x, position.x);
            float zPosition = Mathf.InverseLerp(mapLower.position.z, mapUpper.position.z, position.z);
            return new Vector2(xPosition, zPosition);
        }
    }
}