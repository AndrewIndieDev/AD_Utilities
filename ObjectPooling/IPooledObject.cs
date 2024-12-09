namespace AndrewDowsett.ObjectPooling
{
    public interface IPooledObject
    {
        public void Spawn(ObjectPool pool);
        public void Despawn();
    }
}