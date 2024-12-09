using UnityEngine;

namespace AndrewDowsett.MovementSystems
{
    public class MotorMod : ScriptableObject
    {
        public MovementController2D Motor { get; protected set; }

        /// <summary>
        /// Name of the mod. You can use this however you want.
        /// </summary>
        public string ModName => modName;
        [SerializeField] protected string modName;

        /// <summary>
        /// Used if necessary to describe the mod.
        /// </summary>
        public string ModDescription => modDescription;
        [SerializeField][TextArea] protected string modDescription;

        /// <summary>
        /// This is the base.OnStart() for all mods. Only change this if you know what you are doing.
        /// </summary>
        /// <param name="motor">The BaseMotor that this mod should use.</param>
        public virtual void OnStart(MovementController2D motor)
        {
            Motor = motor; // Sets the Motor to the motor that called this mod.
        }

        /// <summary>
        /// This is the base.OnUpdate() for all mods. Only change this if you know what you are doing.
        /// </summary>
        /// <param name="deltaTime">Time since last frame.</param>
        public virtual void OnUpdate(float deltaTime)
        {
            if (Motor == null)
            {
                Debug.LogWarning($"{modName} :: Has no Motor assigned - Finding motor. . .");

                Motor = FindFirstObjectByType<MovementController2D>();

                if (Motor != null)
                {
                    Debug.LogWarning($"{modName} :: Found motor - Remember to call OnStart when swapping the mods through code. . .");
                }
                else
                {
                    Debug.Log($"{modName} :: No Motor found - Please make sure your character has a BaseMotor script attached to it. . .");
                }
            }
        }
    }
}