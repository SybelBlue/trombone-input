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
    // transform.position = new Vector3(sty.position.x, transform.position.y, transform.position.z);
    // transform.position = new Vector3(transform.position.x, sty.position.y, transform.position.z);
    // transform.position = new Vector3(transform.position.x, transform.position.y, sty.position.z);
    transform.position = new Vector3(sty.position.x, sty.position.y, sty.position.z);
    // transform.rotation = Quaternion.Euler(sty.rotation.x, transform.rotation.y, transform.rotation.z);
    transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, sty.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);

    }
}
