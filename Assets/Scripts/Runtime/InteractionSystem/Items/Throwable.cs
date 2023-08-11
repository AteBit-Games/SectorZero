/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/

using System.Collections;
using Runtime.AI;
using Runtime.InteractionSystem.Interfaces;
using Runtime.InventorySystem;
using Runtime.Managers;
using Runtime.SaveSystem;
using Runtime.SaveSystem.Data;
using Runtime.SoundSystem.ScriptableObjects;
using UnityEngine;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

namespace Runtime.InteractionSystem.Items
{
    public class Throwable : MonoBehaviour, IInteractable, IPersistant, IThrowable
    {
        [SerializeField] private Sound interactSound;
        public Sound InteractSound => interactSound;
        public Sound DropSound => dropSound;
        [SerializeField] private Sound dropSound;
        
        [SerializeField] private Sprite icon;
        [SerializeField] private string persistentID;

        [Range(1, 5), SerializeField] private int maxBounces;
        [SerializeField] private float bounceDistanceDivider = 8f;
        [SerializeField] private float bounceHeightDivider = 6f;
        [SerializeField] private bool randomizeLand = true;
        
        private Coroutine _throwCoroutine;
        private SortingGroup _sortingGroup;

        //========================= Unity Events =========================//
        
        private void Awake()
        {
            _sortingGroup = GetComponent<SortingGroup>();
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if(other.gameObject.layer == LayerMask.NameToLayer("Walls"))
            {
                if (_throwCoroutine != null) StopCoroutine(_throwCoroutine);
                _sortingGroup.sortingOrder = 9;
                GameManager.Instance.SoundSystem.Play(dropSound, transform.GetComponent<AudioSource>());
            }
        }
        
        //========================= Interface events =========================//
        
        public bool OnInteract(GameObject player)
        {
            gameObject.SetActive(false);

            var gameManager = GameManager.Instance;
            gameManager.SoundSystem.Play(interactSound, transform.GetComponent<AudioSource>());
            gameManager.HUD.SetThrowableIcon(icon);
            gameManager.HUD.ShowThrowableIcon(true);

            var inventory = player.GetComponentInParent<PlayerInventory>();
            inventory.PickUpThrowable(gameObject);

            return true;
        }

        public void OnInteractFailed(GameObject player)
        {
            throw new System.NotImplementedException();
        }
        
        public bool CanInteract()
        {
            return true;
        }
        
        //========================= Save System =========================//

        public void LoadData(SaveData data)
        {
            //data.tapeRecorders.Add(this, gameObject.activeSelf);
        }

        public void SaveData(SaveData data)
        {
            //data.tapeRecorders[this] = gameObject.activeSelf;
        }

        public void OnDrop(Transform dropPosition)
        {
            GameManager.Instance.SoundSystem.Play(DropSound, transform.GetComponent<AudioSource>());
            GetComponent<NoiseEmitter>().EmitLocal();
        }
        
        public void Throw(Vector2 throwOrigin, GameObject throwIndicator)
        {
            //pick a random point in the circle
            var landingRange = throwIndicator.GetComponent<CircleCollider2D>();

            _sortingGroup.sortingOrder = 11;
            Vector3 throwPosition;

            if (randomizeLand)
            {
                var randomPointInRange = Random.insideUnitCircle * landingRange.radius;
                throwPosition = new Vector3(randomPointInRange.x, randomPointInRange.y, 0) + throwIndicator.transform.position;
            }
            else
            {
                throwPosition = throwIndicator.transform.position;
            }

            //Determine the distance of the throw
            var throwDistance = Vector2.Distance(throwPosition, gameObject.transform.position);
            var height = Mathf.Clamp(throwDistance/1.5f, 2f, 8f);
            
            //Enable the throwable and set its position to the origin of the player
            gameObject.SetActive(true);
            gameObject.transform.position = throwOrigin;
            
            //Calculate the bezier curve
            var point = new Vector2[3];
            point[0] = gameObject.transform.position;
            point[2] = throwPosition;
            point[1] = point[0] +(point[2] -point[0])/2 + Vector2.up * height;
            
            //Based on higher distance have less bounces
            var bounces = Mathf.RoundToInt(maxBounces+1 - Mathf.Clamp(Mathf.RoundToInt(throwDistance/4), 1, maxBounces));

            //Fall speed modifier
            var modifier = -Mathf.Clamp(throwDistance / 120, 0.065f, 0.07f);
            modifier += 0.1f;
            
            //Start the coroutine to move the throwable on the curve
            _throwCoroutine = StartCoroutine(ThrowCoroutine(0.0f, point, modifier, bounces, throwDistance, throwOrigin));
        }

        private IEnumerator ThrowCoroutine(float bezierCount, Vector2[] points, float countModifier, int bounces, float distance, Vector2 origin)
        {
            if(bezierCount <= 1.0f)
            {
                var bezierPoint = Mathf.Pow(1.0f - bezierCount, 2) * points[0] + 2.0f * (1.0f - bezierCount) * bezierCount * points[1] + Mathf.Pow(bezierCount, 2) * points[2];
                transform.position = bezierPoint;
                
                if(bezierCount <= 0.5) countModifier = Mathf.Clamp(countModifier - 0.0006f, 0, 1);
                else countModifier = Mathf.Clamp(countModifier + 0.0003f, 0, 1);
                
                yield return new WaitForSeconds(0.02f);
                _throwCoroutine = StartCoroutine(ThrowCoroutine(bezierCount + countModifier,  points, countModifier, bounces, distance, origin));
            }
            else
            {
                GameManager.Instance.SoundSystem.Play(DropSound, transform.GetComponent<AudioSource>());

                if (bounces > 0)
                {
                    Debug.Log(bounces);
                    points[0] = transform.position;
                    points[2] += (points[0] - (Vector2)origin).normalized * distance/bounceDistanceDivider;
                    points[1] = points[0] + (points[2] -points[0])/2 + Vector2.up * (distance/bounceHeightDivider * bounces);
                    StartCoroutine(ThrowCoroutine(0.0f, points, countModifier + 0.04f, bounces-1, distance, origin));
                }
                else
                {
                    _sortingGroup.sortingOrder = 9;
                    StopCoroutine(_throwCoroutine);
                    _throwCoroutine = null;
                }
            }
        }
    }
}
