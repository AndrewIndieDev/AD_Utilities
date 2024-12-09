using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace AndrewDowsett.SingleEntryPoint
{
    public enum EProgressBarType
    {
        Bar_Left_To_Right,
        Bar_Right_To_Left,
        Bar_Top_To_Bottom,
        Bar_Bottom_To_Top,
        Radial_Default_Clockwise,
        Radial_Default_AntiClockwise,
        Radial_Reverse_Clockwise,
        Radial_Reverse_AntiClockwise
    }

    public class ProgressBar : MonoBehaviour
    {
        public TMP_Text text;
        public Image progress;
    }
}