/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/

using System.Collections;
using Cinemachine;
using Runtime.AI.Interfaces;
using Runtime.BehaviourTree;
using Runtime.InputSystem;
using Runtime.InventorySystem;
using Runtime.Managers;
using Runtime.Player.Nellient;
using Runtime.SaveSystem;
using Runtime.SaveSystem.Data;
using Runtime.Utils;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

namespace Runtime.Player
{
    
    public class PlayerController : MonoBehaviour, IPersistant, ISightEntity
    {
        [Space(10)]
        [Header("REFERENCES")]
        [SerializeField] private InputReader inputReader;
        
        [Space(10)]
        [Header("MOVEMENT DETAILS")]
        [SerializeField] private float moveSpeed = 1f;

        [Space(10)] 
        [Header("Look Details")] 
        [SerializeField] private Transform lookPointer;
        [SerializeField] private Collider2D lookDeadZone;
        [SerializeField] private Collider2D lookOuterBounds;
        
        [Space(10)] 
        [Header("Throwing System")]
        [SerializeField] private GameObject throwIndicator;
        [SerializeField] private LayerMask throwBoundsMask;

        [Space(10)]
        [Header("STEALTH DETAILS")]
        [SerializeField] private float sneakSpeed = 1f;
        [SerializeField] private Light2D globalLight;
        [SerializeField] private BoxCollider2D viewBounds;
        
        [Space(10)]
        [Header("SAVING SYSTEM")]
        [SerializeField] private string persistentID;
        public string ID
        {
            get => persistentID;
            set => persistentID = value;
        }

        [Space(10)]
        [Header("DEBUG")]
        [SerializeField] private bool debug;

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
        private Vector2 _lastPosition;
        private bool _sneaking;
        private bool _isDead;
        private bool _inputDisabled;
        private bool _cameraDisabled;

        // ============ Hiding System ============
       [HideInInspector] public bool isHiding;
       [HideInInspector] public bool isSeen;

       // ============ Animator Hashes ============
        private readonly int _moveX = Animator.StringToHash("moveX");
        private readonly int _moveY = Animator.StringToHash("moveY");
        private readonly int _isMoving = Animator.StringToHash("isMoving");
        private readonly int _isSneaking = Animator.StringToHash("isSneaking");
        
        // ============ Throwing System ============
        private bool _isAiming;
        private bool _canThrow;
        private SpriteRenderer _indicatorSpriteRenderer;
        private Rigidbody2D _indicatorRb;

        private void OnEnable()
        {
            inputReader.MoveEvent += HandleMove;
            inputReader.SneakEvent += HandleSneak;
            inputReader.AimEvent += HandleAim;
            inputReader.AimCancelEvent += HandleAimCancel;
            inputReader.LeftClickEvent += HandleThrow;
        }

        private void OnDisable()
        {
            inputReader.MoveEvent -= HandleMove;
            inputReader.SneakEvent -= HandleSneak;
            inputReader.AimEvent -= HandleAim;
            inputReader.AimCancelEvent -= HandleAimCancel;
            inputReader.LeftClickEvent -= HandleThrow;
        }

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _movementAnimator = GetComponent<Animator>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _playerShadow = GameObject.FindGameObjectWithTag("Shadow").GetComponent<SpriteRenderer>();
            _audioLowPassFilter = GetComponent<AudioLowPassFilter>();
            _monster = FindObjectOfType<BehaviourTreeOwner>(true);

            if (_monster != null)
            {
                _seenEnter = _monster.FindBlackboardKey<bool>("DidSeeEnter");
            }
            
            _indicatorSpriteRenderer = throwIndicator.GetComponent<SpriteRenderer>();
            _indicatorRb = throwIndicator.GetComponent<Rigidbody2D>();

            if (SceneManager.GetActiveScene().name == "Tutorial")
            {
                TutorialManager.StartListening("TutorialStage3", Init);
                gameObject.SetActive(false);
            }
        }

        private void HandleMove(Vector2 direction)
        {
            if(!_inputDisabled) _movementInput = direction;
        }


