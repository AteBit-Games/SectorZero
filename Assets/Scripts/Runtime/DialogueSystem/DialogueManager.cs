using Runtime.Input;
using TMPro;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.DialogueSystem
{
    public class DialogueManager : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI textElement;
        [SerializeField] private TextMeshProUGUI actorName;
        [SerializeField] private Image actorImage;
        [SerializeField] private InputReader inputReader;
        
        [Range(0, 1)] 
        [SerializeField] private float visibleText;
        [SerializeField] private float textSpeed = 0.05f;
        private float _totalTimeToType, _currentTime;
        private string _currentText;
        
        private Dialogue _currentDialogue;
        private int _currentLineIndex;

        private void Start()
        {
            inputReader.LeftClickEvent += PushText;
        }

        private void PushText()
        {
            _currentLineIndex++;
            if(_currentLineIndex >= _currentDialogue.lines.Count)
            {
                Conclude();
            }
            else
            {
                _currentText = _currentDialogue.lines[_currentLineIndex];
                _totalTimeToType = _currentText.Length * textSpeed;
                _currentTime = 0;
            }
        }
        
        public void StartDialogue(Dialogue dialogue)
        {
            Show(true);
            _currentDialogue = dialogue;
            _currentLineIndex = 0;
            textElement.text = dialogue.lines[_currentLineIndex];
            actorName.text = dialogue.actor.Name;
            actorImage.sprite = dialogue.actor.Sprite;
        }

        private void Conclude()
        {
            Debug.Log("Conclude");
            Show(false);
        }
        
        private void Show(bool show)
        {
            gameObject.SetActive(show);
        }
    }
}