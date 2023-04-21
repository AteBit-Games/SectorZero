using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.Serialization;

namespace Runtime.Navigation
{
    [DisallowMultipleComponent]
    [AddComponentMenu("Navigation/NavAgent")]
    public class NavAgent : MonoBehaviour
    {
        // =============================== PROPERTIES ===============================
        
        [Header("MAP")]
        [SerializeField]
        private NavMap map;
        
        [Header("MOVEMENT")]
        public float maxSpeed = 3.5f;
        [Tooltip("Acceleration is applied when the agent is moving towards the goal.")]
        public float maxForce = 10f;
        [Tooltip("The distance to stop at from the goal.")]
        public float stoppingDistance = 0.1f;
        [Tooltip("The distance to start slowing down")]
        public float slowingDistance = 1;
        [Tooltip("The lookahead distance for Slowing down and agent avoidance | Performance Hit")]
        public float lookAheadDistance = 1;

        [Header("AVOIDANCE")]
        [Tooltip("The avoidance radius of the agent. 0 for no avoidance.")]
		public float avoidRadius ;
        [Tooltip("The max time in seconds where the agent is actively avoiding before considered 'stuck'.")]
        public float avoidanceConsiderStuckTime = 3f;
        [Tooltip("The max remaining path distance which will be considered reached, when the agent is 'stuck'.")]
        public float avoidanceConsiderReachedDistance = 1f;

        [Header("ROTATION")]
        [Tooltip(">Rotate transform as well?")]
		public bool rotateTransform;
        [Tooltip("Speed to rotate at moving direction.")]
        public float rotateSpeed = 350;

        [Header("OPTIONS")]
        [Tooltip(">Custom center offset from original transform position.")]
        public Vector2 centerOffset = Vector2.zero;
        [Tooltip("Should the agent repath? Disable for performance.")]
        public bool repath = true;
        [Tooltip("Should the agent be forced restricted within valid areas? Disable for performance.")]
        public bool restrict;
        [Tooltip("Go to closer point if requested destination is invalid? Disable for performance.")]
        public bool closerPointOnInvalid = true;
        [Tooltip("Will debug the path (gizmos). Disable for performance.")]
        public bool debugPath = true;
        
        // =============================== EVENTS ===============================
        
        public event Action OnNavigationStarted;
        public event Action OnDestinationReached;
        public event Action OnDestinationInvalid;
        public event Action<Vector2> OnNavigationPointReached;
        private event Action<bool> ReachedCallback;
        
        // =============================== PRIVATE VARIABLES ===============================
        
        private Vector2 _currentVelocity = Vector2.zero;
        private int _requests;
        private List<Vector2> _activePath = new();

        private static readonly List<NavAgent> AllAgents = new();
        
        // =============================== PUBLIC PROPERTIES ===============================
        
        public Vector2 Position 
        {
            get => transform.position + (Vector3)centerOffset;
            set => transform.position = new Vector3(value.x, value.y, transform.position.z) - (Vector3)centerOffset;
        }
        
        public List<Vector2> ActivePath {
            get => _activePath;
            set
            {
                _activePath = value;
                if(_activePath.Count > 0 && _activePath[0] == Position) _activePath.RemoveAt(0);
            }
        }
        
        public Vector2 PrimeGoal { get; set; } = Vector2.zero;

        public bool PathPending => _requests > 0;
        
        public NavMap Map {
            get => map != null ? map : NavMap.Current;
            set => map = value;
        }
        
        public bool HasPath => ActivePath.Count > 0;
        
        public Vector2 NextPoint => HasPath ? ActivePath[0] : Position;

        public float RemainingDistance {
            get
            {
                if(!HasPath) return 0;

                float dist = Vector2.Distance(Position, ActivePath[0]);
                for(int i = ActivePath.Count - 1; i >= 0; i--) 
                {
                    dist += Vector2.Distance(ActivePath[i], ActivePath[i == ActivePath.Count - 1 ? i : i + 1]);
                }

                return dist;
            }
        }
        
