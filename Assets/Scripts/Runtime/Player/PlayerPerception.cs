using System.Collections;
using UnityEngine;

namespace Runtime.Player
{
    public class PlayerPerception : MonoBehaviour
    {
        [SerializeField] private float updateRate = 0.5f;
        [SerializeField] private LayerMask blockingLayers;
    
        private GameObject monster;
        private SpriteRenderer _spriteRenderer;
        private Coroutine _perceptionCoroutine;
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
                _perceptionCoroutine = StartCoroutine(PerceptionCheck(other));
            }
        }
    
        private void OnTriggerExit2D(Collider2D other)
        {
            //When monster exits perception trigger, stop coroutine
            if (other.gameObject == monster)
            {
                if(_perceptionCoroutine != null) StopCoroutine(_perceptionCoroutine);
                _spriteRenderer.enabled = false;
            }
        }

        private IEnumerator PerceptionCheck(Component other)
        {
            var dir = other.transform.position - transform.position;
            var hit = Physics2D.Raycast(transform.position, dir, Mathf.Infinity, blockingLayers | monsterLayer);
            if (hit.collider != null)
            {
                Debug.Log("Hit: " + hit.collider.gameObject.name);
                if(hit.collider.gameObject == monster)
                {
                    _spriteRenderer.enabled = true;
                }
            }
        
            yield return new WaitForSeconds(updateRate);
            _perceptionCoroutine = StartCoroutine(PerceptionCheck(other));
        }
    }
}
