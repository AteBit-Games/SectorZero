using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Runtime.Player
{
    public class PlayerController : MonoBehaviour
    {
        // ============ Public Fields ============
        [SerializeField] private float moveSpeed = 1f;
        [SerializeField] private float collisionOffset = 0.02f;
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private Animator animator;
        [SerializeField] private ContactFilter2D movementFilter;
        
        
        // ============ Movement System ============
        private Vector2 _movementInput;
        private bool _animLocked = false, _isMoving = false;
        private readonly List<RaycastHit2D> _castCollisions = new();
        
        private void FixedUpdate()
        {
            if(!_animLocked && _movementInput != Vector2.zero)
            { 
                animator.SetFloat("moveX", _movementInput.x);
                animator.SetFloat("moveY", _movementInput.y);
            }
            
            if (_movementInput != Vector2.zero)
            {
                var success = TryMove(_movementInput);
                _isMoving = true;

                if (!success)
                {
                    success = TryMove(new Vector2(_movementInput.x, 0));
                    
                    if(!success)
                    {
                        TryMove(new Vector2(0, _movementInput.y));
                    }
                }
                
                animator.SetBool("IsMoving", true);
            }
            else
            {
                _isMoving = false;
            }
            
            UpdateAnimation();
        }
        
        private void UpdateAnimation()
        {
            if (!_animLocked)
            {
                if (_movementInput != Vector2.zero)
                {
                    animator.Play("player_move");
                }
                else
                {
                    animator.Play("player_idle");
                }
            }
        }

        private bool TryMove(Vector2 direction)
        {
            var count = rb.Cast(
                direction, // X and Y values between -1 and 1 that represent the direction for the body to look for collisions
                movementFilter, // The settings that determine what the body will collide with such as layers
                _castCollisions, // List of collisions to store the found collisions into after the cast is completed
                moveSpeed * Time.fixedDeltaTime * collisionOffset); // The amount to cast based on movement and offset

            if (count == 0)
            {
                rb.MovePosition(rb.position + _movementInput * (moveSpeed * Time.fixedDeltaTime));
                return true;
            }
            else
            {
                return false;
            }
        }

        private void OnMove(InputValue movementValue)
        {   
            _movementInput = movementValue.Get<Vector2>();
        }
    }
}
