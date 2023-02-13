
using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Runtime.Player
{
    public class PlayerController : MonoBehaviour
    {
        // ============ Public Fields ============
        [SerializeField] private float moveSpeed = 1f;
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private Animator animator;

        // ============ Movement System ============
        [NonSerialized] public bool AnimLocked = false;
        private Vector2 _movementInput;
        private Vector2 _smoothedInput;
        private Vector2 _smoothedVelocity;
        
        private void OnMove(InputValue movementValue)
        {   
            _movementInput = movementValue.Get<Vector2>();
        }
        
        private void FixedUpdate()
        {
            if (!AnimLocked && _movementInput != Vector2.zero)
            {
                animator.SetFloat("moveX", _movementInput.x);
                animator.SetFloat("moveY", _movementInput.y);
                animator.SetBool("isMoving", true);
            }
            else
            {
                animator.SetBool("isMoving", false);
            }
            
            _smoothedInput = Vector2.SmoothDamp(_smoothedInput, _movementInput, ref _smoothedVelocity, 0.1f);
            rb.velocity = _smoothedInput * moveSpeed;
        }
    }
}

