/****************************************************************
 * Copyright (c) 2023 AteBit Games
 * All rights reserved.
 ****************************************************************/

using System.Collections;
using Runtime.Managers;
using UnityEngine;
using UnityEngine.UIElements;

namespace Runtime.UI
{
    [DefaultExecutionOrder(6)]
    public class DebugMenu : MonoBehaviour
    {
        private UIDocument _uiDocument;
        private VisualElement _mainContainer;
        
        private VisualElement _aiManagerContainer;
        private Label _menaceStateValue;
        private Label _menaceGaugeValue;
        private Label _aggroLevelValue;
        
        private VisualElement _monsterStateContainer;
        private Label _activeStateValue;
        
        private Coroutine _updateStates;
        
        public void Show()
        {
            _mainContainer.style.display = DisplayStyle.Flex;
            _updateStates = StartCoroutine(UpdateStates());
        }
        
        public void Hide()
        {
            _mainContainer.style.display = DisplayStyle.None;

            if (_updateStates != null)
            {
                StopCoroutine(_updateStates);
                _updateStates = null;
            }
        }

        private void Awake()
        {
            _uiDocument = GetComponent<UIDocument>();
            var rootVisualElement = _uiDocument.rootVisualElement;
            
            _mainContainer = rootVisualElement.Q<VisualElement>("debug-container");
            
            SetupAIManager(rootVisualElement);
            SetupMonsterState(rootVisualElement);
        }

        private void SetupMonsterState(VisualElement rootVisualElement)
        {
            _monsterStateContainer = rootVisualElement.Q<VisualElement>("monster-state");
            _activeStateValue = _monsterStateContainer.Q<Label>("active-state-value");
        }

        private void SetupAIManager(VisualElement root)
        {
            _aiManagerContainer = root.Q<VisualElement>("ai-manager");
            _menaceStateValue = _aiManagerContainer.Q<Label>("menace-state-value");
            _menaceGaugeValue = _aiManagerContainer.Q<Label>("menace-gauge-value");
            _aggroLevelValue = _aiManagerContainer.Q<Label>("aggro-level-value");
        }

        private IEnumerator UpdateStates()
        {
            while (true)
            {
                UpdateAIManagerValues();
                UpdateMonsterStateValues();
                
                yield return new WaitForSeconds(0.2f);
            }
        } 

        private void UpdateAIManagerValues()
        {
            _menaceStateValue.text = "Menace State: " + MenaceStateString();
            _menaceGaugeValue.text = "Menace Gauge: " + GameManager.Instance.AIManager.menaceGaugeValue;
            _aggroLevelValue.text = "Aggro Level: " + GameManager.Instance.AIManager.AggroLevel;
        }
        
        private void UpdateMonsterStateValues()
        {
            _activeStateValue.text = "Active State: " + GameManager.Instance.AIManager.monster.currentMonsterState;
        }

        private static string MenaceStateString()
        {
            var state = GameManager.Instance.AIManager.menaceState;
            return state ? "Active" : "Inactive";
        }
    }
}