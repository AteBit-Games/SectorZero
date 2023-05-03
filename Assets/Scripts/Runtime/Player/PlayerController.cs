/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/
using Runtime.BehaviourTree;
using Runtime.InputSystem;
using Runtime.SaveSystem;
using Runtime.SaveSystem.Data;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Runtime.Player
{
    public class PlayerController : MonoBehaviour, IPersistant
    {
        #region Header MOVEMENT DETAILS
        [Space(10)]
        [Header("REFERENCES")]
        #endregion
        [SerializeField] private InputReader inputReader;
        [SerializeField] private BehaviourTreeOwner behaviourTreeOwner;
        
        #region Header MOVEMENT DETAILS
        [Space(10)]
        [Header("MOVEMENT DETAILS")]
        #endregion
        [SerializeField] private float moveSpeed = 1f;

        #region Header MOVEMENT DETAILS
        [Space(10)]
        [Header("STEALTH DETAILS")]
        #endregion
        [SerializeField] private float sneakSpeed = 1f;
        [SerializeField] private Light2D globalLight;

        // ============ Movement System ============
        private Rigidbody2D _rb;
        private Animator _movementAnimator;
        private SpriteRenderer _spriteRenderer;
        private Vector2 _movementInput;
        private bool _sneaking;

        // ============ Hiding System ============
       [HideInInspector] public bool isHiding;
       [HideInInspector] public bool isSeen;
        
        // ============ Animator Hashes ============
        private readonly int _moveX = Animator.StringToHash("moveX");
        private readonly int _moveY = Animator.StringToHash("moveY");
        private readonly int _isMoving = Animator.StringToHash("isMoving");
        private readonly int _isSneaking = Animator.StringToHash("isSneaking");
        
        private void OnEnable()
        {
            inputReader.MoveEvent += HandleMove;
            inputReader.SneakEvent += HandleSneak;
        }

        private void OnDisable()
        {
            inputReader.MoveEvent -= HandleMove;
            inputReader.SneakEvent -= HandleSneak;
        }

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _movementAnimator = GetComponent<Animator>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void HandleMove(Vector2 direction)
        {
            _movementInput = direction;
        }
        
        private void HandleSneak()
        {
            _sneaking = !_sneaking;
            _movementAnimator.SetBool(id: _isSneaking, _sneaking);
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
            
            _rb.MovePosition(_rb.position + _movementInput * ((_sneaking ? sneakSpeed : moveSpeed) * Time.fixedDeltaTime));
        }
        
        public void LoadData(SaveData data)
        {
            transform.position = data.playerPosition;
        }
        
        public void SaveData(SaveData data)
        {
            data.playerPosition = transform.position;
        }
        
        public void HidePlayer(Vector2 position)
        {
            DisableInput();
            isHiding = true;
            behaviourTreeOwner.SetBlackboardValue("Is Player Hiding", isHiding);
            globalLight.intensity = 0.25f;
            _movementAnimator.SetBool(id: _isSneaking, false);
            _movementAnimator.SetBool(id: _isMoving, false);
            _movementAnimator.enabled = false;
            _spriteRenderer.enabled = false;
            transform.position = position;
        }
        
        public void RevealPlayer(Vector2 position)
        {
            EnableInput();
            isHiding = false;
            behaviourTreeOwner.SetBlackboardValue("Is Player Hiding", isHiding);
            globalLight.intensity = 0.3f;
            _movementAnimator.enabled = true;
            _spriteRenderer.enabled = true;
            transform.position = position;
        }
        
        public void DisableInput()
        {
            inputReader.MoveEvent -= HandleMove;
            inputReader.SneakEvent -= HandleSneak;
        }

        public void EnableInput()
        {
            inputReader.MoveEvent += HandleMove;
            inputReader.SneakEvent += HandleSneak;
        }

    }
}