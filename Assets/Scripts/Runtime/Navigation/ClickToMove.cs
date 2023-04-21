

using UnityEngine;

namespace Runtime.Navigation
{
    [ExecuteInEditMode]
    public class ClickToMove : MonoBehaviour
    {
        private NavAgent _agent;
        private NavAgent agent {
            get { return _agent != null ? _agent : _agent = GetComponent<NavAgent>(); }
        }

        void OnEnable() {
            agent.OnDestinationReached += OnDestinationReached;
        }

        void OnDisable() {
            agent.OnDestinationReached -= OnDestinationReached;
        }

        void OnDestinationReached() {
            Debug.Log("Click Destination Reached");
        }

        void Update() {
            if ( Input.GetMouseButton(0) ) {
                agent.SetDestination(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            }
        }
        
    }
}