/*
 * Create : 2021-05-11
 * Writer : lgs6108
 * Desc : 캐릭터 컨트롤러
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class character : MonoBehaviour
{
    public enum ControlType
    {
        KEYBOARD,
        JOYSTICK,
    }
    [SerializeField] ControlType _controlType = ControlType.KEYBOARD;
    [SerializeField] Transform _camTr;
    [SerializeField] Animator _animator;
    [SerializeField] Transform _viewPoint;
    [SerializeField] Joystick _joystick;

    public float _moveSpeed = 5f;
    public float _rotSpeed = 100f;

    void Update()
    {
        switch(_controlType)
        {
            case ControlType.KEYBOARD:
                var moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
                Control(moveInput);
                break;
            case ControlType.JOYSTICK:
                if (_joystick != null)
                    Control(_joystick._InputVector);
                else
                    Debug.Log("조이스틱 객체 없음");
                break;
        }
    }

    void Control(Vector3 moveVector)
    {
        //_animator.SetBool("isRun", true);
        var moveInput = moveVector;
        bool isMove = moveInput.magnitude != 0;
        if (isMove)
        {
            Vector3 lookForward = new Vector3(_camTr.forward.x, 0, _camTr.forward.z);
            Vector3 lookright = new Vector3(_camTr.right.x, 0, _camTr.right.z);
            Vector3 moveDirection = lookForward * moveInput.y + lookright * moveInput.x;

            Quaternion lookRot = Quaternion.LookRotation(moveDirection);
            Quaternion lerpRot = Quaternion.Lerp(transform.rotation, lookRot, Time.deltaTime * _rotSpeed);
            transform.rotation = lerpRot;
            transform.position += moveDirection * Time.deltaTime * _moveSpeed;
        }
        else
        {
            //_animator.SetBool("isRun", false);
        }
    }
}