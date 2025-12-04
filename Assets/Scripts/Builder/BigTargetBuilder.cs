using UnityEngine;
using CarnivalShooter2D.Targets;
using Color = UnityEngine.Color;

namespace CarnivalShooter2D.Builders
{
    public class BigTargetBuilder : TargetBuilder
    {

        public BigTargetBuilder()
        {
            speed = 1.8f;   // slow
            pts = 10;     // low score
            scl = Vector3.one * 1.5f; // big
            color = Color.red;
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
