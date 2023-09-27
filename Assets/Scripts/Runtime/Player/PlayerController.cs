/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/

using System.Collections;
using Cinemachine;
using Runtime.AI;
using Runtime.AI.Interfaces;
using Runtime.BehaviourTree;
using Runtime.InputSystem;
using Runtime.Managers;
using Runtime.Managers.Tutorial;
using Runtime.SaveSystem;
using Runtime.SaveSystem.Data;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

namespace Runtime.Player
{
    [DefaultExecutionOrder(6)]
    public class PlayerController : MonoBehaviour, IPersistant, ISightEntity
    {
        [Space(10)]
        [Header("REFERENCES")]
        [SerializeField] private InputReader inputReader;
        
        [Space(10)]
        [Header("MOVEMENT DETAILS")]
        [SerializeField] private float moveSpeed = 1f;

        [Space(10)] 
        [Header("CAMERA DETAILS")] 
        [SerializeField] private Transform lookPointer;
        [SerializeField] private Collider2D lookDeadZone;
        [SerializeField] private Collider2D lookOuterBounds;

        [Space(10)]
        [Header("STEALTH DETAILS")]
        [SerializeField] private float sneakSpeed = 1f;
        [SerializeField] private float sneakCooldown = 1f;
        [SerializeField] private Light2D globalLight;
        [SerializeField] private BoxCollider2D viewBounds;

        // ============ Movement System ============
        private Rigidbody2D _rb;
        private Animator _movementAnimator;
        private SpriteRenderer _playerShadow;
        private SpriteRenderer _spriteRenderer;
        
        // ============ Miscellaneous ============
        private BehaviourTreeOwner _monster;
        private BlackboardKey<bool> _seenEnter;
        private BlackboardKey<int> _activeState;

        [HideInInspector] public Collider2D hideable;
        private AudioLowPassFilter _audioLowPassFilter;

        // ============ Variables Tracking ============
        private Vector2 _movementInput;
        private Vector2 _lastSeenPosition;
        private bool _isDead;
        private bool _movementDisabled;
        private bool _inputDisabled;
        private bool _cameraDisabled;
        [HideInInspector] public bool isSneaking;

        // ============ Hiding System ============
        public bool IsSeen { get; set; }
        public bool InputDisabled => _inputDisabled;
        
        [HideInInspector] public bool isHiding;
        private Coroutine _hideCoroutine;
        private bool _sneakCooldownActive;


       // ============ Animator Hashes ============
        private readonly int _moveX = Animator.StringToHash("moveX");
        private readonly int _moveY = Animator.StringToHash("moveY");
        private readonly int _isMoving = Animator.StringToHash("isMoving");
        private readonly int _isSneaking = Animator.StringToHash("isSneaking");
        
        // =========================== Unity Events ===========================

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

            //Setup Monster
            _monster = FindFirstObjectByType<BehaviourTreeOwner>(FindObjectsInactive.Include);
            if (_monster != null)
            {
                _seenEnter = _monster.FindBlackboardKey<bool>("DidSeeEnter");
            }

            if (SceneManager.GetActiveScene().name == "Tutorial") TutorialManager.StartListening("TutorialStage3", Init);
        }
        
        private void LateUpdate()
        {
            if(_isDead) return;
            
            if(!_cameraDisabled) UpdateMouseLook();
        }
        
        private void FixedUpdate()
        {
            if(_isDead) return;
            
            //Move the player
            var newPosition = _rb.position + _movementInput * ((isSneaking ? sneakSpeed : moveSpeed) * Time.fixedDeltaTime);
            _rb.MovePosition(newPosition);

            if (_movementInput != Vector2.zero)
            {
                _movementAnimator.SetFloat(id: _moveX, _movementInput.x);
                _movementAnimator.SetFloat(id: _moveY, _movementInput.y);
                _movementAnimator.SetBool(id: _isMoving, Vector2.Distance(_lastSeenPosition, _rb.position) > 0.01f);
            }
            else
            {
                _movementAnimator.SetBool(id: _isMoving, false);
            }
            
            _lastSeenPosition = _rb.position;
        }
        
         //================================== Public Methods ==================================
        
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
        
