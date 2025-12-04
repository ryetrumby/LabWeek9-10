// Turret.cs
using UnityEngine;
using UnityEngine.InputSystem;
using CarnivalShooter2D.Pooling;
using CarnivalShooter2D.Gameplay;

public class Turret : MonoBehaviour
{
    [Header("Shoot")]
    [SerializeField] Transform firePoint;       // where bullets spawn
    [SerializeField] float bulletSpeed = 20f;   // adjustable in Inspector
    [SerializeField] float fireRate = 8f;       // bullets per second
    float nextFireTime;

    void Update()
    {
        AimAtMouse();
        HandleFire();
    }

    void AimAtMouse()
    {
        Vector3 mouseScreen = Mouse.current.position.ReadValue();
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(mouseScreen);
        mouseWorld.z = 0f;

        Vector3 dir = (mouseWorld - transform.position);
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle - 90f);
    }

    void HandleFire()
    {
        // left mouse held or pressed
        bool firePressed = Mouse.current.leftButton.wasPressedThisFrame;

        if (!firePressed) return;
        if (Time.time < nextFireTime) return;

        nextFireTime = Time.time + (1f / fireRate);

        // compute spawn transform
        Vector3 spawnPos = firePoint ? firePoint.position : transform.position;

        // use turret's current up as the bullet forward (2D)
        Quaternion rot = transform.rotation;

        // compute initializer to set position/rotation/speed before activation
        BulletPool2D.Instance.Get(b =>
        {
            b.transform.SetPositionAndRotation(spawnPos, rot);
            b.SetSpeed(bulletSpeed);
        });
    }
}
