using UnityEngine;

public class CameraMan : MonoBehaviour
{
    [SerializeField]
    Transform _target;

    private void LateUpdate()
    {
        transform.position += (_target.position - transform.position);
    }
}
