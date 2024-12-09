using Cysharp.Threading.Tasks;
using UnityEngine;

namespace AndrewDowsett.SingleEntryPoint
{
    public class IntroAnimation : MonoBehaviour
    {
        public async UniTask Play()
        {
            // await an animation, or Feel Feedbacks.
            await UniTask.Delay(3000);
        }
    }
}