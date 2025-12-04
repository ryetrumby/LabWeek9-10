using UnityEngine;
using CarnivalShooter2D.Targets;

namespace CarnivalShooter2D.Builders
{
    public abstract class TargetBuilder
    {
        protected GameObject prefab;
        protected Vector3 spawnPos;

        protected float speed = 2f;
        protected int pts = 10;
        protected Vector2 dir = Vector2.right;
        protected Color color = Color.white;
        protected Vector3 scl = Vector3.one;

        public TargetBuilder WithPrefab(GameObject p) { prefab = p; return this; }
        public TargetBuilder At(Vector3 pos) { spawnPos = pos; return this; }
        public TargetBuilder Speed(float s) { speed = s; return this; }
        public TargetBuilder Points(int p) { pts = p; return this; }
        public TargetBuilder Direction(Vector2 d) { dir = d; return this; }
        public TargetBuilder Scale(Vector3 s) { scl = s; return this; }

        public abstract Target2D Build(Transform parent);
    }
}
