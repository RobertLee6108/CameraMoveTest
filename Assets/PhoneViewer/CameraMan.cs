using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMan : MonoBehaviour
{
    [SerializeField]
    CameraAxis _axis;

    [SerializeField]
    Transform _target;

    Vector3 temp;

    // Update is called once per frame
    private void Update()
    {
        /*Vector3.SmoothDamp(transform.position,target.position)*/

        //transform.position = Vector3.SmoothDamp(transform.position, _target.position, ref temp, 1f);

        transform.position += (_target.position - transform.position)/16;
    }
}
