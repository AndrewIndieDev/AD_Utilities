using AndrewDowsett.Utility;
using UnityEngine;

namespace AndrewDowsett.MovementSystems
{
    [CreateAssetMenu(fileName = "New Extra Jump Mod", menuName = "2.5D Controller/Extra Jump Mod")]
    public class ExtraJumpMod : MotorMod
    {
        public int MaxExtraJumps => maxExtraJumps;
        [SerializeField] private int maxExtraJumps = 1;

        private int extraJumps;

        public override void OnStart(MovementController2D motor)
        {
            base.OnStart(motor);
            extraJumps = maxExtraJumps;
        }

        public override void OnUpdate(float deltaTime)
        {
            base.OnUpdate(deltaTime);

            if (Motor.IsGrounded)
                extraJumps = maxExtraJumps;

            if (!Motor.IsAbleToLand)
                return;
            if (extraJumps <= 0)
                return;

            if (Input.GetButtonDown("Jump"))
            {
                Motor.Jump();
                extraJumps--;
                TextPopup.Create(Motor.transform.position, modName + (extraJumps > 0 ? $"<{extraJumps}>" : ""), Color.green);
            }
        }
    }
}