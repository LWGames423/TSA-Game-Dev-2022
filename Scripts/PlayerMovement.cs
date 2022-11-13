using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.Serialization;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class PlayerMovement : MonoBehaviour
{
    #region Variables

    private Vector2 _moveInput;
    private Rigidbody2D _rb;
    
    [Header("Movement")]
    public float moveSpeed;
    
    public float acceleration;
    public float deceleration;
    public float frictionAmount;
    
    public float velPower;
    
    [Header("Jump")]
    public float jumpForce;
    
    private bool _facingRight = true;

    private bool _isGrounded;

    private bool _canJump;
    private bool _isJumping;

    private readonly float _gravityScale = 1f;
    public float fallGravityMultiplier;

    public float jumpCutMultiplier;
    
    [Header("Checks")]
    public Transform groundCheck;
    
    public Vector2 checkRadius;
    public LayerMask groundLayer;

    [Header("Timer")] 
    public float coyoteTime;
    private float _coyoteTimeCounter;
    
    #endregion

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        // TODO: make the player flip when appropriate
        
        _facingRight = true;
    }
    
    private void Update()
    {
        #region InputChecks
        
        _moveInput.x = Input.GetAxisRaw("Horizontal");
        _moveInput.y = Input.GetAxisRaw("Vertical");

        #endregion

        #region Jump

        if (_moveInput.y > 0  && _canJump && _coyoteTimeCounter > 0 && _isJumping==false)
        {
            _coyoteTimeCounter = 0;
            _rb.velocity = new Vector2(_rb.velocity.x, jumpForce);
            _canJump = false;
            _isJumping = true;
            Jump();
        }

        if (_moveInput.y < 0.01 && _isJumping)
        {
            JumpUp();
        }

        #endregion

        #region Timer

        _coyoteTimeCounter -= Time.deltaTime;

        #endregion
        
    }

    

    private void FixedUpdate()
    {
        #region GroundCheck

        _isGrounded = Physics2D.OverlapBox(groundCheck.position, checkRadius, 0, groundLayer);
        if (_isGrounded)
        {
            _canJump = true;
            _isJumping = false;
            _coyoteTimeCounter = coyoteTime;
        }

        #endregion

        #region Jump Gravity

        if(_rb.velocity.y < 0)
        {
            _rb.gravityScale = _gravityScale * fallGravityMultiplier;
            _isJumping = false;
        }
        else
        {
            _rb.gravityScale = _gravityScale;
        }
        

        #endregion
        
        #region Run
        
        float targetSpeed = _moveInput.x * moveSpeed;
        float speedDif = targetSpeed - _rb.velocity.x;
        
        float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? acceleration : deceleration;
        float movement = Mathf.Pow(Mathf.Abs(speedDif) * accelRate, velPower) * Mathf.Sign(speedDif);

        _rb.AddForce(movement * Vector2.right);

        #endregion
        
        #region Friction

        if (_isGrounded && Mathf.Abs(_moveInput.x) < 0.01f)
        {
            float amount = Mathf.Min(Mathf.Abs(_rb.velocity.x), Mathf.Abs(frictionAmount));

            amount *= Mathf.Sign(_rb.velocity.x);
            
            _rb.AddForce(Vector2.right * -amount, ForceMode2D.Impulse);
        }
        
        #endregion
    }

    #region Flip
    
    void Flip()
    {
        _facingRight = !_facingRight;
        Vector3 scaler = transform.localScale;
        scaler.x *= -1;
        transform.localScale = scaler;
    }
    
    #endregion

    #region Jump Script

    void Jump()
    {
        _rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    void JumpUp()
    {
        if (_rb.velocity.y >0 && _isJumping)
        {
            _rb.AddForce(Vector2.down * _rb.velocity.y * (1-jumpCutMultiplier), ForceMode2D.Impulse);
        }
    }
    
    #endregion
}