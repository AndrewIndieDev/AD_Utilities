using UnityEngine;

namespace AndrewDowsett.ObjectPooling
{
    /// <summary>
    /// To use the IPooledObject interface, a game object must implement it.
    /// This is an example of a game object that can be pooled.
    /// </summary>
    public class PooledGameObjectTest : MonoBehaviour, IPooledObject
    {
        /// <summary>
        /// Called when the game object has been pulled from the ObjectPool.
        /// </summary>
        public void Spawn(ObjectPool pool) { }
        /// <summary>
        /// Called when the game object has been returned to the ObjectPool.
        /// </summary>
        public void Despawn() { }
    }
}