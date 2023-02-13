
using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Runtime.Player
{
    public class PlayerController : MonoBehaviour
    {
        #region Header MOVEMENT DETAILS
        [Space(10)]
        [Header("MOVEMENT DETAILS")]
        #endregion
        
        [Tooltip("The speed multiplier for the player's movement")]
        [SerializeField] private float moveSpeed = 1f;
        
        [Tooltip("Reference to the player's rigidbody2D component")]
        [SerializeField] private Rigidbody2D rb;
        
        [Tooltip("Reference to the player's animator component")]
        [SerializeField] private Animator animator;

        // ============ Movement System ============
        [NonSerialized] public bool AnimLocked = false;
        private Vector2 _movementInput;
        private Vector2 _smoothedInput;
        private Vector2 _smoothedVelocity;
        
        // ============ Movement System ============
        private static readonly int MoveX = Animator.StringToHash("moveX");
        private static readonly int MoveY = Animator.StringToHash("moveY");
        private static readonly int IsMoving = Animator.StringToHash("isMoving");

        private void OnMove(InputValue movementValue)
        {   
            _movementInput = movementValue.Get<Vector2>();
        }
        
        private void FixedUpdate()
        {
            if (!AnimLocked && _movementInput != Vector2.zero)
            {
                animator.SetFloat(id: MoveX, _movementInput.x);
                animator.SetFloat(id: MoveY, _movementInput.y);
                animator.SetBool(id: IsMoving, true);
            }
            else
            {
                animator.SetBool(id: IsMoving, false);
            }
            
            _smoothedInput = Vector2.SmoothDamp(_smoothedInput, _movementInput, ref _smoothedVelocity, 0.1f);
            rb.velocity = _smoothedInput * moveSpeed;
        }
    }
}