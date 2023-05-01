using System;
using System.Collections;
using UnityEngine;

namespace Runtime.AI
{
    public class Sentinel : MonoBehaviour
    {
        [Header("SENTINEL PROPERTIES")]
        [SerializeField] private float cooldown = 3f;
        [SerializeField] private float activeTime = 8f;
        //[SerializeField] private 
        
        [Space(10)]
        [Header("SIGHT PROPERTIES")]
        [Tooltip("The layer mask of the target.")]
        public LayerMask playerMask = -1;
        [Tooltip("The layer mask of obstacles.")]
        public LayerMask obstacleMask = -1;
        [Tooltip("The direction to look in.")]
        [Range(0f, 360f)]
        public float lookAngle;
        [Tooltip("Distance within which to look.")]
        public float maxDistance = 50f;
        [Tooltip("The view angle to use for the check.")]
        public float viewAngle = 70f;
        
        private bool _canSeePlayer;
        private bool _isActivated;
        private Vector2 _lastKnownPlayerPosition;
        
        private float _cooldownTimeLeft;
        private float _activeTimeLeft;
        
        private void Start()
        {
            StartCoroutine(TimeOutActivate());
        }

        private IEnumerator TimeOutActivate()
        {
            yield return new WaitForSeconds(4f);
            ActivateSentinel();
        }
        
        private void Update()
        {
            if (_isActivated)
            {
                if (_activeTimeLeft > 0f)
                {
                    _activeTimeLeft -= Time.deltaTime;
                    if (_activeTimeLeft <= 0f)
                    {
                        _isActivated = false;
                        _cooldownTimeLeft = cooldown;
                    }
                }
            }
            else
            {
                if (_cooldownTimeLeft > 0f)
                {
                    _cooldownTimeLeft -= Time.deltaTime;
                    if (_cooldownTimeLeft <= 0f)
                    {
                        _cooldownTimeLeft = 0f;
                    }
                }
                
                return;
            }
            
            _canSeePlayer = false;
            var targetsInViewRadius = Physics2D.OverlapCircleAll(transform.position, maxDistance, playerMask);
            foreach (var currentTarget in targetsInViewRadius)
            {
                var directionToTarget = (currentTarget.transform.position - transform.position).normalized;
                var lookDirection = Quaternion.Euler(0f, 0f, lookAngle) * Vector2.right;
                if (Vector2.Angle(lookDirection, directionToTarget) < viewAngle / 2f)
                {
                    float distanceToTarget = Vector2.Distance(transform.position, currentTarget.transform.position);
                    if (!Physics2D.Raycast(transform.position, directionToTarget, distanceToTarget, obstacleMask))
                    {
                        _canSeePlayer = true;
                        _lastKnownPlayerPosition = currentTarget.transform.position;
                        DeactivateSentinel();
                        Debug.Log($"CanSeeTarget: Target is within view angle and there is no obstacle in the way.");
                    }
                    
                    Debug.Log($"CanSeeTarget: Target is within view angle but there is an obstacle in the way.");
                }
            }
        }
        
        public void ActivateSentinel()
        {
            _isActivated = true;
            _activeTimeLeft = activeTime;
        }
        
        public void DeactivateSentinel()
        {
            _activeTimeLeft = 0f;
            _cooldownTimeLeft = cooldown;
            _isActivated = false;
        }
        
        public bool CanSeePlayer()
        {
            return _canSeePlayer;
        }
        
        public Vector2 GetLastKnownPlayerPosition()
        {
            return _lastKnownPlayerPosition;
        }
        
        public void OnDrawGizmosSelected()
        {
            //draw cone in direction of lookAngle with width of viewAngle
            var lookDirection = Quaternion.Euler(0f, 0f, lookAngle) * Vector2.right;
            var leftRayDirection = Quaternion.Euler(0f, 0f, lookAngle - viewAngle / 2f) * Vector2.right;
            var rightRayDirection = Quaternion.Euler(0f, 0f, lookAngle + viewAngle / 2f) * Vector2.right;
            
            Gizmos.color = _isActivated ? Color.red : Color.green;

            Gizmos.DrawRay(transform.position, lookDirection * maxDistance);
            Gizmos.DrawRay(transform.position, leftRayDirection * maxDistance);
            Gizmos.DrawRay(transform.position, rightRayDirection * maxDistance);
            
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(transform.position, maxDistance);
        }
    }
}
