/*
 * Create : 2021-03-22
 * Writer : lgs6108
 * Desc : 씬에 해당 클래스가 존재하면 PhoneViewerUI를 켜준다.
 * RoomEditor 씬에서 추가 가능
 */
using UnityEngine;

public class ModelViewrUISwitcher : MonoBehaviour
{
    static ModelViewrUISwitcher _instance = null;

    [SerializeField]
    MeshRenderer _renderer;

    [SerializeField]
    Collider _collider;

    private void Awake()
    {
        if (_instance == null)
            _instance = this;
        else
            Destroy(gameObject);

        _collider = GetComponent<Collider>();


        if (_renderer != null)
        {
            if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "RoomEditor")
            {
                _renderer.enabled = false;
                _collider.enabled = false;
            }
            
                
        }
    }

    private void OnDestroy()
    {
        _instance = null;
    }
}