        public Vector2 MovingDirection => HasPath ? _currentVelocity.normalized : Vector2.zero;
        
        public float CurrentSpeed => _currentVelocity.magnitude;
        
        public bool IsAvoiding { get; private set; }
        
        public float AvoidingElapsedTime { get; private set; }

        // =============================== UNITY METHODS ===============================

        private void OnEnable()
        {
            AllAgents.Add(this);
        }

        public void OnDisable()
        {
            AllAgents.Remove(this);
        }

        private void Awake() 
        {
            PrimeGoal = Position;
            if (map == null) 
            {
                map = FindObjectsOfType<NavMap>().FirstOrDefault(m => m.PointIsValid(Position));
            }
        }

        private void LateUpdate() 
        {
            if(Map == null) return;
            
            if(!HasPath)
            {
                Restrict();
                return;
            }

            if(maxSpeed <= 0)
            {
                return;
            }

            var targetVelocity = _currentVelocity;
            if ( RemainingDistance < slowingDistance ) 
            {
                targetVelocity += Arrive(NextPoint);
            }
            else
            {
                targetVelocity += Seek(NextPoint);
            }
            
            _currentVelocity = Vector2.MoveTowards(_currentVelocity, targetVelocity, maxForce * Time.deltaTime);
            _currentVelocity = Vector2.ClampMagnitude(_currentVelocity, maxSpeed);
            
            LookAhead();
            Position += _currentVelocity * Time.deltaTime;
            

            if(IsAvoiding && AvoidingElapsedTime >= avoidanceConsiderStuckTime) 
            {
                if(RemainingDistance > avoidanceConsiderReachedDistance)
                {
                    OnInvalid();
                }
                else 
                {
                    OnArrived();
                }
            }
            
            Restrict();
            
            if(rotateTransform)
            {
                float rot = -Mathf.Atan2(MovingDirection.x, MovingDirection.y) * 180 / Mathf.PI;
                float newZ = Mathf.MoveTowardsAngle(transform.localEulerAngles.z, rot, rotateSpeed * Time.deltaTime);
                transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, newZ);
            }

            if(repath) 
            {
                if(Map.CheckLos(Position, NextPoint) == false) Repath();
                
                if(!HasPath) 
                {
                    OnArrived();
                    return;
                }
            }
            
            if(HasPath) 
            {
                float proximity = ( ActivePath[^1] == NextPoint ) ? stoppingDistance : 0.001f;
                if((Position - NextPoint ).magnitude <= proximity)
                {
                    ActivePath.RemoveAt(0);

                    if (!HasPath) 
                    {
                        OnArrived();
                        return;
                    }
                    else 
                    {
                        if(repath) 
                        {
                            Repath();
                        }

                        OnNavigationPointReached?.Invoke(Position);
                    }
                }
            }
            
            if(ActivePath.Count > 1 && Map.CheckLos(Position, ActivePath[1])) 
            {
                ActivePath.RemoveAt(0);
                OnNavigationPointReached?.Invoke(Position);
            }
        }
        
        // =============================== PUBLIC METHODS ===============================
        
        public bool SetDestination(Vector2 goal, Action<bool> callback = null) 
        {
            if(Map == null) 
            {
                Debug.LogError("No NavMap assigned and none exists in the scene!");
                return false;
            }
            
            if((goal - PrimeGoal ).magnitude < Mathf.Epsilon) return true;

            ReachedCallback = callback;
            PrimeGoal = goal;
            
            if((goal - Position ).magnitude < stoppingDistance)
            {
                OnArrived();
                return true;
            }
            
            if(!Map.PointIsValid(goal)) 
            {
                if(closerPointOnInvalid)
                {
                    SetDestination(Map.GetCloserEdgePoint(goal), callback);
                    return true;
                }
                else 
                {
                    OnInvalid();
                    return false;
                }
            }
            
            if (_requests > 0) return true;
            
            _requests++;
            Map.FindPath(Position, goal, SetPath);
            return true;
        }
        
