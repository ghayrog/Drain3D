using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System;

namespace GUI
{
    [Serializable]
    internal sealed class GUINavigation
    {
        [SerializeField]
        private Button[] _panelActivationButtons;

        [SerializeField]
        private Button[] _panelResetButtons;

        [SerializeField]
        private Transform[] _panels;

        private UnityAction[] _buttonActions;

        internal void OnEnable()
        {
            ResetClick();
            _buttonActions = new UnityAction[_panelActivationButtons.Length];
            for (int i = 0; i < _panelActivationButtons.Length; i++)
            {
                var panelID = i + 1;
                _buttonActions[i] = delegate
                {
                    ButtonClick(panelID);
                };
                _panelActivationButtons[i].onClick.AddListener(_buttonActions[i]);
            }
            for (int i = 0; i < _panelResetButtons.Length; i++)
            {
                _panelResetButtons[i].onClick.AddListener(ResetClick);
            }
        }

        internal void OnDisable()
        {
            for (int i = 0; i < _panelActivationButtons.Length; i++)
            {
                _panelActivationButtons[i].onClick.RemoveListener(_buttonActions[i]);
            }
            for (int i = 0; i < _panelResetButtons.Length; i++)
            {
                _panelResetButtons[i].onClick.RemoveListener(ResetClick);
            }
        }

        private void ButtonClick(int panelNumber)
        {
            Debug.Log($"Show panel number {panelNumber}");
            if (panelNumber >= _panels.Length) return;
            _panels[panelNumber].gameObject.SetActive(true);
            for (int i = 1; i < _panels.Length; i++)
            {
                if (i != panelNumber) _panels[i].gameObject.SetActive(false);
            }
        }

        private void ResetClick()
        {
            _panels[0].gameObject.SetActive(true);
            for (int i = 1; i < _panels.Length; i++)
            {
                _panels[i].gameObject.SetActive(false);
            }
        }
    }
}
