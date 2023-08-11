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
        private const string PlaceHolderClass = "unity-text-input__placeholder";
        private const string ValueClass = "unity-text-input__value";
        private const string PlaceholderText = "Please include the following if applicable: \n1) Description of the issue \n2) Steps to reproduce";

        //================================ Unity Events =================================//
        
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

            descriptionField.value = PlaceholderText;
            descriptionField.RegisterCallback<FocusInEvent>(_ =>
            {
                if (descriptionField.ClassListContains(PlaceHolderClass))
                {
                    descriptionField.RemoveFromClassList(PlaceHolderClass);
                    descriptionField.value = "";
                }
                
                descriptionField.AddToClassList(ValueClass);
            });
            descriptionField.RegisterCallback<FocusOutEvent>(_ =>
            {
                if((string.IsNullOrEmpty(descriptionField.value)))
                {
                    descriptionField.AddToClassList(PlaceHolderClass);
                    descriptionField.RemoveFromClassList(ValueClass);
                    descriptionField.value = PlaceholderText;
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
        
        //================================ Public Methods =================================//
        
        public void ShowForm()
        {
           UIUtils.ShowUIElement(_feedbackWindow);
        }
        
        public void HideForm()
        {
            UIUtils.HideUIElement(_feedbackWindow);
        }

        public void SendForm()
        {
            if(string.IsNullOrEmpty(descriptionField.value) || descriptionField.value == PlaceholderText || descriptionField.value.Length < 10)
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
                resultDelegate: (success, _) =>
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
        
        //================================ Private Methods =================================//

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
            descriptionField.value = PlaceholderText;
            if(!descriptionField.ClassListContains(PlaceHolderClass)) descriptionField.AddToClassList(PlaceHolderClass);
            if(descriptionField.ClassListContains(ValueClass)) descriptionField.RemoveFromClassList(ValueClass);
            categoryDropdown.index = 0;
            HideForm();
        }
    }
}
