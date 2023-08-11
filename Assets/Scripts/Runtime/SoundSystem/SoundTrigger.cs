/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/

using Runtime.Managers;
using Runtime.SoundSystem.ScriptableObjects;
using UnityEngine;

namespace Runtime.SoundSystem
{
    /// <summary>
    /// Used to play a sound when the player enters a trigger
    /// </summary>
    public class SoundTrigger : MonoBehaviour
    {
        [SerializeField] private Sound sound;
        private const string TriggerTag = "Player";

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag(TriggerTag))
            {
                GameManager.Instance.SoundSystem.Play(sound, transform.GetComponent<AudioSource>());
                Destroy(gameObject);
            }
        }
    }
}
