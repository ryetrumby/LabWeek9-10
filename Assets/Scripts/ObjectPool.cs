using System.Collections.Generic;
using UnityEngine;

namespace CarnivalShooter2D.Pooling
{
    // Generic object pool for Components
    public class ObjectPool<T> : MonoBehaviour where T : Component
    {
        [SerializeField] protected T prefab;
        [SerializeField] protected int initialSize = 20;
        [SerializeField] protected bool expandable = true;

        protected readonly Queue<T> pool = new Queue<T>();

        protected virtual void Awake()
        {
            for (int i = 0; i < initialSize; i++)
                pool.Enqueue(CreateInstance());
        }

        protected virtual T CreateInstance()
        {
            var inst = Instantiate(prefab, transform);
            inst.gameObject.SetActive(false);
            return inst;
        }

        public T Get(System.Action<T> initializer)
        {
            if (pool.Count == 0 && expandable)
                pool.Enqueue(CreateInstance());

            var inst = pool.Dequeue();
            // configure before activation
            initializer?.Invoke(inst);
            inst.gameObject.SetActive(true);
            return inst;
        }


        public virtual void ReturnToPool(T instance)
        {
            instance.gameObject.SetActive(false);
            pool.Enqueue(instance);
        }
    }
}
