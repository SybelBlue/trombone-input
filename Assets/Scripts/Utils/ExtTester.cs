using UnityEngine;

namespace Extracted
{
    public class ExtTester : MonoBehaviour
    {
        public Vector2 pos;
        public ExtTiltType tiltType;

        // Start is called before the first frame update
        void Start()
        {
            pos = new Vector2(0.5f, 0.5f);
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                pos.x += 0.05f;
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                pos.x -= 0.05f;
            }
            
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                pos.y += 0.05f;
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                pos.y -= 0.05f;
            }

            pos.x = (1 + pos.x) % 1;
            pos.y = (1 + pos.y) % 1;

            Vector3 data = new Vector3(pos.x, 0, pos.y);
            tiltType.UpdateHighlight(data);

            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                tiltType.useAlternate = !tiltType.useAlternate;
            }

            if (Input.GetKeyDown(KeyCode.Return))
            {
                Debug.Log($"Keypress {tiltType.GetSelectedLetter(data)}");
            }
        }
    }
}