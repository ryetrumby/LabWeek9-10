using UnityEngine;
using CarnivalShooter2D.Gameplay;

namespace CarnivalShooter2D.Pooling
{
    public class BulletPool2D : ObjectPool<Bullet2D>
    {
        public static BulletPool2D Instance { get; private set; }
        protected override void Awake()
        {
            base.Awake();
            Instance = this;
        }

    }
}
