using UnityEngine;

namespace Runtime.AI.Interfaces
{
    public interface ISightHandler
    {
        void OnSightEnter();
        void OnSightExit();
    }
}