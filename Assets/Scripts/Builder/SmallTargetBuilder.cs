using UnityEngine;
using CarnivalShooter2D.Targets;
using System.Drawing;
using Color = UnityEngine.Color;

namespace CarnivalShooter2D.Builders
{
    public class SmallTargetBuilder : TargetBuilder
    {
        public SmallTargetBuilder()
        {
            speed = 5f;     // fast
            pts = 40;     // high score
            scl = Vector3.one * 0.8f; // small
            color = Color.blue;
        }

        public override Target2D Build(Transform parent)
        {
            var go = Object.Instantiate(prefab, spawnPos, Quaternion.identity, parent);
            go.SetActive(false);                          // ensure OnEnable hasn't run yet

            var t = go.GetComponent<Target2D>();
            t.moveSpeed = speed;
            t.points = pts;
            t.moveDir = dir;
            t.scale = scl;
            t.color = color;

            go.SetActive(true);                           // now OnEnable applies sr.color = color
            return t;
        }

    }
}
