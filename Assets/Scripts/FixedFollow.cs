using UnityEngine;

public class FixedFollow : MonoBehaviour
{
    public GameObject followParent;

    public Vector3 followOffset;

    // public bool followRotation;

    void Update()
    {
        if (followParent is null) return;

        var nextPos = Vector3.Slerp(transform.position, followParent.transform.position + followOffset, 0.15f);
        // var nextRot = followRotation ? Quaternion.Slerp(transform.rotation, followParent.transform.rotation, 0.05f) : transform.rotation;

        // transform.SetPositionAndRotation(nextPos, nextRot);
        transform.position = nextPos;
    }
}
