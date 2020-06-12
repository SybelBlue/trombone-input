using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTheStylus : MonoBehaviour
{
  Transform sty;
    // Start is called before the first frame update
    void Start()
    {
        sty = GameObject.Find("Stylus").transform;
    }

    // Update is called once per frame
    void Update()
    {
    transform.position = new Vector3(sty.position.x, sty.position.y, sty.position.z);
    }
}