        private bool _sneakCooldownActive;
        private void HandleSneak()
        {
            if(!_sneakCooldownActive && !_inputDisabled)
            {
                _sneakCooldownActive = true;
                _sneaking = !_sneaking;
                _movementAnimator.SetBool(id: _isSneaking, _sneaking);
                if(_monster != null) _monster.isPlayerCrouching = _sneaking;
                SetViewBounds(_sneaking);
                StartCoroutine(SneakCooldown());
            }
        }

        private IEnumerator SneakCooldown()
        {
            yield return new WaitForSeconds(0.1f);
            _sneakCooldownActive = false;
        }

        private void SetViewBounds(bool sneaking)
        {
            if (!sneaking)
            {
                viewBounds.offset = new Vector2(-0.02924603f, 0.8556569f);
                viewBounds.size = new Vector2(0.787723303f,1.982361f);
            }
            else
            {
                viewBounds.offset = new Vector2(-0.1f, 0.531635f);
                viewBounds.size = new Vector2(1.5f, 1.36327f);
            }
        }
        
        private void FixedUpdate()
        {
            if(_isDead) return;
            
            var newPosition = _rb.position + _movementInput * ((_sneaking ? sneakSpeed : moveSpeed) * Time.fixedDeltaTime);
            _rb.MovePosition(newPosition);
            if(!_cameraDisabled) UpdateMouseLook();
            if(_isAiming) UpdateThrowIndicator();

            if (_movementInput != Vector2.zero)
            {
                _movementAnimator.SetFloat(id: _moveX, _movementInput.x);
                _movementAnimator.SetFloat(id: _moveY, _movementInput.y);
                _movementAnimator.SetBool(id: _isMoving, Vector2.Distance(_lastPosition, _rb.position) > 0.01f);
            }
            else
            {
                _movementAnimator.SetBool(id: _isMoving, false);
            }
            
            _lastPosition = _rb.position;
        }

        public void LoadData(SaveData data)
        {
            if (debug)
            {
                var nellient = FindObjectOfType<TutorialNellient>();
                if (nellient != null) nellient.gameObject.SetActive(false);
                Init();
            }

            FindObjectOfType<CinemachineTargetGroup>().transform.position = data.playerData.position;
            transform.position = data.playerData.position;
            lookPointer.position = new Vector2(transform.position.x, transform.position.y + 2.5f);
            gameObject.SetActive(data.playerData.enabled);
        }
        
