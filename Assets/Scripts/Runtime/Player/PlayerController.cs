/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/

using Runtime.AI.Interfaces;
using Runtime.BehaviourTree;
using Runtime.InputSystem;
using Runtime.InteractionSystem.Interfaces;
using Runtime.SaveSystem;
using Runtime.SaveSystem.Data;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Runtime.Player
{
    public class PlayerController : MonoBehaviour, IPersistant, ISightEntity
    {
        #region Header MOVEMENT DETAILS
        [Space(10)]
        [Header("REFERENCES")]
        #endregion
        [SerializeField] private InputReader inputReader;

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
        private SpriteRenderer _playerShadow;
        private SpriteRenderer _spriteRenderer;
        private AudioLowPassFilter _audioLowPassFilter;

        private BehaviourTreeOwner _monster;
        private BlackboardKey<bool> _seenEnter;
        [HideInInspector] public Collider2D hideable;

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
            _playerShadow = GameObject.FindGameObjectWithTag("Shadow").GetComponent<SpriteRenderer>();
            _audioLowPassFilter = GetComponent<AudioLowPassFilter>();
            _monster = FindObjectOfType<BehaviourTreeOwner>();
            
            _seenEnter = _monster.FindBlackboardKey<bool>("DidSeeEnter");
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
        
        public void HidePlayer(GameObject hideable, Vector2 position)
        {
            if(IsSeen) _seenEnter.value = true;
            
            SetData(false);
            DisableInput();
            _movementAnimator.SetBool(id: _isMoving, false);
            globalLight.intensity = 0.2f;
            transform.position = position;
            this.hideable = hideable.GetComponent<Collider2D>();
            Invoke(nameof(EnableLowPassFilter), 1f);
        }
        
        private void EnableLowPassFilter()
        {
            _audioLowPassFilter.enabled = true;
        }
        
        public void RevealPlayer(Vector2 position)
        {
            SetData(true);
            EnableInput();
            _movementAnimator.SetBool(id: _isSneaking, _sneaking);
            globalLight.intensity = 0.3f;
            transform.position = position;
            _audioLowPassFilter.enabled = false;
            _seenEnter.value = false;
        }

        private void SetData(bool state)
        {
            isHiding = !state;
            _movementAnimator.enabled = state;
            _spriteRenderer.enabled = state;
            _playerShadow.enabled = state;
        }

        private void DisableInput()
        {
            inputReader.MoveEvent -= HandleMove;
            inputReader.SneakEvent -= HandleSneak;
            _movementInput = Vector2.zero;
        }

        private void EnableInput()
        {
            inputReader.MoveEvent += HandleMove;
            inputReader.SneakEvent += HandleSneak;
        }

        public void Die()
        {
            DisableInput();
            _spriteRenderer.enabled = false;
            _playerShadow.enabled = false;
        }

        public bool IsSeen { get; set; }
    }
}