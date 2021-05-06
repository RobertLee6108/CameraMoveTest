/*
 * Create : 2021-01-27
 * Writer : lgs6108
 * Desc : 핸드폰 모델 뷰 씬에서 카메라 컨트롤 해주는 스크립트
 */
using UnityEngine;

public class CameraAxis : MonoBehaviour
{
    public enum RotationType
    {
        //0,0,0
        ZERO,
        //45,45,0
        QUATERVIEW,
        //_customRotation
        CUSTOM,
    }
    [SerializeField]
    RotationType _defaultRotationType = RotationType.ZERO;
    /// <summary>
    /// 카메라 Transform
    /// </summary>
    [SerializeField]
    Transform _camTranform;
    /// <summary>
    /// 카메라 컴포넌트
    /// </summary>
    [SerializeField]
    Camera _cam;

    [SerializeField]
    Vector3 _mouseGap;
    [SerializeField]
    Vector3 _mouseGapPos;

    [SerializeField,Range(1f,10f),Tooltip("모델 회전 속도")]
    float _rotationSpeed = 5f;
    
    [Header("확대관련 변수"),SerializeField]
    float _maxZoomDistance = 2f;
    [SerializeField]
    float _minZoomDistance = 1f;
    [SerializeField,Range(1f,10f)]
    float _WheelSpeed = 5f;
    [SerializeField]
    float _zoomDistance = 1f;
    [SerializeField]
    float _defaultZoomDistance = 1f;
    [SerializeField]
    bool _isLockMove = false;

    [Header("회전관련 변수"),SerializeField]
    float _minXRotation = -90f;
    [SerializeField]
    float _maxXRotation = 90f;
    [SerializeField]
    Vector3 _customRotation = new Vector3(0, 0, 0);
    [Space(20)]
    [SerializeField,Tooltip("True일 때 X 축 회전각도 제한")]
    bool _useRotationLimit = true;
    [SerializeField]
    bool _inverseXRotation = true;
    [SerializeField]
    bool _inverseYRotation = false;

    [SerializeField]
    float _offsetFromWall = 0.3f;
    [SerializeField]
    character _character;
    private void Start()
    {
        init();
    }
    /// <summary>
    /// 초기화 함수
    /// </summary>
    void init()
    {
        if(_camTranform == null)
        {
            ChangeCamTransForm("ModelViewerCamera");
        }
        _cam = _camTranform.GetComponent<Camera>();
        setRotation(_defaultRotationType);
    }

    private void LateUpdate()
    //private void Update()
    {
        if (_camTranform == null) return;
        //오른쪽 클릭 시 회전
        if (Input.GetMouseButton(1))
        {
            //_inverseYRotation 에 따라 마우스 오른쪽 드래그와 
            //왼쪽 드래그 시 회전 방향이 바뀜
            if (_inverseXRotation)
                _mouseGap.x += Input.GetAxis("Mouse Y") * _rotationSpeed * -1;
            else
                _mouseGap.x += Input.GetAxis("Mouse Y") * _rotationSpeed;

            if (_inverseYRotation)
                _mouseGap.y += Input.GetAxis("Mouse X") * _rotationSpeed * -1;
            else
                _mouseGap.y += Input.GetAxis("Mouse X") * _rotationSpeed;
            //_useRotationLimit true면 카메라 X축회전 각도가 _minXRotation ~ _maxXRotation사이로 고정 됨
            if (_useRotationLimit)
                _mouseGap.x = Mathf.Clamp(_mouseGap.x, _minXRotation, _maxXRotation);

            transform.rotation = Quaternion.Euler(_mouseGap);
        }
        //왼쪽 클릭 시 해당 지점으로 이동
        //if (Input.GetMouseButtonDown(0))
        //{
        //    //Debug.Log("????");
        //    Ray ray = _cam.ScreenPointToRay(Input.mousePosition);
        //    RaycastHit posHit;
        //
        //    if (Physics.Raycast(ray, out posHit, Mathf.Infinity, LayerMask.GetMask("Ground")))
        //    {
        //        Debug.DrawRay(_cam.transform.position, _cam.transform.forward, Color.red);
        //        _character.moveToDestination(posHit.point);
        //        Debug.Log($"layer : { LayerMask.LayerToName(posHit.collider.gameObject.layer)}");
        //    }
        //}
        //오른쪽 클릭 시 상하좌우로 이동
        if (Input.GetMouseButton(3) && !_isLockMove)
        {
            _mouseGapPos.x = Input.GetAxis("Mouse X") * -1;
            _mouseGapPos.y = Input.GetAxis("Mouse Y") * -1;

            transform.Translate(_mouseGapPos * 0.01f, Space.Self);
        }

        //마우스 휠에 따라 카메라가 앞뒤로 이동
        Vector3 axisBack = transform.forward * -1;
        _zoomDistance += Input.GetAxis("Mouse ScrollWheel") * -1 * _WheelSpeed;
        _zoomDistance = Mathf.Clamp(_zoomDistance, _minZoomDistance, _maxZoomDistance);

        //벽이나 땅에 가까울 때 카메가 이동
        //Axis에서 카메라로 향하는 방향 계산 
        var dir = (_camTranform.position - transform.position).normalized;
        //Axis와 카메라 사이의 거리 계산
        var distance = Vector3.Distance(transform.position, _camTranform.position);
        //레이캐스트 맞을 레이어 지정
        var mask = LayerMask.GetMask("wall") | LayerMask.GetMask("Ground");
        //레이캐스트 거리 지정
        var rayDist = distance > _zoomDistance ? _zoomDistance : distance + 0.1f;

        RaycastHit hit;
        //if (Physics.Raycast(transform.position, dir, out hit, distance + 0.5f, mask))
        if (Physics.Raycast(transform.position, dir, out hit, rayDist, mask))
        {
            _camTranform.position = hit.point + (dir * -0.05f);
            Debug.DrawRay(hit.point, _camTranform.forward * rayDist, Color.green);
            //Debug.Log("벽 맞음");
        }
        else
        {
            _camTranform.position = transform.position + axisBack * _zoomDistance;
            //Debug.Log("벽 안 맞음");
        }

    }

