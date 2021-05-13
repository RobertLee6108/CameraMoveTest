using UnityEngine.EventSystems;
using UnityEngine;

public class Joystick : MonoBehaviour ,IDragHandler,IBeginDragHandler,IEndDragHandler
{
    [SerializeField]
    RectTransform _backGround;
    [SerializeField]
    RectTransform _joystickHandle;

    [SerializeField] Vector3 _inputVector;
    /// <summary>
    /// 조이스틱 방향 값
    /// </summary>
    public Vector3 _InputVector
    {
        internal set { _inputVector = value; }
        get {
            if (_isInput)
                return _inputVector;
            else
                return Vector3.zero;
        }
    }
    [SerializeField]
    bool _isInput = false;

    //public void OnPointerDown(PointerEventData eventData)
    //{
    //    OnDrag(eventData);
    //    _isInput = true;
    //}

    public void OnDrag(PointerEventData eData)
    {
        Vector2 touchPos;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(_backGround, eData.position, eData.pressEventCamera, out touchPos))
        {
            touchPos.x = (touchPos.x / _backGround.sizeDelta.x);
            touchPos.y = (touchPos.y / _backGround.sizeDelta.y);
                        
            _inputVector = new Vector3(touchPos.x * 2, touchPos.y * 2, 0);
            _inputVector = (_inputVector.magnitude > 1.0f) ? _inputVector.normalized : _inputVector;

            _joystickHandle.anchoredPosition = new Vector3(_inputVector.x * (_backGround.sizeDelta.x / 2)
                                                            , _inputVector.y * (_backGround.sizeDelta.y / 2));
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _inputVector = Vector3.zero;
        _joystickHandle.anchoredPosition = Vector2.zero;
        _isInput = false;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Vector2 touchPos;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(_backGround, eventData.position, eventData.pressEventCamera, out touchPos))
        {
            touchPos.x = (touchPos.x / _backGround.sizeDelta.x);
            touchPos.y = (touchPos.y / _backGround.sizeDelta.y);

            //_inputVector = new Vector3(touchPos.x * 2 + 1, touchPos.y * 2 - 1, 0);
            _inputVector = new Vector3(touchPos.x * 2, touchPos.y * 2, 0);
            _inputVector = (_inputVector.magnitude > 1.0f) ? _inputVector.normalized : _inputVector;

            _joystickHandle.anchoredPosition = new Vector3(_inputVector.x * (_backGround.sizeDelta.x / 2)
                                                            , _inputVector.y * (_backGround.sizeDelta.y / 2));
        }
        _isInput = true;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _inputVector = Vector3.zero;
        _joystickHandle.anchoredPosition = Vector2.zero;
        _isInput = false;
    }
}