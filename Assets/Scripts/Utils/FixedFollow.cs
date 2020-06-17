using UnityEngine;

public class FixedFollow : MonoBehaviour
{
    public GameObject followParent;

    public Vector3 followOffset;

    public bool followRotation;

    void Update()
    {
        if (followParent == null) return;

        Vector3 nextPos;
        Quaternion nextRot;

        if (followRotation)
        {
            nextRot = Quaternion.Slerp(transform.rotation, followParent.transform.rotation, 0.15f);
            var target = followParent.transform.position + nextRot * followOffset;
            nextPos = Vector3.Slerp(transform.position, target, 0.15f); ;
        }
        else
        {
            nextPos = Vector3.Slerp(transform.position, followParent.transform.position + followOffset, 0.15f);
            nextRot = transform.rotation;
        }

        transform.SetPositionAndRotation(nextPos, nextRot);
    }
}
