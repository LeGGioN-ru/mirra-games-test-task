using System;
using ClockApp.Core.Editing;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace ClockApp.Presentation.Editing
{
    public sealed class EditPanelView : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _viewingGroup;
        [SerializeField] private CanvasGroup _editingGroup;
        [SerializeField] private Button _editButton;
        [SerializeField] private Button _resetButton;
        [SerializeField] private CanvasGroup _resetButtonGroup;
        [SerializeField, Range(0f, 1f)] private float _disabledAlpha = 0.35f;
        [SerializeField] private Button _saveButton;
        [SerializeField] private Button _cancelButton;
        [SerializeField] private InputField _timeInput;
        [SerializeField] private Color _validTextColor = new Color32(230, 232, 239, 255);
        [SerializeField] private Color _invalidTextColor = new Color32(255, 107, 107, 255);
        [SerializeField] private float _fadeDuration = 0.2f;

        public event Action EditRequested;

        public event Action ResetRequested;

        public event Action SaveRequested;

        public event Action CancelRequested;

        public event Action<string> TimeTextChanged;

        private void Awake()
        {
            _editButton.onClick.AddListener(OnEditClicked);
            _resetButton.onClick.AddListener(OnResetClicked);
            _saveButton.onClick.AddListener(OnSaveClicked);
            _cancelButton.onClick.AddListener(OnCancelClicked);
            _timeInput.onValueChanged.AddListener(OnTimeTextChanged);
            _timeInput.onSubmit.AddListener(OnTimeSubmitted);
            _timeInput.onValidateInput = ValidateCharacter;
        }

        private void OnDestroy()
        {
            _editButton.onClick.RemoveListener(OnEditClicked);
            _resetButton.onClick.RemoveListener(OnResetClicked);
            _saveButton.onClick.RemoveListener(OnSaveClicked);
            _cancelButton.onClick.RemoveListener(OnCancelClicked);
            _timeInput.onValueChanged.RemoveListener(OnTimeTextChanged);
            _timeInput.onSubmit.RemoveListener(OnTimeSubmitted);
        }

        public void ShowViewing(bool canReset)
        {
            _resetButton.interactable = canReset;
            _resetButtonGroup.alpha = canReset ? 1f : _disabledAlpha;
            _timeInput.DeactivateInputField();
            Fade(_editingGroup, false);
            Fade(_viewingGroup, true);
        }

        public void ShowEditing(string timeText)
        {
            SetTimeText(timeText);
            SetTimeTextValid(true);
            Fade(_viewingGroup, false);
            Fade(_editingGroup, true);
        }

        public void SetTimeText(string timeText)
        {
            _timeInput.SetTextWithoutNotify(timeText);
        }

        public void SetTimeTextValid(bool isValid)
        {
            _timeInput.textComponent.color = isValid ? _validTextColor : _invalidTextColor;
            _saveButton.interactable = isValid;
        }

        private void Fade(CanvasGroup group, bool isVisible)
        {
            group.DOKill();
            group.interactable = isVisible;
            group.blocksRaycasts = isVisible;
            group.DOFade(isVisible ? 1f : 0f, _fadeDuration).SetUpdate(true).SetLink(group.gameObject);
        }

        private void OnEditClicked()
        {
            EditRequested?.Invoke();
        }

        private void OnResetClicked()
        {
            ResetRequested?.Invoke();
        }

        private void OnSaveClicked()
        {
            SaveRequested?.Invoke();
        }

        private void OnCancelClicked()
        {
            CancelRequested?.Invoke();
        }

        private void OnTimeTextChanged(string text)
        {
            TimeTextChanged?.Invoke(text);
        }

        private void OnTimeSubmitted(string text)
        {
            SaveRequested?.Invoke();
        }

        private static char ValidateCharacter(string text, int charIndex, char addedChar)
        {
            return TimeInputParser.IsAllowedCharacter(addedChar) ? addedChar : '\0';
        }
    }
}
