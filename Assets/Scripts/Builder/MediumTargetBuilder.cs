using UnityEngine;
using CarnivalShooter2D.Targets;
using System.Drawing;
using Color = UnityEngine.Color;

namespace CarnivalShooter2D.Builders
{
    public class MediumTargetBuilder : TargetBuilder
    {
        public MediumTargetBuilder()
        {
            speed = 3.5f;   // medium
            pts = 20;     // moderate score
            scl = Vector3.one * 1.1f; // medium size
            color = Color.green;
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
