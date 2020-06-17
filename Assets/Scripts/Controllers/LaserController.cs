using UnityEngine;

#pragma warning disable 108
public class LaserController : MonoBehaviour
{

    [SerializeField]
    private LineRenderer renderer;

    void Update()
        => renderer.SetPosition(1, transform.forward * 60);
}
