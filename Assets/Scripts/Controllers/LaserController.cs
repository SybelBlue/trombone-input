using UnityEngine;

#pragma warning disable 108
#pragma warning disable 649
public class LaserController : MonoBehaviour
{

    [SerializeField]
    private LineRenderer renderer;

    void Update()
        => renderer.SetPosition(1, Vector3.forward * 60);
}
