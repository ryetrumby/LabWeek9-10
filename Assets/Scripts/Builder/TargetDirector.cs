using UnityEngine;
using CarnivalShooter2D.Targets;

namespace CarnivalShooter2D.Builders
{
    public class TargetDirector2D
    {
        public Target2D Construct(TargetBuilder builder, GameObject prefab, Vector3 pos, Vector2 dir, Transform parent)
        {
            return builder
                .WithPrefab(prefab)
                .At(pos)
                .Direction(dir)
                .Build(parent);
        }
    }
}
