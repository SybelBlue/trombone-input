using UnityEngine;

namespace Utils
{
#pragma warning disable 649
    public class FollowTheStylus : MonoBehaviour
    {
        [SerializeField]
        private Transform stylusTransform;

        void Start()
        {
            stylusTransform = GameObject.FindWithTag("StylusTag").transform;
        }

        // Update is called once per frame
        void Update()
        {
            Vector3 transAngles = transform.rotation.eulerAngles;
            transform.rotation = Quaternion.Euler(stylusTransform.rotation.eulerAngles.x, transAngles.y, transAngles.z);
        }
    }
}
