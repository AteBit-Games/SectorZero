/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/

using System;
using System.Collections;
using Runtime.AI.Interfaces;
using Runtime.BehaviourTree;
using Runtime.InputSystem;
using Runtime.InteractionSystem.Interfaces;
using Runtime.InventorySystem;
using Runtime.Managers;
using Runtime.SaveSystem;
using Runtime.SaveSystem.Data;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

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

        [Space(10)]
        [Header("STEALTH DETAILS")]
        [SerializeField] private float sneakSpeed = 1f;
        [SerializeField] private Light2D globalLight;
        
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
        private SpriteRenderer _indicatorSpriteRenderer;
        private Coroutine _throwCoroutine;

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
                //_seenEnter = _monster.FindBlackboardKey<bool>("DidSeeEnter");
            }
            
            _indicatorSpriteRenderer = throwIndicator.GetComponent<SpriteRenderer>();

            if (SceneManager.GetActiveScene().name == "Tutorial")
            {
                TutorialManager.StartListening("TutorialStage3", Init);
                gameObject.SetActive(false);
            }

            if (debug)
            {
                var nellient = FindObjectOfType<Nellient>();
                if (nellient != null) nellient.gameObject.SetActive(false);
                Init();
            }
        }

        private void HandleMove(Vector2 direction)
        {
            _movementInput = direction;
        }
        
        private void HandleSneak()
        {
            Debug.Log(gameObject.name + " is sneaking");
            Debug.Log("Sneak");
            _sneaking = !_sneaking;
            _movementAnimator.SetBool(id: _isSneaking, _sneaking);
        }
        
        private void FixedUpdate()
        {
            var newPosition = _rb.position + _movementInput * ((_sneaking ? sneakSpeed : moveSpeed) * Time.fixedDeltaTime);
            _rb.MovePosition(newPosition);
            UpdateMouseLook();
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
        
        public void Die()
        {
            DisableInput();
            _spriteRenderer.enabled = false;
            _playerShadow.enabled = false;
        }

        public bool IsSeen { get; set; }

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
                lookPointer.position = gameObject.transform.position;
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
                UpdateThrowIndicator();
                _indicatorSpriteRenderer.enabled = true;
            }
        }

        private void HandleThrow()
        {
            if (_isAiming)
            {
                var inventory = gameObject.GetComponent<PlayerInventory>();
                Throw(inventory.GetThrowable());
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
            var rbIndicator = throwIndicator.GetComponent<Rigidbody2D>();
            rbIndicator.MovePosition(Camera.main.ScreenToWorldPoint(Input.mousePosition));

            //scale the indicator based on the distance from the player
            var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var distance = Vector2.Distance(transform.position, mousePos);
            var scale = Mathf.Clamp(distance/25, 0.5f, 1f);

            throwIndicator.transform.localScale = new Vector3(scale, scale, 1);
        }

        private void Throw(GameObject throwable)
        {
            //pick a random point in the circle
            var landingRange = throwIndicator.GetComponent<CircleCollider2D>();
            var randomPointInRange = Random.insideUnitCircle * landingRange.radius;
            var throwPosition = new Vector3(randomPointInRange.x, randomPointInRange.y, 0) + throwIndicator.transform.position;

            //Determine the distance of the throw
            var throwDistance = Vector2.Distance(throwPosition, gameObject.transform.position);
            var height = Mathf.Clamp(throwDistance/1.5f, 2f, 8f);

            //Enable the throwable and set its position to the origin of the player
            throwable.SetActive(true);
            throwable.transform.position = gameObject.transform.position;
            
            //Calculate the bezier curve
            var point = new Vector2[3];
            point[0] = gameObject.transform.position;
            point[2] = throwPosition;
            point[1] = point[0] +(point[2] -point[0])/2 + Vector2.up * height;
            
            //Based on higher distance have less bounces
            var bounces = Mathf.RoundToInt(3 - Mathf.Clamp(Mathf.RoundToInt(throwDistance/4), 1, 2));
            
            //Fall speed modifier
            var modifier = -Mathf.Clamp(throwDistance / 120, 0.065f, 0.07f);
            modifier += 0.1f;

            //Start the coroutine to move the throwable on the curve
            _throwCoroutine = StartCoroutine(ThrowCoroutine(0.0f, throwable, point, modifier, bounces, throwDistance));
        }

        private IEnumerator ThrowCoroutine(float bezierCount, GameObject throwable, Vector2[] points, float countModifier, int bounces, float distance)
        {
            if(bezierCount <= 1.0f)
            {
                var bezierPoint = Mathf.Pow(1.0f - bezierCount, 2) * points[0] + 2.0f * (1.0f - bezierCount) * bezierCount * points[1] + Mathf.Pow(bezierCount, 2) * points[2];
                throwable.transform.position = bezierPoint;
                
                if(bezierCount <= 0.5) countModifier = Mathf.Clamp(countModifier - 0.0006f, 0, 1);
                else countModifier = Mathf.Clamp(countModifier + 0.0003f, 0, 1);
                
                yield return new WaitForSeconds(0.02f);
                _throwCoroutine = StartCoroutine(ThrowCoroutine(bezierCount + countModifier, throwable, points, countModifier, bounces, distance));
            }
            else
            {
                throwable.GetComponent<IThrowable>().OnDrop(throwable.transform);

                if (bounces > 0)
                {
                    points[0] = throwable.transform.position;
                    points[2] += (points[0] - (Vector2)transform.position).normalized * distance/8;
                    points[1] = points[0] + (points[2] -points[0])/2 + Vector2.up * ((distance/6) * bounces);
                    StartCoroutine(ThrowCoroutine(0.0f, throwable, points, countModifier+0.04f, bounces-1, distance));
                }
                else
                {
                    StopCoroutine(_throwCoroutine);
                    _throwCoroutine = null;
                }
            }
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
            inputReader.MoveEvent -= HandleMove;
            inputReader.SneakEvent -= HandleSneak;
            _movementInput = Vector2.zero;
        }

        public void EnableInput()
        {
            inputReader.MoveEvent += HandleMove;
            inputReader.SneakEvent += HandleSneak;
        }

        private void DisableMovement()
        {
            inputReader.MoveEvent -= HandleMove;
            _movementInput = Vector2.zero;
        }

        private void EnableMovement()
        {
            inputReader.MoveEvent += HandleMove;
        }

        private void EnableLowPassFilter()
        {
            _audioLowPassFilter.enabled = true;
        }

        private void Init()
        {
            gameObject.SetActive(true);
            _movementAnimator.SetFloat(id: _moveX, 1);
        }
    }
}