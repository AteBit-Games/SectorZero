/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/

using Runtime.Managers;
using Runtime.SoundSystem.ScriptableObjects;
using UnityEngine;

namespace Runtime.SoundSystem
{
    public class AudioTrigger : MonoBehaviour
    {
        [SerializeField] private Sound sound;
        [SerializeField] private Transform origin;

        private const string TriggerTag = "Player";

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag(TriggerTag))
            {
                var soundOrigin = origin == null ? transform : origin;
                GameManager.Instance.SoundSystem.Play(sound, soundOrigin);
                Destroy(gameObject);
            }
        }
    }
}