    /// <summary>
    /// 카메라 회전 상태를 DefaultRotationType에 따라 돌림
    /// </summary>
    /// <param name="rotType"></param>
    void setRotation(RotationType rotType)
    {
        switch (rotType)
        {
            case RotationType.ZERO:
                transform.rotation = Quaternion.identity;
                _mouseGap = Vector3.zero;
                break;
            case RotationType.QUATERVIEW:
                Vector3 rotation = new Vector3(45f, 45f, 0f);
                transform.Rotate(rotation);
                _mouseGap = rotation;
                break;
            case RotationType.CUSTOM:
                transform.Rotate(_customRotation);
                _mouseGap = _customRotation;
                break;
        }
    }

    /// <summary>
    /// 카메라의 _customRotation을 정의
    /// </summary>
    /// <param name="eulerAngles">회전할 각도</param>
    public void setCustomRotation(Vector3 Angles)
    {
        _customRotation = Angles;
    }

    /// <summary>
    /// 트랜스폼으로 카메라 교체
    /// </summary>
    /// <param name="camTransForm">카메라 Transform 값</param>
    public void ChangeCamTransForm(Transform camTransform)
    {
        _camTranform = camTransform;
    }

    /// <summary>
    /// 태그명으로 카메라 교체
    /// </summary>
    /// <param name="camTag">카메라 태그 string 값</param>
    public void ChangeCamTransForm(string camTag)
    {
        _camTranform = GameObject.FindWithTag(camTag).transform;
    }

    /// <summary>
    /// 초기 세팅으로 돌림
    /// </summary>
    public void resetPropertys()
    {
        //임시 값
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        _camTranform.localPosition = Vector3.zero;
        _camTranform.localRotation = Quaternion.identity;
        _zoomDistance = _defaultZoomDistance;
        _mouseGap = Vector3.zero;
        _mouseGapPos = Vector3.zero;
    }

    /// <summary>
    /// 모델뷰 카메라를 On/Off 해준다.
    /// </summary>
    /// <param name="active">True이면 카메라를 켜줌</param>
    public void setActiveCam(bool active)
    {
        _cam.enabled = active;
    }

    public Vector3 getCamPosition()
    {
        return _camTranform.position;
    }
    public Vector3 getForwardVector(bool onlyForward = true)
    {
        if (!onlyForward)
            return _camTranform.forward;
        else
        {
            return new Vector3(_camTranform.forward.x, 0,  _camTranform.forward.z);
        }
    }
    public Vector3 getRightVector(bool onlyRight = true)
    {
        if (!onlyRight)
            return _camTranform.right;
        else
        {
            return new Vector3(_camTranform.right.x, 0, _camTranform.right.z);
        }
    }
}
