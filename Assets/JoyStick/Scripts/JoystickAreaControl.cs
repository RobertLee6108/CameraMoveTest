using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class JoystickAreaControl : MonoBehaviour , IBeginDragHandler,IDragHandler,IEndDragHandler, IPointerDownHandler
{
    [SerializeField]
    RectTransform _controlArea;
    [SerializeField]
    RectTransform _touchArea;

    [SerializeField]
    Image _image;



    public void OnBeginDrag(PointerEventData eventData)
    {
        //_image.rectTransform.localPosition = eventData.position;
        _isStart = true;
        _startPos = Input.mousePosition;
    }

    public void OnDrag(PointerEventData eventData)
    {
        //_image.rectTransform.anchoredPosition = eventData.position;
        _endPos = Input.mousePosition;
        Vector3 dir = (_endPos - _startPos).normalized;
        Debug.Log($"벡터 : {dir}");
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _isStart = false;
        Debug.Log($"끗");
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        ////Debug.Log($"클릭 포지션 : {eventData.position}");
        ////_image.rectTransform.anchoredPosition = eventData.position;
        //
        //Debug.Log($"이미지 앵커포지션 : {_image.rectTransform.anchoredPosition}");
        //_image.rectTransform.anchoredPosition = eventData.position;
    }

    Vector2 _startPos;
    Vector2 _endPos;

    bool _isStart = false;

    void Update()
    {
        if(Input.touchCount == 2)
        {
            Touch touchFirst = Input.GetTouch(0);
            Touch touchSecond = Input.GetTouch(1);

            Vector2 touchFirstPrevPos = touchFirst.position - touchFirst.deltaPosition;
            Vector2 touchSecondPrevPos = touchFirst.position - touchSecond.deltaPosition;

            float prevTouchDeltaMag = (touchFirstPrevPos - touchSecondPrevPos).magnitude;
            float touchDeltaMag = (touchFirst.position - touchSecond.position).magnitude;

            float deltaMag = prevTouchDeltaMag - touchDeltaMag;

            //deltaMag값을 카메라 포워드 값에 곱해준다.
        }
/*
        if (Input.GetMouseButtonDown(0))
        {
            _isStart = true;
            _startPos = Input.mousePosition;
        }
        if (Input.GetMouseButton(0)&& _isStart)
        {
            _endPos = Input.mousePosition;
            Vector3 dir = (_endPos - _startPos).normalized;
            Debug.Log($"벡터 : {dir}");
        }

        if (Input.GetMouseButtonUp(0))
        {
            _isStart = false;


        }
*/
    }
}
