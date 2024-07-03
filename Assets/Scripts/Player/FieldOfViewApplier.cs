using System.IO;
using UnityEngine;

namespace Game.Player
{
    [RequireComponent(typeof(Camera))]
    public class FieldOfViewApplier : MonoBehaviour
    {
        private Camera _camera;
        
        private void Start()
        {
            _camera = gameObject.GetComponent<Camera>();
            _camera.fieldOfView = JsonUtility.FromJson<GameSettings>(File.ReadAllText(Application.persistentDataPath + "/settings.json")).FieldOfView;
        }

        private void ReloadFieldOfView()
        {
            _camera.fieldOfView = JsonUtility.FromJson<GameSettings>(File.ReadAllText(Application.persistentDataPath + "/settings.json")).FieldOfView;
        }
    }
}