using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    [SerializeField] Transform CameraPosition;

    // Update is called once per frame
    void Update()
    {
        //Sets the position of the camera to Camera position object to stop jittering
        transform.position = CameraPosition.position;
    }
}
