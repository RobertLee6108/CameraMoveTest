using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class character : MonoBehaviour
{
    public CameraAxis _axisCam;
    public Animator _animator;
    public Transform _viewPoint;
    public float _moveSpeed = 5f;
    public float _rotSpeed = 100f;
    public bool _isJoystickControl = true;
    public Joystick _joystick;

    private void Start()
    {
        //_move += move;
    }
    public void move(Vector3 moveVec)
    {
        Debug.Log("움직");
        run(moveVec);
    }

    void Update()
    {
        if (!_isJoystickControl)
            run();
        else
        {
            var input = _joystick._InputVector;
            run(input);
        }
    }

    void run()
    {
        _animator.SetBool("isRun", true);
        //var moveInput = _joystick.
        var moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        bool isMove = moveInput.magnitude != 0;
        if (isMove)
        {
            var lookForward = _axisCam.getForwardVector();
            var lookright = _axisCam.getRightVector();
            var moveDirection = lookForward * moveInput.y + lookright * moveInput.x;
            //var lookRot = Quaternion.LookRotation(lookForward);
            var lookRot = Quaternion.LookRotation(moveDirection);
            var lerpRot = Quaternion.Lerp(transform.rotation, lookRot, Time.deltaTime * _rotSpeed);
            transform.rotation = lerpRot;
            transform.position += moveDirection * Time.deltaTime * _moveSpeed;
        }
        else
        {
            _animator.SetBool("isRun", false);
        }
    }

    void run(Vector3 moveVector)
    {
        _animator.SetBool("isRun", true);
        //var moveInput = _joystick.
        var moveInput = moveVector;
        bool isMove = moveInput.magnitude != 0;
        if (isMove)
        {
            var lookForward = _axisCam.getForwardVector();
            var lookright = _axisCam.getRightVector();
            var moveDirection = lookForward * moveInput.y + lookright * moveInput.x;
            //var lookRot = Quaternion.LookRotation(lookForward);
            var lookRot = Quaternion.LookRotation(moveDirection);
            var lerpRot = Quaternion.Lerp(transform.rotation, lookRot, Time.deltaTime * _rotSpeed);
            transform.rotation = lerpRot;
            transform.position += moveDirection * Time.deltaTime * _moveSpeed;
        }
        else
        {
            _animator.SetBool("isRun", false);
        }
    }

    public void moveToDestination(Vector3 dest)
    {
        StopAllCoroutines();
        StartCoroutine(coMoveToDestination(dest));
    }
    IEnumerator coMoveToDestination(Vector3 dest)
    {
        _isJoystickControl = false;
        _animator.SetBool("isRun", true);
        while (true)
        {
            var lookVec = dest - transform.position;
            var lookVecNormal = lookVec.normalized;
            transform.position += lookVecNormal * Time.deltaTime * 2f;

            var lookRot = Quaternion.LookRotation(lookVec);
            transform.rotation = lookRot;
            if (lookVec.magnitude < 0.3f)
            {
                _animator.SetBool("isRun", false);
                break;
            }

            yield return null;
        }
        _isJoystickControl = true;
    }

}
