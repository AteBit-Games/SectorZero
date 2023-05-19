/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/

using System;
using System.Collections.Generic;
using Runtime.InputSystem;
using Runtime.InteractionSystem.Interfaces;
using UnityEngine;

namespace Runtime.InteractionSystem
{
    public class PlayerInteraction : MonoBehaviour
    {
        #region Header MOVEMENT DETAILS
        [Space(10)]
        [Header("MOVEMENT DETAILS")]
        #endregion
        [SerializeField] private CircleCollider2D interactionRadius;
        [SerializeField] private LayerMask interactionMask;
        [SerializeField] private InputReader inputReader;
        
        private List<IInteractable> _interactables = new List<IInteractable>();

        private static readonly int OutlineThickness = Shader.PropertyToID("_OutlineThickness");
        private static readonly int OutlineColor = Shader.PropertyToID("_OutlineColor");

        private void OnTriggerEnter2D(Collider2D col)
        {
            //check if the collider that triggered was the interaction radius
            //if (col.
        }
        //
        // private void OnEnable()
        // {
        //     inputReader.InteractEvent += Interact;   
        // }
        //
        // private void OnDisable()
        // {
        //     inputReader.InteractEvent -= Interact;
        // }
        //
        // private void Awake()
        // {
        //     _castPosition = GetComponent<Collider2D>();
        // }
        //
        // private void LateUpdate()
        // {
        //     _colliderCount = Physics2D.OverlapCircleNonAlloc(transform.position, interactionRadius, _colliders, interactionMask);
        //     if(_colliderCount == 0) return;
        //     
        //     foreach (var collider1 in _colliders)
        //     {
        //         if (collider1 == null) continue;
        //         var spriteRenderer = collider1.gameObject.GetComponent<SpriteRenderer>();
        //         var distance = Vector2.Distance(_castPosition.bounds.center, collider1.transform.position);
        //         spriteRenderer.material.SetFloat(OutlineThickness, distance <= interactionRadius ? 1f : 0f);
        //     }
        // }
        //
        // private void Update()
        // {
        //     _colliderCount = Physics2D.OverlapCircleNonAlloc(transform.position, interactionRadius, _colliders, interactionMask);
        //     if(_colliderCount == 0) return;
        //     
        //     var closest = GetClosestInteractable(_colliders);
        //     if(closest == null) return;
        //     
        //     var interactable = closest.GetComponent<IInteractable>();
        //     var spriteRenderer = closest.gameObject.GetComponent<SpriteRenderer>();
        //     spriteRenderer.material.SetColor(OutlineColor, interactable.CanInteract() ? new Color(255f, 255f, 255f) : new Color(0.88f, 0.56f, 0f));
        // }
        //
        // private void Interact()
        // {
        //     if (_colliderCount == 0) return;
        //     var closest = GetClosestInteractable(_colliders);
        //     var interactable = closest.GetComponent<IInteractable>();
        //     interactable?.OnInteract(gameObject);
        // }
        //
        // private Collider2D GetClosestInteractable(Collider2D[] interactables)
        // {
        //     Collider2D bestTarget = null;
        //     float closestDistanceSqr = Mathf.Infinity;
        //     Vector3 currentPosition = _castPosition.bounds.center;
        //     
        //     foreach(var potentialTarget in interactables)
        //     {
        //         if(potentialTarget == null) continue;
        //         Debug.Log(potentialTarget);
        //         Vector3 directionToTarget = potentialTarget.transform.position - currentPosition;
        //         float dSqrToTarget = directionToTarget.sqrMagnitude;
        //         if(dSqrToTarget < closestDistanceSqr)
        //         {
        //             closestDistanceSqr = dSqrToTarget;
        //             bestTarget = potentialTarget;
        //         }
        //     }             
        //     return bestTarget;
        // }
    }
}

