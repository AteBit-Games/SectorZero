using System;
using System.Collections;
using UnityEngine;

namespace Runtime.Player
{
    public class PlayerPerception : MonoBehaviour
    {
        [SerializeField] private LayerMask blockingLayers;
    
        private GameObject monster;
        private SpriteRenderer _spriteRenderer;
        private bool _inRange;
        private LayerMask monsterLayer;
    
        private void Awake()
        {
            monster = GameObject.FindWithTag("Monster");
            _spriteRenderer = monster.GetComponent<SpriteRenderer>();
            monsterLayer = LayerMask.GetMask("Monsters");
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject == monster)
            {
                _inRange = true;
            }
        }
    
        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.gameObject == monster)
            {
                _spriteRenderer.enabled = false;
                _inRange = false;
            }
        }

        private void Update()
        {
            if (_inRange)
            {
                var dir = monster.transform.position - transform.position;
                //linecast from player to monster to check if there is a wall in between
                var hit = Physics2D.Linecast(transform.position, monster.transform.position, blockingLayers);
                _spriteRenderer.enabled = hit.collider == null;
            }
        }
    }
}
