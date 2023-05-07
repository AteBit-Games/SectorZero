/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/

using System;
using System.Linq;
using Runtime.InputSystem;
using UnityEngine;

namespace Runtime.InteractionSystem
{
    public class PlayerInteraction : MonoBehaviour
    {
        #region Header MOVEMENT DETAILS
        [Space(10)]
        [Header("MOVEMENT DETAILS")]
        #endregion
        [SerializeField] private float interactionRadius = 3f;
        [SerializeField] private LayerMask interactionMask;
        [SerializeField] private InputReader inputReader;
        
        private readonly Collider2D[] _colliders = new Collider2D[3];
        private int _colliderCount;
        
        private static readonly int OutlineThickness = Shader.PropertyToID("_OutlineThickness");
        private static readonly int OutlineColor = Shader.PropertyToID("_OutlineColor");

        private void OnEnable()
        {
            inputReader.InteractEvent += Interact;   
        }
        
        private void OnDisable()
        {
            inputReader.InteractEvent -= Interact;
        }

        // private void LateUpdate()
        // {
        //     _colliderCount = Physics2D.OverlapCircleNonAlloc(transform.position, interactionRadius, _colliders, interactionMask);
        //     if(_colliderCount == 0) return;
        //     
        //     foreach (var collider in _colliders)
        //     {
        //         if (collider == null) continue;
        //         var spriteRenderer = collider.gameObject.GetComponent<SpriteRenderer>();
        //         var distance = Vector2.Distance(transform.position, collider.transform.position);
        //         spriteRenderer.material.SetFloat(OutlineThickness, distance <= interactionRadius ? 1f : 0f);
        //     }
        // }

        private void Update()
        {
            _colliderCount = Physics2D.OverlapCircleNonAlloc(transform.position, interactionRadius, _colliders, interactionMask);
            if(_colliderCount == 0) return;
            
            var closest = GetClosestInteractable(_colliders);
            if(closest == null) return;
            
            var interactable = closest.GetComponent<IInteractable>();
            var spriteRenderer = closest.gameObject.GetComponent<SpriteRenderer>();
            spriteRenderer.material.SetColor(OutlineColor, interactable.CanInteract() ? new Color(255f, 255f, 255f) : new Color(0.88f, 0.56f, 0f));
        }

        private void Interact()
        {
            if (_colliderCount == 0) return;
            var closest = GetClosestInteractable(_colliders);
            var interactable = closest.GetComponent<IInteractable>();
            interactable?.OnInteract(gameObject);
        }

        private GameObject GetClosestInteractable(Collider2D[] items)
        {
            var closest = items[0];
            var closestDistance = Vector2.Distance(transform.position, closest.transform.position);
            foreach (var item in items)
            {
                if (item == null) continue;
                var distance = Vector2.Distance(transform.position, item.transform.position);
                if (distance < closestDistance)
                {
                    closest = item;
                    closestDistance = distance;
                }
            }

            return closest.gameObject;
        }
    }
}

