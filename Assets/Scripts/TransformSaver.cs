using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using CarnivalShooter2D.Observer;

#if ENABLE_INPUT_SYSTEM || UNITY_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class TransformSaver : MonoBehaviour
{
    [Serializable]
    public struct TransformSnapshot
    {
        public string id;
        public float x, y, z;
    }

    [Serializable]
    public class SceneTransforms
    {
        public List<TransformSnapshot> items = new();
    }

    [Serializable]
    public class ScoreData { public int score; }

    string TransformsPath => Path.Combine(Application.persistentDataPath, "transforms.json");
    string ScorePath => Path.Combine(Application.persistentDataPath, "score.dat");

    void Update()
    {
#if ENABLE_INPUT_SYSTEM || UNITY_INPUT_SYSTEM
        var kb = Keyboard.current;
        if (kb == null) return;
        if (kb.sKey.wasPressedThisFrame) SaveAll();
        if (kb.lKey.wasPressedThisFrame) LoadAll();
#else
        if (Input.GetKeyDown(KeyCode.S)) SaveAll();
        if (Input.GetKeyDown(KeyCode.L)) LoadAll();
#endif
    }

    void SaveAll()
    {
        // Active only; exclude pooled/inactive
        var allMono = FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        var allSav = allMono.OfType<ISaveable>().ToList();

        // Always include the Player, plus enemies that are currently visible on screen
        var candidates = new List<ISaveable>();

        foreach (var s in allSav)
        {
            var go = (s as Component)?.gameObject;
            if (!go) continue;

            if (IsPlayer(go, s)) { candidates.Add(s); continue; }
            if (IsVisibleEnemy(go)) { candidates.Add(s); }
        }

        var scene = new SceneTransforms
        {
            items = candidates.Select(s => new TransformSnapshot
            {
                id = s.SaveId,
                x = s.TargetTransform.position.x,
                y = s.TargetTransform.position.y,
                z = s.TargetTransform.position.z
            }).ToList()
        };

        var json = JsonUtility.ToJson(scene, true);
        Directory.CreateDirectory(Path.GetDirectoryName(TransformsPath));
        File.WriteAllText(TransformsPath, json);

        var score = GetCurrentScore();
        SaveScoreBinary(score);

        int enemyCount = candidates.Count(c => !IsLikelyPlayerId(c.SaveId));
        Debug.Log($"[TransformSaver] Saved player + {enemyCount} visible enemies to {TransformsPath}");
        Debug.Log($"[TransformSaver] Saved score={score} to {ScorePath}");
    }

    void LoadAll()
    {
        if (File.Exists(TransformsPath))
        {
            var json = File.ReadAllText(TransformsPath);
            var scene = JsonUtility.FromJson<SceneTransforms>(json) ?? new SceneTransforms();

            // Bucket snapshots by id so duplicates are handled gracefully
            var buckets = scene.items
                .GroupBy(i => i.id)
                .ToDictionary(g => g.Key, g => new Queue<TransformSnapshot>(g));

            var allMono = FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
            var allSav = allMono.OfType<ISaveable>();

            int restored = 0;

            foreach (var s in allSav)
            {
                if (!buckets.TryGetValue(s.SaveId, out var queue)) continue;
                if (queue.Count == 0) continue;

                var snap = queue.Dequeue();
                s.TargetTransform.position = new Vector3(snap.x, snap.y, snap.z);
                restored++;
            }

            Debug.Log($"[TransformSaver] Restored {restored} transforms from {TransformsPath}");
        }
        else
        {
            Debug.LogWarning("[TransformSaver] No transforms file found to load.");
        }

        if (File.Exists(ScorePath))
        {
            int score = LoadScoreBinary();
            ApplyScore(score);
            Debug.Log($"[TransformSaver] Restored score={score} from {ScorePath}");
        }
        else
        {
            Debug.LogWarning("[TransformSaver] No score file found to load.");
        }
    }


    bool IsPlayer(GameObject go, ISaveable s)
    {
        if (go.CompareTag("Player")) return true;
        return IsLikelyPlayerId(s.SaveId);
    }

    bool IsLikelyPlayerId(string id)
    {
        if (string.IsNullOrEmpty(id)) return false;
        var x = id.ToLowerInvariant();
        return x == "player" || x.StartsWith("player_");
    }

    bool IsVisibleEnemy(GameObject go)
    {
        // must be tagged Enemy or look like an enemy id/name
        bool looksLikeEnemy = go.CompareTag("Enemy") || go.name.ToLowerInvariant().Contains("enemy");

        if (!looksLikeEnemy) return false;

        var cam = Camera.main;
        if (!cam) return true; // no camera => fall back to saving

        Vector3 vp = cam.WorldToViewportPoint(go.transform.position);
        bool inFront = vp.z > 0f;
        bool onScreen = vp.x >= 0f && vp.x <= 1f && vp.y >= 0f && vp.y <= 1f;

        // also allow renderer visibility when available
        var rend = go.GetComponentInChildren<Renderer>();
        bool visibleByRenderer = rend && rend.isVisible;

        return inFront && (onScreen || visibleByRenderer);
    }

    int GetCurrentScore()
    {
        var sm = ScoreManager.Instance ?? FindFirstObjectByType<ScoreManager>();
        return sm ? sm.score : 0;
    }

    void ApplyScore(int value)
    {
        var sm = ScoreManager.Instance ?? FindFirstObjectByType<ScoreManager>();
        if (!sm) return;

        sm.score = value;
        sm.Notify(value);
    }

    void SaveScoreBinary(int score)
    {
        using (var fs = File.Open(ScorePath, FileMode.Create))
        {
#pragma warning disable SYSLIB0011
            var bf = new BinaryFormatter();
            bf.Serialize(fs, new ScoreData { score = score });
#pragma warning restore SYSLIB0011
        }
    }

    int LoadScoreBinary()
    {
        using (var fs = File.Open(ScorePath, FileMode.Open))
        {
#pragma warning disable SYSLIB0011
            var bf = new BinaryFormatter();
            var data = (ScoreData)bf.Deserialize(fs);
#pragma warning restore SYSLIB0011
            return data.score;
        }
    }
}
