using UnityEngine;

public class TouchPointController : MonoBehaviour
{
    public bool active;
    public int touchIndex;

    void Update()
    {
        if (active = Input.touchCount > touchIndex)
        {
            var fTC = Input.GetTouch(touchIndex).position;
            transform.position = Camera.main.ScreenToWorldPoint(fTC);
        }
    }

    
}
