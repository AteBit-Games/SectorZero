
using System;
using Runtime.Input;
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

        [SerializeField] private InputReader inputReader;
        [SerializeField] private float moveSpeed = 1f;
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private Animator movementAnimator;

        // ============ Movement System ============
        private Vector2 _movementInput;
        private static readonly int MoveX = Animator.StringToHash("moveX");
        private static readonly int MoveY = Animator.StringToHash("moveY");
        private static readonly int IsMoving = Animator.StringToHash("isMoving");

        private void Start()
        {
            inputReader.MoveEvent += HandleMove;
            inputReader.DodgeEvent += HandleDodge;
        }

        private void HandleMove(Vector2 direction)
        {
            _movementInput = direction;
        }
        
        private void HandleDodge()
        {
            Debug.Log("Dodge");
        }

        private void FixedUpdate()
        {
            if (_movementInput != Vector2.zero)
            {
                movementAnimator.SetFloat(id: MoveX, _movementInput.x);
                movementAnimator.SetFloat(id: MoveY, _movementInput.y);
                movementAnimator.SetBool(id: IsMoving, true);
            }
            else
            {
                movementAnimator.SetBool(id: IsMoving, false);
            }
            
            rb.velocity = _movementInput * moveSpeed;
        }
    }
}