/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/
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
        
        private Collider2D[] _colliders = new Collider2D[3];
        private int _colliderCount;
        private static readonly int OutlineThickness = Shader.PropertyToID("_OutlineThickness");

        private void OnEnable()
        {
            inputReader.InteractEvent += Interact;   
        }
        
        private void OnDisable()
        {
            inputReader.InteractEvent -= Interact;
        }
        
        private void Update()
        {
            _colliders = Physics2D.OverlapCircleAll(transform.position, interactionRadius, interactionMask);
            foreach (var item in _colliders)
            {
                if(item == null) continue;
                Debug.Log(item.gameObject.name);
                var distance = Vector2.Distance(transform.position, item.transform.position);
                item.gameObject.GetComponent<SpriteRenderer>().material.SetFloat(OutlineThickness, 1);
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

