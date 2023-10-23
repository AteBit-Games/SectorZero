/****************************************************************
* Copyright (c) 2023 AteBit Games
* All rights reserved.
****************************************************************/
using System;
using Runtime.Managers;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Runtime.InputSystem
{
    [CreateAssetMenu(fileName = "NewInputReader", menuName = "InputReader")]
    public class InputReader : ScriptableObject, PlayerInput.IGameplayActions, PlayerInput.IUIActions
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
            
            _playerInput.Enable();
        }

        public event Action PauseEvent;
        public event Action OpenInventoryEvent;
        public event Action CloseUIEvent;

        public event Action<Vector2> MoveEvent;
        public event Action LeftClickEvent;
        public event Action SkipEvent;
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

        public void OnSecondaryClick(InputAction.CallbackContext context)
        {
            
        }

        public void OnMiddleClick(InputAction.CallbackContext context)
        {
            
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
                GameManager.Instance.SoundSystem.Play(GameManager.Instance.ClickSound());
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
            }
        }

        public void OnAim(InputAction.CallbackContext context)
        {
            if(context.phase == InputActionPhase.Performed)
            {

            }
            else if(context.phase == InputActionPhase.Canceled)
            {

            }
        }

        public void OnNavigate(InputAction.CallbackContext context)
        {
            
        }

        public void OnExit(InputAction.CallbackContext context)
        {
            if(context.phase == InputActionPhase.Performed)
            {
                switch (context.control.name)
                {
                    case "tab":
                        if(GameManager.Instance.InventorySystem != null) 
                            if(GameManager.Instance.InventorySystem.isInventoryOpen) OpenInventoryEvent?.Invoke();
                        break;
                    case "escape":
                        if(context.phase == InputActionPhase.Performed) CloseUIEvent?.Invoke();
                        break;
                    case "e":
                        if(GameManager.Instance.HUD != null) 
                            if(GameManager.Instance.HUD.isMapOpen) CloseUIEvent?.Invoke();
                        break;
                }
            }
        }

        public void OnPoint(InputAction.CallbackContext context)
        {
            
        }

        public void OnSelect(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Performed)
            {
                SkipEvent?.Invoke();
            }
        }

        public void OnScrollWheel(InputAction.CallbackContext context)
        {
            
        }

        public void OnPrimaryClick(InputAction.CallbackContext context)
        {
            
        }

        public void SetGameplay()
        {
            _playerInput.Gameplay.Enable();
            _playerInput.UI.Disable();
        }
        
        public void SetUI()
        {
            _playerInput.UI.Enable();
            _playerInput.Gameplay.Disable();
        }
    }
}
