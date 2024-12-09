using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AndrewDowsett.Utility
{
    public enum EDialoguePanelType
    {
        OK,
        CancelOK,
        NoYes,
        CancelNoYes,
        CustomOneButton,
        CustomTwoButtons,
        CustomThreeButtons
    }

    public enum EDialogueResult
    {
        OK,
        Cancel,
        No,
        Yes,
        CustomOne,
        CustomTwo,
        CustomThree
    }

    public class PopupManager : MonoBehaviour
    {
        public static PopupManager Instance;

        public TMP_Text dialogueTitleText;
        public TMP_Text dialogueMessageText;
        Action<EDialogueResult, string> dialogueAction;
        public TMP_InputField dialogueInputField;
        public GameObject dialoguePanel;
        public Button leftButton;
        public Button leftMidButton;
        public Button midButton;
        public Button rightMidButton;
        public Button rightButton;

        [Header("Button Texts")]
        public string okString;
        public string cancelString;
        public string noString;
        public string yesString;

        private void Start()
        {
            Instance = this;
        }

        public void ShowDialoguePanel(string title, string message, EDialoguePanelType panelType = EDialoguePanelType.CancelOK, bool inputField = false, Action<EDialogueResult, string> action = null, string customButtonTextOne = null, string customButtonTextTwo = null, string customButtonTextThree = null, object[] titleArgs = null, object[] contentArgs = null)
        {
            dialogueTitleText.text = title;
            dialogueMessageText.text = message;
            dialogueAction = action;
            dialogueInputField.SetTextWithoutNotify("");
            dialogueInputField.gameObject.SetActive(inputField);
            leftButton.gameObject.SetActive(false);
            leftMidButton.gameObject.SetActive(false);
            midButton.gameObject.SetActive(false);
            rightButton.gameObject.SetActive(false);
            rightMidButton.gameObject.SetActive(false);
            switch (panelType)
            {
                case EDialoguePanelType.OK:
                    midButton.DialogueSetup(okString, () => OnResultButtonClicked(EDialogueResult.OK));
                    break;
                case EDialoguePanelType.CancelOK:
                    leftMidButton.DialogueSetup(cancelString, () => OnResultButtonClicked(EDialogueResult.Cancel));
                    rightMidButton.DialogueSetup(okString, () => OnResultButtonClicked(EDialogueResult.OK));
                    break;
                case EDialoguePanelType.NoYes:
                    leftMidButton.DialogueSetup(noString, () => OnResultButtonClicked(EDialogueResult.No));
                    rightMidButton.DialogueSetup(yesString, () => OnResultButtonClicked(EDialogueResult.Yes));
                    break;
                case EDialoguePanelType.CancelNoYes:
                    leftButton.DialogueSetup(cancelString, () => OnResultButtonClicked(EDialogueResult.Cancel));
                    midButton.DialogueSetup(noString, () => OnResultButtonClicked(EDialogueResult.No));
                    rightButton.DialogueSetup(yesString, () => OnResultButtonClicked(EDialogueResult.Yes));
                    break;
                case EDialoguePanelType.CustomOneButton:
                    midButton.DialogueSetup(customButtonTextOne, () => OnResultButtonClicked(EDialogueResult.CustomOne));
                    break;
                case EDialoguePanelType.CustomTwoButtons:
                    leftMidButton.DialogueSetup(customButtonTextOne, () => OnResultButtonClicked(EDialogueResult.CustomOne));
                    rightMidButton.DialogueSetup(customButtonTextTwo, () => OnResultButtonClicked(EDialogueResult.CustomTwo));
                    break;
                case EDialoguePanelType.CustomThreeButtons:
                    leftButton.DialogueSetup(customButtonTextOne, () => OnResultButtonClicked(EDialogueResult.CustomOne));
                    midButton.DialogueSetup(customButtonTextTwo, () => OnResultButtonClicked(EDialogueResult.CustomTwo));
                    rightButton.DialogueSetup(customButtonTextThree, () => OnResultButtonClicked(EDialogueResult.CustomThree));
                    break;
                default:
                    Debug.LogWarning("Enum value not in switch block");
                    break;
            }
            dialoguePanel.SetActive(true);
        }

        public void OnResultButtonClicked(EDialogueResult result)
        {
            dialogueAction?.Invoke(result, dialogueInputField.text);
            dialoguePanel.SetActive(false);
        }
    }
}