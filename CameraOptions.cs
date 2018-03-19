using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraOptions : MonoBehaviour {

    public void RotateLeft()
    {
        transform.Rotate(Vector3.up, 30, Space.Self);
    }

    public void RotateRight()
    {
        transform.Rotate(Vector3.up, -30, Space.Self);
    }
}