        public void SaveData(SaveData data)
        {
            data.playerData.position = transform.position;
            data.playerData.enabled = gameObject.activeSelf;
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
        
        public void Die()
        {
            DisableInput();
            GameManager.Instance.DialogueSystem.CancelDialogue();
            _spriteRenderer.enabled = false;
            _playerShadow.enabled = false;
            lookPointer.position = gameObject.transform.position;
            _isDead = true;
        }

        public bool IsSeen { get; set; }

        public void RevealPlayer(Vector2 position, Vector2 facingDirection)
        {
            SetData(true);
            EnableInput();
            _movementAnimator.SetBool(id: _isSneaking, _sneaking);
            globalLight.intensity = 0.3f;
            transform.position = position;
            //_audioLowPassFilter.enabled = false;
            _seenEnter.value = false;
            
            if (facingDirection != Vector2.zero)
            {
                SetFacingDirection(facingDirection);
            }
        }
        
        private void SetFacingDirection(Vector2 direction)
        {
            _movementAnimator.SetFloat(id: _moveX, direction.x);
            _movementAnimator.SetFloat(id: _moveY, direction.y);
        }
        
        public void LookDown()
        {
            SetFacingDirection(Vector2.down);
        }

        private void UpdateMouseLook()
        {
            var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            var deadZoneBounds = lookDeadZone.bounds;
            var deadZoneMinX = deadZoneBounds.min.x;
            var deadZoneMaxX = deadZoneBounds.max.x;
            
            var deadZoneMinY = deadZoneBounds.min.y;
            var deadZoneMaxY = deadZoneBounds.max.y;
            
            //check if mouse is in dead zone
            if (mousePos.x > deadZoneMinX && mousePos.x < deadZoneMaxX && mousePos.y > deadZoneMinY && mousePos.y < deadZoneMaxY)
            {
                lookPointer.position = new Vector2(gameObject.transform.position.x, gameObject.transform.position.y+2.8f);
            }
            else
            {
                var outerBounds = lookOuterBounds.bounds;
                var outerBoundsMinX = outerBounds.min.x;
                var outerBoundsMaxX = outerBounds.max.x;

                var outerBoundsMinY = outerBounds.min.y;
                var outerBoundsMaxY = outerBounds.max.y;

                mousePos.x = Mathf.Clamp(mousePos.x, outerBoundsMinX, outerBoundsMaxX);
                mousePos.y = Mathf.Clamp(mousePos.y, outerBoundsMinY, outerBoundsMaxY);
                mousePos.z = 0;
            
                lookPointer.position = mousePos;
            }
        }
        
        private void HandleAim()
        {
            if(gameObject.GetComponent<PlayerInventory>().HasThrowableItem)
            {
                _isAiming = true;
                DisableMovement();
                _indicatorSpriteRenderer.enabled = true;
                UpdateThrowIndicator();
            }
        }

        private void HandleThrow()
        {
            if (_isAiming && _canThrow)
            {
                var inventory = gameObject.GetComponent<PlayerInventory>();

                inventory.GetThrowable().Throw(new Vector2(transform.position.x, transform.position.y + 1f), throwIndicator);
                inventory.DropThrowable();
                HandleAimCancel();
                GameManager.Instance.HUD.ShowThrowableIcon(false);
            }
        }

        private void HandleAimCancel()
        {
            _isAiming = false;
            EnableMovement();
            _indicatorSpriteRenderer.enabled = false;
        }
        
        private void UpdateThrowIndicator()
        {
            //move the game object to the mouse position based on rigidbody physics
            _indicatorRb.MovePosition(Camera.main.ScreenToWorldPoint(Input.mousePosition));

            //scale the indicator based on the distance from the player
            var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var distance = Vector2.Distance(transform.position, mousePos);
            var scale = Mathf.Clamp(distance/25, 0.5f, 1f);
            throwIndicator.transform.localScale = new Vector3(scale, scale, 1);

            //calculate the angle between the player and the mouse and set the rotation of the player animation of vector 2
            var direction = (mousePos - transform.position).normalized;
            _movementAnimator.SetFloat(id: _moveX, direction.x);
            _movementAnimator.SetFloat(id: _moveY, direction.y);
            
            //get list of all colliders that the indicator is overlapping
            var roomBound = Physics2D.OverlapPoint(throwIndicator.transform.position, throwBoundsMask);
            
            //if overlapping with a object with tag "RoomBounds" then change the color to red

            if (roomBound != null)
            {
                _canThrow = true;
                _indicatorSpriteRenderer.color = Color.white;
            }
            else
            {
                _canThrow = false;
                _indicatorSpriteRenderer.color = Color.red;
            };
        }

        // ==================== Helper Methods ====================
        
        private void SetData(bool state)
        {
            isHiding = !state;
            _movementAnimator.enabled = state;
            _spriteRenderer.enabled = state;
            _playerShadow.enabled = state;
        }

        public void DisableInput()
        {
            _inputDisabled = true;
            _movementInput = Vector2.zero;
        }

        public void EnableInput()
        {
            _inputDisabled = false;
        }
        
        public void SetCamera(bool disabled)
        {
            _cameraDisabled = disabled;
        }

        public void LookAt(Vector2 direction)
        {
            _movementAnimator.SetFloat(id: _moveX, direction.x);
            _movementAnimator.SetFloat(id: _moveY, direction.y);
        }

        private void DisableMovement()
        {
            _inputDisabled = true;
            inputReader.MoveEvent -= HandleMove;
            _movementInput = Vector2.zero;
        }

        private void EnableMovement()
        {
            _inputDisabled = false;
            inputReader.MoveEvent += HandleMove;
        }

        private void EnableLowPassFilter()
        {
            //_audioLowPassFilter.enabled = true;
        }

        private void Init()
        {
            gameObject.SetActive(true);
            _movementAnimator.SetFloat(id: _moveX, 1);
        }

        public bool IsSneaking => _sneaking;
        public bool MovementDisabled => _inputDisabled;
    }
}