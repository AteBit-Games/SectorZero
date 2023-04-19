/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/
using Runtime.InputSystem;
using Runtime.SaveSystem;
using Runtime.SaveSystem.Data;
using UnityEngine;

namespace Runtime.Player
{
    public class PlayerController : MonoBehaviour, IPersistant
    {
        #region Header MOVEMENT DETAILS
        [Space(10)]
        [Header("MOVEMENT DETAILS")]
        #endregion
        [SerializeField] private InputReader inputReader;
        [SerializeField] private float moveSpeed = 1f;

        // ============ Movement System ============
        private Rigidbody2D _rb;
        private Animator _movementAnimator;
        private Vector2 _movementInput;
        private readonly int _moveX = Animator.StringToHash("moveX");
        private readonly int _moveY = Animator.StringToHash("moveY");
        private readonly int _isMoving = Animator.StringToHash("isMoving");

        private void Start()
        {
            inputReader.MoveEvent += HandleMove;
            _rb = GetComponent<Rigidbody2D>();
            _movementAnimator = GetComponent<Animator>();
        }

        private void HandleMove(Vector2 direction)
        {
            _movementInput = direction;
        }

        
        private void FixedUpdate()
        {
            if (_movementInput != Vector2.zero)
            {
                _movementAnimator.SetFloat(id: _moveX, _movementInput.x);
                _movementAnimator.SetFloat(id: _moveY, _movementInput.y);
                _movementAnimator.SetBool(id: _isMoving, true);
            }
            else
            {
                _movementAnimator.SetBool(id: _isMoving, false);
            }
            
            _rb.velocity = _movementInput * moveSpeed;
        }
        
        public void LoadData(SaveData data)
        {
            transform.position = data.playerPosition;
        }
        
        public void SaveData(SaveData data)
        {
            data.playerPosition = transform.position;
        }
    }
}