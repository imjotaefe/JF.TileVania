using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float playerSpeed = 10f;
    [SerializeField] private float jumpSpeed = 10f;
    [SerializeField] private float climbSpeed = 10f;
    
    private Animator _animatior;
    private Vector2 _moveInput;
    private Rigidbody2D _rb;
    private CapsuleCollider2D _capsule;
    private BoxCollider2D _myfeet;
    private float _gravityScaleAtStart;
    
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _animatior = GetComponent<Animator>();
        _capsule = GetComponent<CapsuleCollider2D>();
        _myfeet = GetComponent<BoxCollider2D>();
        _gravityScaleAtStart = _rb.gravityScale;
    }

    void Update()
    {
        Run();
        FlipSprite();
        ClimbLadder();
    }

    private void Run()
    {
        var playerVelocity = new Vector2(_moveInput.x * playerSpeed, _rb.velocity.y);
        _rb.velocity = playerVelocity;
        
        bool playerHasHorizontalSpeed = Mathf.Abs(_rb.velocity.x) > Mathf.Epsilon;
        _animatior.SetBool("isRunning", playerHasHorizontalSpeed);
    }

    void OnMove(InputValue value)
    {
        _moveInput = value.Get<Vector2>();
    }

    void OnJump(InputValue value)
    {
        if (!_myfeet.IsTouchingLayers(LayerMask.GetMask("Ground"))) return;
        if (value.isPressed)
        {
            _rb.velocity += new Vector2(0f, jumpSpeed);
        }
    }

    void FlipSprite()
    {
        bool playerHasHorizontalSpeed = Mathf.Abs(_rb.velocity.x) > Mathf.Epsilon;
        if (playerHasHorizontalSpeed)
        {
            transform.localScale = new Vector2(Mathf.Sign(_rb.velocity.x), 1f);
        }
    }

    void ClimbLadder()
    {
        if (!_myfeet.IsTouchingLayers(LayerMask.GetMask("Climbing")))
        {
            _rb.gravityScale = _gravityScaleAtStart;
            _animatior.SetBool("isClimbing", false);
            return;
        }
        var climbVelocity = new Vector2(_rb.velocity.x, _moveInput.y * climbSpeed);
        _rb.velocity = climbVelocity;
        _rb.gravityScale = 0;
        bool playerHasVerticalSpeed = Mathf.Abs(_rb.velocity.y) > Mathf.Epsilon;
        _animatior.SetBool("isClimbing", playerHasVerticalSpeed);
    }
}
