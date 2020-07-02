using UnityEngine;

public class MainBackground : MonoBehaviour
{
    public void Start()
        => GetComponent<Canvas>().worldCamera = Camera.main;
}