        public void Stop() 
        {
            ActivePath.Clear();
            _currentVelocity = Vector2.zero;
            _requests = 0;
            PrimeGoal = Position;
            AvoidingElapsedTime = 0;
        }
        
        // =============================== PRIVATE METHODS ===============================

        private void SetPath(Vector2[] path) 
        {
            if(_requests == 0) return;

            _requests--;

            if(path == null || path.Length == 0) 
            {
                OnInvalid();
                return;
            }

            ActivePath = path.ToList();
            OnNavigationStarted?.Invoke();
        }

        private Vector2 Seek(Vector2 target)
        {
            var desiredVelocity = (target - Position).normalized * maxSpeed;
            var steer = desiredVelocity - _currentVelocity;
            return steer;
        }

        public Vector2 Arrive(Vector2 target) 
        {
            var desiredVelocity = ( target - Position ).normalized * maxSpeed;
            desiredVelocity *= RemainingDistance / slowingDistance;
            var steer = desiredVelocity - _currentVelocity;
            return steer;
        }

        private void LookAhead() 
        {
            if(lookAheadDistance <= 0 || !Map.PointIsValid(Position)) return;

            var currentLookAheadDistance = Mathf.Lerp(0, lookAheadDistance, _currentVelocity.magnitude / maxSpeed);
            var lookAheadPos = Position + ( _currentVelocity.normalized * currentLookAheadDistance );

            Debug.DrawLine(Position, lookAheadPos, Color.blue);

            if(!Map.PointIsValid(lookAheadPos)) 
            {
                _currentVelocity -= (lookAheadPos - Position);
            }
            
            if(avoidRadius > 0) 
            {
                IsAvoiding = false;
                foreach (var otherAgent in AllAgents)
                {
                    if (otherAgent == this || otherAgent.avoidRadius <= 0) 
                    {
                        continue;
                    }

                    var mlt = otherAgent.avoidRadius + this.avoidRadius;
                    var dist = ( lookAheadPos - otherAgent.Position ).magnitude;
                    var str = ( lookAheadPos - otherAgent.Position ).normalized * mlt;
                    var steer = Vector3.Lerp(str, Vector3.zero, dist / mlt);
                    if (!IsAvoiding ) IsAvoiding = steer.magnitude > 0;
                    _currentVelocity += ((Vector2)steer) * _currentVelocity.magnitude;

                    Debug.DrawLine(otherAgent.Position, otherAgent.Position + str, new Color(1, 0, 0, 0.1f));
                }

                AvoidingElapsedTime = IsAvoiding ? AvoidingElapsedTime += Time.deltaTime : 0;
            }
        }
        
        private void OnArrived() 
        {
            Stop();
            ReachedCallback?.Invoke(true);
            OnDestinationReached?.Invoke();
        }
        
        private void OnInvalid() 
        {
            Stop();
            ReachedCallback?.Invoke(false);
            OnDestinationInvalid?.Invoke();
        }

        private void Repath() 
        {
            if ( _requests > 0 ) return;
            _requests++;
            Map.FindPath(Position, PrimeGoal, SetPath);
        }

        //keep agent within valid area
        private void Restrict() 
        {
            if(!restrict) return;

            if ( !Map.PointIsValid(Position) ) {
                Position = Map.GetCloserEdgePoint(Position);
            }
        }
        
        // =============================== EDITOR METHODS ===============================
        
#if UNITY_EDITOR

        public void OnDrawGizmos() 
        {
            Gizmos.color = new Color(1, 1, 1, 0.1f);
            Gizmos.DrawWireSphere(Position, avoidRadius);

            if(!HasPath )return;

            if(debugPath) 
            {
                Gizmos.color = new Color(1f, 1f, 1f, 0.2f);
                Gizmos.DrawLine(Position, ActivePath[0]);
                for(int i = 0; i < ActivePath.Count; i++) 
                {
                    Gizmos.DrawLine(ActivePath[i], ActivePath[( i == ActivePath.Count - 1 ) ? i : i + 1]);
                }
            }
        }

#endif
        
    }
}
