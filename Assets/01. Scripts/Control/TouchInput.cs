using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchInput : MonoBehaviour
{
    [SerializeField] Vector3 _inputVector;
    public Vector3 _InputVector
    {
        internal set { _inputVector = value; }
        get { return _inputVector; }
    }


}
