using System;
using UnityEngine;

namespace Runtime.InteractionSystem
{
    public interface IInteractable
    {
        public AudioClip InteractSound { get; }

        public bool OnInteract(GameObject player);
    }
}
