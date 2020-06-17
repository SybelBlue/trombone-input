using UnityEngine;

public class FollowTheStylus : MonoBehaviour
{
    [SerializeField]
    private Transform stylusTransform;

    // Update is called once per frame
    void Update()
    {
        // transform.position = new Vector3(stylusTransform.position.x, transform.position.y, transform.position.z);
        // transform.position = new Vector3(transform.position.x, stylusTransform.position.y, transform.position.z);
        // transform.position = new Vector3(transform.position.x, transform.position.y, stylusTransform.position.z);
        transform.position = stylusTransform.position;
        // transform.rotation = Quaternion.Euler(stylusTransform.rotation.x, transform.rotation.y, transform.rotation.z);
        Vector3 transAngles = transform.rotation.eulerAngles;
        transform.rotation = Quaternion.Euler(transAngles.x, stylusTransform.rotation.eulerAngles.y, transAngles.z);
    }
}