        public void HidePlayer(GameObject hidableObject, Vector2 position)
        {
            //Hide the player
            SetVisible(false);
            DisableMovement();
            
            //Set the player to the hiding position
            _movementAnimator.SetBool(id: _isMoving, false);
            globalLight.intensity = 0.2f;
            transform.position = position;
            hideable = hidableObject.GetComponent<Collider2D>();
            
            //Let monster know the player is hiding if they were seen
            if (IsSeen)
            {
                _seenEnter.value = true;
                _monster.SetState(State.AggroInspect);
            }
            
            //Start the hiding coroutine
            _hideCoroutine = StartCoroutine(EnableLowPass());
        }
        
        public void RevealPlayer(Vector2 position, Vector2 facingDirection)
        {
            //Show the player
            SetVisible(true);
            EnableMovement();
            
            //Set the player to the hiding position
            _movementAnimator.SetBool(id: _isSneaking, isSneaking);
            globalLight.intensity = 0.3f;
            transform.position = position;

            if (_seenEnter.value)
            {
                _monster.SetState(State.AggroChase);
            }
            
            _audioLowPassFilter.enabled = false;
            if (facingDirection != Vector2.zero) SetFacingDirection(facingDirection);
            if(_hideCoroutine != null) StopCoroutine(_hideCoroutine);
        }

        public void SetFacingDirection(Vector2 direction)
        {
            _movementAnimator.SetFloat(id: _moveX, direction.x);
            _movementAnimator.SetFloat(id: _moveY, direction.y);
        }
        
        public void LookDown()
        {
            SetFacingDirection(Vector2.down);
        }
        
        public void Die()
        {
            _isDead = true;

            //Disable the player
            DisableInput();
            _movementInput = Vector2.zero;
            _spriteRenderer.enabled = false;
            _playerShadow.enabled = false;
            
            //Reset Systems
            GameManager.Instance.DialogueSystem.CancelDialogue();
            lookPointer.position = gameObject.transform.position + new Vector3(-2f, 2.5f, 0f);
        }
        
        // =========================== Input System Methods ===========================

        private void HandleMove(Vector2 direction)
        {
            if(!_movementDisabled && !_inputDisabled) _movementInput = direction;
        }
        
        private void HandleSneak()
        {
            if(!_sneakCooldownActive && !_inputDisabled)
            {
                _sneakCooldownActive = true;
                isSneaking = !isSneaking;
                _movementAnimator.SetBool(id: _isSneaking, isSneaking);
                
                //Let monster know the player is crouching
                GameManager.Instance.AIManager.isPlayerCrouching = isSneaking;
                
                SetViewBounds(isSneaking);
                StartCoroutine(SneakCooldown());
            }
        }
        
        //================================== Save System ==================================

        public string LoadData(SaveGame game)
        {
            FindFirstObjectByType<CinemachineTargetGroup>().transform.position = game.playerData.position;
            transform.position = game.playerData.position;
            lookPointer.position = new Vector2(transform.position.x, transform.position.y + 2.5f);
            gameObject.SetActive(game.playerData.enabled);
            
            return "Player";
        }

        public void SaveData(SaveGame game)
        {
            game.playerData.position = transform.position;
            game.playerData.enabled = gameObject.activeSelf;
        }

        // ==================== Helper Methods ====================
        
        private void Init()
        {
            gameObject.SetActive(true);
            _movementAnimator.SetFloat(id: _moveX, 1);
        }
        
        private void SetVisible(bool state)
        {
            isHiding = !state;
            _movementAnimator.enabled = state;
            _spriteRenderer.enabled = state;
            _playerShadow.enabled = state;
        }

        private void DisableMovement()
        {
            _movementDisabled = true;
            _movementInput = Vector2.zero;
        }

        private void EnableMovement()
        {
            _movementDisabled = false;
            _movementInput = Vector2.zero;
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
        
        // ==================== Coroutines ====================
        
        private IEnumerator SneakCooldown()
        {
            yield return new WaitForSeconds(sneakCooldown);
            _sneakCooldownActive = false;
        }
        
        private IEnumerator EnableLowPass()
        {
            yield return new WaitForSeconds(0.8f);
            _audioLowPassFilter.enabled = true;
        }
    }
}