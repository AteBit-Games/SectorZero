using System;
using Runtime.Input;
using UnityEngine;

namespace Runtime.InteractionSystem
{
    public class PlayerInteraction : MonoBehaviour
    {
        [SerializeField] private float interactionRadius = 3f;
        [SerializeField] private LayerMask interactionMask;
        [SerializeField] private InputReader inputReader;
        
        private readonly Collider2D[] _colliders = new Collider2D[3];
        private int _colliderCount;
        
        private void Start()
        {
            inputReader.InteractEvent += Interact;   
        }
        
        private void Update()
        {
            _colliderCount = Physics2D.OverlapCircleNonAlloc(transform.position, interactionRadius, _colliders, interactionMask);
            foreach (var item in _colliders)
            {
                if(item == null) continue;
                var distance = Vector2.Distance(transform.position, item.transform.position);
                item.gameObject.GetComponent<SpriteRenderer>().material.SetFloat("_OutlineThickness", distance <= interactionRadius ? 0.75f : 0f);
            }
        }

        private void Interact()
        {
            if (_colliderCount == 0) return;
            var interactable = _colliders[0].GetComponent<IInteractable>();
            interactable?.OnInteract(gameObject);
        }
    }
}

