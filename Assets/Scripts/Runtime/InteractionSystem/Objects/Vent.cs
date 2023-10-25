/****************************************************************
 * Copyright (c) 2023 AteBit Games
 * All rights reserved.
 ****************************************************************/

using System.Collections;
using System.Collections.Generic;
using Runtime.InteractionSystem.Interfaces;
using Runtime.Managers;
using Runtime.Player;
using Runtime.SoundSystem;
using Tweens;
using UnityEngine;

namespace Runtime.InteractionSystem.Objects
{
    public class Vent : MonoBehaviour, IInteractable
    {
        [SerializeField] private float moveTime = 2f;
        
        [SerializeField] private Sound closeSound;
        [SerializeField] private Sound openSound;
        public Sound InteractSound => openSound;
        
        [SerializeField, Tooltip("The location to move the player when the exit the locker")] private Transform revealPosition;
        [SerializeField] private ExitDirection exitDirection;
        [SerializeField] private List<Transform> movePoints;
        
        private Coroutine _movePlayerRoutine;
        [HideInInspector] public float progress;
        [HideInInspector] public bool hasPlayer;
        
        private AudioSource _audioSource;
        
        //========================= Interface events =========================//

        private void Awake()
        {
            _audioSource = transform.GetComponent<AudioSource>();
        }

        public bool OnInteract(GameObject player)
        {
            hasPlayer = true;
            var playerController = player.GetComponentInParent<PlayerController>();
            GameManager.Instance.SoundSystem.PlayOneShot(openSound, _audioSource);
            _audioSource.Play();

            var interaction = player.GetComponent<PlayerInteraction>();
            interaction.RemoveInteractable(gameObject);
            interaction.enabled = false;
            progress = 0;
            
            _movePlayerRoutine = StartCoroutine(MovePlayer(playerController, interaction));
            playerController.DisableInput();
            playerController.SetVisible(false);
            
            return true;
        }

        public void OnInteractFailed(GameObject player)
        {
            
        }
        
        public bool CanInteract()
        {
            return true;
        }
        
        //========================= Coroutines =========================//

        private IEnumerator MovePlayer(PlayerController playerController, PlayerInteraction playerInteraction)
        {
            //move the player to the first point
            playerController.transform.position = movePoints[0].position;
            var tempPoints = new List<Transform>(movePoints);
            tempPoints.RemoveAt(0);
            
            //tween between the points in the movePoints list, calculate the time between each point based on the moveTime
            var timeBetweenPoints = moveTime / (movePoints.Count-1);
            
            //loop through the points and move the player to each one
            foreach (var point in tempPoints)
            {
                var tween = new PositionTween
                {
                    easeType = EaseType.SineInOut,
                    to = point.position,
                    duration = timeBetweenPoints,
                    from = playerController.transform.position,
                    onUpdate = (_, value) =>
                    {
                        progress += (Time.deltaTime / timeBetweenPoints)/moveTime;
                        playerController.transform.position = value;
                    }
                };
                
                playerController.gameObject.AddTween(tween);
                yield return new WaitForSeconds(timeBetweenPoints+0.1f);
            }
            
            //move the player to the reveal position
            playerController.gameObject.CancelTweens();
            ShowPlayer(playerController, playerInteraction);
        }

        private void ShowPlayer(PlayerController playerController, PlayerInteraction playerInteraction)
        {
            var facingDirection = exitDirection switch
            {
                ExitDirection.Left => Vector2.left,
                ExitDirection.Right => Vector2.right,
                ExitDirection.Up => Vector2.up,
                ExitDirection.Down => Vector2.down,
                _ => Vector2.zero
            };
            
            playerController.transform.position = revealPosition.position;
            if (facingDirection != Vector2.zero) playerController.SetFacingDirection(facingDirection);

            playerInteraction.enabled = true;
            playerController.EnableInput();
            playerController.SetVisible(true);
            
            hasPlayer = false;
            _audioSource.Stop();
            GameManager.Instance.SoundSystem.PlayOneShot(closeSound, _audioSource);
        }
        
        public void CancelMovePlayer()
        {
            if (_movePlayerRoutine != null)
            {
                StopCoroutine(_movePlayerRoutine);
                _movePlayerRoutine = null;
            }
        }
    }
}
