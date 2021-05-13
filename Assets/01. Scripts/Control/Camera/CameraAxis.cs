/*
 * Create : 2021-05-11
 * Writer : lgs6108
 * Desc : 카메라 Axis 컨트롤러
 * _controlType 타입에 따라서 마우스, 터치조작 가능(controlType 분리 필요?)
 */
using UnityEngine;

public class CameraAxis : MonoBehaviour
{
    public enum ControlType
    {
        MOUSE,
        TOUCH,
    }
    [SerializeField]
    ControlType _controlType = ControlType.MOUSE;
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
    Vector3 _rotationGap;

    [SerializeField,Range(1f,10f),Tooltip("카메ㄹ회전 속도")]
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
    
    [Space(20)]
    [SerializeField,Tooltip("True일 때 X 축 회전각도 제한")]
    bool _useRotationLimit = true;
    [SerializeField]
    bool _inverseXRotation = true;
    [SerializeField]
    bool _inverseYRotation = false;

    public bool _startInput = false;
    public Vector2 _inputStartVector = Vector2.zero;
    public Vector2 _directionVector = Vector2.zero;
    
    [SerializeField]
    float _halfScreenWidth;
    /// <summary>
    /// 벽이나 바닥 offset 거_zoomDistance
    /// </summary>
    [Range(0,3)]
    public float _surfaceDistanceOffset = 0.2f;

    private void Start()
    {
        init();
    }
    /// <summary>
    /// 초기화 함수
    /// </summary>
    void init()
    {
        _cam = _camTranform.GetComponent<Camera>();
        _halfScreenWidth = Screen.width / 2;
    }
        
    private void Update()
    {
        if (_camTranform == null) return;

        switch (_controlType)
        {
            case ControlType.MOUSE:
                MouseControl();
                break;
            case ControlType.TOUCH:
                TouchControl();
                break;
        }
        //벽이나 땅에 가까울 때 카메가 이동
        //Axis에서 카메라로 향하는 방향 계산 
        var dir = (_camTranform.position - transform.position).normalized;
        //Axis와 카메라 사이의 거리 계산
        var distance = (_camTranform.position - transform.position).magnitude;
        //레이캐스트 맞을 레이어 지정
        var mask = LayerMask.GetMask("wall") | LayerMask.GetMask("Ground");
        //레이캐스트 거리 지정
        var rayDist = distance < _zoomDistance ? _zoomDistance : distance;
        RaycastHit hit;
        if(Physics.Raycast(transform.position, dir, out hit, rayDist, mask))
        {
            _camTranform.position = hit.point + (-dir * _surfaceDistanceOffset);
            Debug.DrawRay(hit.point, _camTranform.forward * rayDist, Color.green);
            //Debug.Log($"벽 맞음");
        }
        else
        {
            Vector3 axisBack = transform.forward * -1;
            _zoomDistance += Input.GetAxis("Mouse ScrollWheel") * -1 * _WheelSpeed;
            _zoomDistance = Mathf.Clamp(_zoomDistance, _minZoomDistance, _maxZoomDistance);
            _camTranform.position = transform.position + axisBack * _zoomDistance;
        }
    }
    void MouseControl()
    {
        //오른쪽 클릭 시 회전
        if (Input.GetMouseButton(1))
        {
            //_inverseYRotation 에 따라 마우스 오른쪽 드래그와 
            //왼쪽 드래그 시 회전 방향이 바뀜
            if (_inverseXRotation)
                _rotationGap.x += Input.GetAxis("Mouse Y") * _rotationSpeed * -1;
            else
                _rotationGap.x += Input.GetAxis("Mouse Y") * _rotationSpeed;

            if (_inverseYRotation)
                _rotationGap.y += Input.GetAxis("Mouse X") * _rotationSpeed * -1;
            else
                _rotationGap.y += Input.GetAxis("Mouse X") * _rotationSpeed;
            //_useRotationLimit true면 카메라 X축회전 각도가 _minXRotation ~ _maxXRotation사이로 고정 됨
            if (_useRotationLimit)
                _rotationGap.x = Mathf.Clamp(_rotationGap.x, _minXRotation, _maxXRotation);

            transform.rotation = Quaternion.Euler(_rotationGap);
        }
        //마우스 휠에 따라 _zoomDistance값 조controlType
        Vector3 axisBack = transform.forward * -1;
        _zoomDistance += Input.GetAxis("Mouse ScrollWheel") * -1 * _WheelSpeed;
        _zoomDistance = Mathf.Clamp(_zoomDistance, _minZoomDistance, _maxZoomDistance);
    }
    void TouchControl()
    {
        TouchInput();

        if (_inverseXRotation)
            _rotationGap.x += _directionVector.y * _rotationSpeed * -1;
        else
            _rotationGap.x += _directionVector.y * _rotationSpeed;

        if (_inverseYRotation)
            _rotationGap.y += _directionVector.x * _rotationSpeed * -1;
        else
            _rotationGap.y += _directionVector.x * _rotationSpeed;
        //_useRotationLimit true면 카메라 X축회전 각도가 _minXRotation ~ _maxXRotation사이로 고정 됨
        if (_useRotationLimit)
            _rotationGap.x = Mathf.Clamp(_rotationGap.x, _minXRotation, _maxXRotation);

        transform.rotation = Quaternion.Euler(_rotationGap);
    }
    void TouchInput()
    {
        for (int i = 0; i < Input.touchCount; i++)
        {
            Touch t = Input.GetTouch(i);
            switch (t.phase)
            {
                //터치 시작 시 _inputStartVector에 포지션 저장
                case TouchPhase.Began:
                    if (_halfScreenWidth < t.position.x)
                    {
                        _startInput = true;
                        _inputStartVector = t.position;
                    }
                    break;
                //터치가 끝나거나 의도치 못하게 종료 됐을 시 zero 벡터로 초기화
                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    _inputStartVector = Vector2.zero;
                    _directionVector = Vector2.zero;
                    _startInput = false;
                    break;
                //방향벡터 구해서 저장 해둠
                case TouchPhase.Moved:
                    if (_halfScreenWidth < t.position.x)
                    {
                        var delta = t.deltaPosition * Time.deltaTime;
                        //Debug.Log(delta);
                        _directionVector = delta;
                    }
                    break;
                //터치 후 드래그 하지 않고 가만 있을때 zero 벡터로 초기화
                case TouchPhase.Stationary:
                    _directionVector = Vector2.zero;
                    break;
            }
        }
    }   
}