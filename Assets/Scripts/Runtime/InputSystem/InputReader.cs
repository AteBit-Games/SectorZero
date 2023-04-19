/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/
using System;
using Runtime.Managers;
using Runtime.SoundSystem;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Runtime.InputSystem
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
        public event Action OpenInventoryEvent;
        public event Action CloseInventoryEvent;

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
                GameManager.Instance.SoundSystem.PauseAll();
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
            if(context.phase == InputActionPhase.Performed)
            {
                OpenInventoryEvent?.Invoke();
                SetUI();
            }
        }

        public void OnNavigate(InputAction.CallbackContext context)
        {
            
        }

        public void OnExit(InputAction.CallbackContext context)
        {
            if(context.phase == InputActionPhase.Performed)
            {
                ResumeEvent?.Invoke();
                CloseInventoryEvent?.Invoke();
                SetGameplay();
                GameManager.Instance.SoundSystem.ResumeAll();
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
        
        public void SetGameplay()
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
