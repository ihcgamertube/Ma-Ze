using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "MazeWall")
            transform.Rotate(-70, transform.localRotation.y, transform.localRotation.z);
    }

    void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.tag == "MazeWall")
            transform.Rotate(-20, transform.localRotation.y, transform.localRotation.z);
    }
}
