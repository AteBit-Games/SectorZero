/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/
using System.Collections.Generic;
using System.Linq;
using Runtime.InputSystem;
using Runtime.InteractionSystem.Interfaces;
using UnityEngine;

namespace Runtime.InteractionSystem
{
    public class PlayerInteraction : MonoBehaviour
    {
        [SerializeField] private LayerMask interactionMask;
        [SerializeField] private InputReader inputReader;
        
        //----- Private Variables -----//
        private CircleCollider2D _interactionRadius;
        private readonly List<GameObject> _interactables = new();

        //----- Hash Variables -----//
        private static readonly int OutlineThickness = Shader.PropertyToID("_Radius");
        private static readonly int OutlineColor = Shader.PropertyToID("_Color");
        
        //========================= Unity Events =========================//
        
        private void OnEnable()
        {
            inputReader.InteractEvent += Interact;   
        }

        private void OnDisable()
        {
            inputReader.InteractEvent -= Interact;
        }
        
        private void Awake()
        {
            _interactionRadius = GetComponent<CircleCollider2D>();
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (interactionMask != (interactionMask | (1 << other.gameObject.layer))) return;
            
            var interactable = other.GetComponent<IInteractable>();
            if (interactable != null)
            {
                _interactables.Add(other.gameObject);
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (interactionMask != (interactionMask | (1 << other.gameObject.layer))) return;

            var interactable = other.GetComponent<IInteractable>();
            if (interactable != null && _interactables.Contains(other.gameObject))
            {
                var spriteRenderer = other.GetComponent<SpriteRenderer>();
                spriteRenderer.material.SetFloat(OutlineThickness, 0f);
                _interactables.Remove(other.gameObject);
            }
        }

        private void FixedUpdate()
        {
            if(_interactables.Count == 0) return;

            foreach (var spriteRenderer in _interactables.Select(interactable => interactable.GetComponent<SpriteRenderer>()))
            {
                spriteRenderer.material.SetFloat(OutlineThickness, 0f);
            }
            
            var closest = GetClosestInteractable(_interactables.ToArray());
            if(closest != null)
            {
                var interactable = closest.GetComponent<IInteractable>();
                var spriteRenderer = closest.GetComponent<SpriteRenderer>();
                spriteRenderer.material.SetFloat(OutlineThickness, 1f);
                spriteRenderer.material.SetColor(OutlineColor, interactable.CanInteract() ? new Color(255f, 255f, 255f) : new Color(0.88f, 0.56f, 0f));
            }
        }

        public void RemoveInteractable(GameObject interactable)
        {
            
            if (interactable != null && _interactables.Contains(interactable))
            {
                var spriteRenderer = interactable.GetComponent<SpriteRenderer>();
                spriteRenderer.material.SetFloat(OutlineThickness, 0f);
                _interactables.Remove(interactable);
            }
        }

        //========================= Private Methods =========================//
        
        private void Interact()
        {
            if(_interactables.Count == 0) return;
            var closest = GetClosestInteractable(_interactables.ToArray())?.GetComponent<IInteractable>();
            
            if(closest != null && closest.CanInteract()) closest.OnInteract(gameObject);
            else if (closest != null && !closest.CanInteract()) closest.OnInteractFailed(gameObject);
        }

        private GameObject GetClosestInteractable(IEnumerable<GameObject> interactables)
        {
            GameObject bestTarget = null;
            float closestDistanceSqr = Mathf.Infinity;
            Vector3 currentPosition = _interactionRadius.bounds.center;
            
            foreach(var potentialTarget in interactables)
            {
                if(potentialTarget == null) continue;
                Vector3 directionToTarget = potentialTarget.transform.position - currentPosition;
                float dSqrToTarget = directionToTarget.sqrMagnitude;
                if(dSqrToTarget < closestDistanceSqr)
                {
                    closestDistanceSqr = dSqrToTarget;
                    bestTarget = potentialTarget;
                }
            }

            return bestTarget;
        }
    }
}

