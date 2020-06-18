using UnityEngine;

#pragma warning disable 108
#pragma warning disable 649
public class LaserController : MonoBehaviour
{
    [SerializeField]
    private StylusModelController modelController;

    [SerializeField]
    private LineRenderer renderer;

    void Update()
    {
        if (!transform.hasChanged) return;

        RaycastHit? hit;
        modelController.Raycast(out hit);

        renderer.SetPosition(1, (hit?.distance ?? 50) * Vector3.forward);
    }
}
