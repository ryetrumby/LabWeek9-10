using UnityEngine;
using CarnivalShooter2D.Pooling;
using CarnivalShooter2D.Targets;

namespace CarnivalShooter2D.Gameplay
{
    [RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
    public class Bullet2D : MonoBehaviour
    {
        [SerializeField] float speed = 20f;
        [SerializeField] float lifeSeconds = 1.8f;

        Rigidbody2D rb;
        float despawnAt;

        public void SetSpeed(float newSpeed) { speed = newSpeed; }
        void Awake() => rb = GetComponent<Rigidbody2D>();

        void OnEnable()
        {
            despawnAt = Time.time + lifeSeconds;
            rb.linearVelocity = (Vector2)transform.up * speed; // fire along up axis
        }

        void Update()
        {
            if (Time.time >= despawnAt) Despawn();
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if (other.TryGetComponent<Target2D>(out var target))
            {
                target.OnHit();
                Despawn();
            }
        }

        void Despawn()
        {
            rb.linearVelocity = Vector2.zero;
            BulletPool2D.Instance.ReturnToPool(this);
        }
    }
}
