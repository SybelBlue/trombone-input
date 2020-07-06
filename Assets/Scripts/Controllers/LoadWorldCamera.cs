using UnityEngine;

namespace Utils
{
    public class LoadWorldCamera : MonoBehaviour
    {
        public void Start()
            => GetComponent<Canvas>().worldCamera = Camera.main;
    }
}