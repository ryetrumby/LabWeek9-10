using UnityEngine;

[DisallowMultipleComponent]
public class SaveableTag : MonoBehaviour, ISaveable
{
    // If set in Inspector to "Player", it will be kept. Otherwise we'll assign at runtime.
    [SerializeField] string saveId = "UNSET";
    bool assignedRuntimeId;

    public string SaveId => saveId;
    public Transform TargetTransform => transform;

    void Awake()
    {
        // Keep explicit Player id if designer set it
        if (saveId == "Player") return;

        // If a prefab default leaked in, mark we still need a runtime id
        if (string.IsNullOrEmpty(saveId) || saveId == "UNSET") assignedRuntimeId = false;
        else assignedRuntimeId = true; // already hand-set in Inspector
    }

    void OnEnable()
    {
        // Assign once per instance; keep it stable across pool enable/disable
        if (!assignedRuntimeId && saveId != "Player")
        {
            saveId = GenerateRuntimeId();
            assignedRuntimeId = true;
        }
    }

    string GenerateRuntimeId()
    {
        // Name-based prefix helps debugging, counter ensures uniqueness across instances
        return $"{gameObject.name}_{SaveIdAllocator.NextId()}";
    }

#if UNITY_EDITOR
    void Reset()
    {
        // Editor convenience: set Player if tagged Player, otherwise default
        saveId = gameObject.CompareTag("Player") ? "Player" : "UNSET";
        assignedRuntimeId = (saveId == "Player");
    }
#endif
}
