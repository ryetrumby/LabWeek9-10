using UnityEngine;

public interface ISaveable
{
    string SaveId { get; }
    Transform TargetTransform { get; }
}
