using System.Collections.Generic;
using System.Text;
using Runtime.Managers;
using Runtime.Utils;
using UnityEngine;
using UnityEngine.UIElements;

namespace Runtime.ReporterSystem
{
    public class FeedbackForm : MonoBehaviour
    {
        [SerializeField] private Sprite successIcon;
        [SerializeField] private Sprite errorIcon;
        
        private UIDocument _uiDocument;
        public DropdownField categoryDropdown;
        public TextField descriptionField;
        private VisualElement _feedbackWindow;
        private Button _submitButton;
        private Button _closeButton;

        private CodecksCardCreator _cardCreator;
        const string placeHolderClass = "unity-text-input__placeholder";
        const string valueClass = "unity-text-input__value";
        const string placeholderText = "Please include the following if applicable: \n1) Description of the issue \n2) Steps to reproduce";

        private void Start()
        {
            _cardCreator = GetComponent<CodecksCardCreator>();
        }
        
        private void Awake()
        {
            _uiDocument = GetComponent<UIDocument>();
            var rootVisualElement = _uiDocument.rootVisualElement;
            
            _feedbackWindow = rootVisualElement.Q<VisualElement>("feedback-window");
            

            categoryDropdown = rootVisualElement.Q<DropdownField>("category-dropdown");
            categoryDropdown.choices = new List<string>
            {
                "<b>Feedback</b> - Suggestions, ideas, improvements",
                "<b>Minor</b> - Typos, visual glitches, missing sounds or animations",
                "<b>Major</b> - Exploits, unreadable text, game breaking bugs",
                "<b>Critical</b> - Crashes, freezes, corrupted saves"
            };
            categoryDropdown.index = 0;
            
            descriptionField = rootVisualElement.Q<TextField>("description-field");

            descriptionField.value = placeholderText;
            descriptionField.RegisterCallback<FocusInEvent>(_ =>
            {
                if (descriptionField.ClassListContains(placeHolderClass))
                {
                    descriptionField.RemoveFromClassList(placeHolderClass);
                    descriptionField.value = "";
                }
                
                descriptionField.AddToClassList(valueClass);
            });
            descriptionField.RegisterCallback<FocusOutEvent>(_ =>
            {
                if((string.IsNullOrEmpty(descriptionField.value)))
                {
                    descriptionField.AddToClassList(placeHolderClass);
                    descriptionField.RemoveFromClassList(valueClass);
                    descriptionField.value = placeholderText;
                }
            });
            
            _submitButton = rootVisualElement.Q<Button>("submit-button");
            _submitButton.RegisterCallback<ClickEvent>(_ =>
            {
                GameManager.Instance.SoundSystem.Play(GameManager.Instance.ClickSound());
                SendForm();
            });

            _closeButton = rootVisualElement.Q<Button>("cancel-button");
            _closeButton.RegisterCallback<ClickEvent>(_ =>
            {
                GameManager.Instance.SoundSystem.Play(GameManager.Instance.ClickSound());
                HideForm();
            });
        }
        
        public void ShowForm()
        {
           UI_Utils.ShowUIElement(_feedbackWindow);
        }
        
        public void HideForm()
        {
            UI_Utils.HideUIElement(_feedbackWindow);
        }

        public void SendForm()
        {
            if(string.IsNullOrEmpty(descriptionField.value) || descriptionField.value == placeholderText || descriptionField.value.Length < 10)
            {
                GameManager.Instance.NotificationManager.ShowResultNotification("Description Too Short", errorIcon);
                return;
            }
            
            string reportText = $"{descriptionField.value}\n\n";
            reportText += GetMetaText();
            
            _submitButton.SetEnabled(false);

            _cardCreator.CreateNewCard(
                text: reportText,
                severity: (CodecksCardCreator.CodecksSeverity)categoryDropdown.index,
                resultDelegate: (success, result) =>
                {
                    if (success)
                    {
                        Debug.Log("Card created successfully");
                        GameManager.Instance.NotificationManager.ShowResultNotification("Submitted", successIcon);
                        _submitButton.SetEnabled(true);
                        ResetForm();
                    }
                    else
                    {
                        GameManager.Instance.NotificationManager.ShowResultNotification("Failed", errorIcon);
                        _submitButton.SetEnabled(true);
                    }
                }
            );
        }

        private static string GetMetaText()
        {
            StringBuilder metaText = new StringBuilder(); 
            metaText.AppendLine($"```");
            metaText.AppendLine($"Platform: {Application.platform.ToString()}");
            metaText.AppendLine($"App Version: {Application.version}");
            metaText.AppendLine("```");
            return metaText.ToString();
        }
        
        private void ResetForm()
        {
            descriptionField.value = placeholderText;
            if(!descriptionField.ClassListContains(placeHolderClass)) descriptionField.AddToClassList(placeHolderClass);
            if(descriptionField.ClassListContains(valueClass)) descriptionField.RemoveFromClassList(valueClass);
            categoryDropdown.index = 0;
            HideForm();
        }
    }
}
