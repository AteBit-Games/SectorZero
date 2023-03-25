using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Runtime.Input
{
    [CreateAssetMenu(fileName = "NewInputReader", menuName = "InputReader")]
    public class InputReader : ScriptableObject, PlayerInput.IUIActions, PlayerInput.IGameplayActions
    {
        private PlayerInput _playerInput;
        
        private void OnEnable()
        {
            if (_playerInput == null)
            {
                _playerInput = new PlayerInput();
                _playerInput.UI.SetCallbacks(this);
                _playerInput.Gameplay.SetCallbacks(this);
                
                SetGameplay();
            }
        }

        public event Action PauseEvent;
        public event Action ResumeEvent;
        
        public event Action<Vector2> MoveEvent;
        public event Action LeftClickEvent;
        public event Action InteractEvent;
        public event Action SneakEvent;

        public void OnMove(InputAction.CallbackContext context)
        {
            MoveEvent?.Invoke(context.ReadValue<Vector2>());
        }

        public void OnLook(InputAction.CallbackContext context)
        {
            
        }

        public void OnLeftClick(InputAction.CallbackContext context)
        {
            if(context.phase == InputActionPhase.Performed)
            {
                LeftClickEvent?.Invoke();
            }
        }

        public void OnSneak(InputAction.CallbackContext context)
        {
            if(context.phase == InputActionPhase.Performed)
            {
                SneakEvent?.Invoke();
            }
        }
        

        public void OnPause(InputAction.CallbackContext context)
        {
            if(context.phase == InputActionPhase.Performed)
            {
                PauseEvent?.Invoke();
                SetUI();
            }
        }

        public void OnInteract(InputAction.CallbackContext context)
        {
            if(context.phase == InputActionPhase.Performed)
            {
                InteractEvent?.Invoke();
            }
        }

        public void OnInventory(InputAction.CallbackContext context)
        {
            throw new NotImplementedException();
        }

        public void OnNavigate(InputAction.CallbackContext context)
        {
            
        }

        public void OnExit(InputAction.CallbackContext context)
        {
            if(context.phase == InputActionPhase.Performed)
            {
                ResumeEvent?.Invoke();
                SetGameplay();
            }
        }

        public void OnPoint(InputAction.CallbackContext context)
        {
            
        }

        public void OnSelect(InputAction.CallbackContext context)
        {
            
        }

        public void OnScrollWheel(InputAction.CallbackContext context)
        {
            
        }
        
        private void SetGameplay()
        {
            _playerInput.Gameplay.Enable();
            _playerInput.UI.Disable();
        }
        
        private void SetUI()
        {
            _playerInput.UI.Enable();
            _playerInput.Gameplay.Disable();
        }
    }
}
