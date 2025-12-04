using UnityEngine;
using CarnivalShooter2D.Builders;
using CarnivalShooter2D.Targets; 

public class TargetSpawner : MonoBehaviour
{
    [SerializeField] float timeBetweenSpawns = 1.0f;
    float timer;

    [SerializeField] GameObject targetBasePrefab;   // replace target
    [SerializeField] Transform container;         

    [SerializeField]
    BuilderKind[] laneBuilders = new BuilderKind[6]
    {
        BuilderKind.Small, BuilderKind.Medium, BuilderKind.Big,
        BuilderKind.Small, BuilderKind.Medium, BuilderKind.Big
    };

    // lane spawn positions (L R on top three rows, R L on top three rows)
    Vector3[] spawnPos;

    // director to manage construction
    TargetDirector2D director = new();

    public enum BuilderKind { Random, Small, Medium, Big }


    void Awake()
    {
        ResetTimer();
        SetSpawns();
    }

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            SpawnWave();
            ResetTimer();
        }
    }

    void ResetTimer() => timer = timeBetweenSpawns;

    void SetSpawns()
    {
        spawnPos = new Vector3[3];
        spawnPos[0] = new Vector3(-10f, 4.00f, 0f);
        spawnPos[1] = new Vector3(-10f, 1.50f, 0f);
        spawnPos[2] = new Vector3(10f, 2.75f, 0f);
    }

    void SpawnWave()
    {
        if (!targetBasePrefab)
        {
            Debug.LogWarning("[TargetSpawner] targetBasePrefab is not assigned.");
            return;
        }

        for (int i = 0; i < spawnPos.Length; i++)
        {
            var pos = spawnPos[i];

            // direction (r -> l or l -> r)
            var dir = (pos.x < 0f) ? Vector2.right : Vector2.left;

            // builder select per lane
            TargetBuilder builder = CreateBuilderForLane(i);

            // build
            director.Construct(builder, targetBasePrefab, pos, dir, container);
        }
    }

    TargetBuilder CreateBuilderForLane(int laneIndex)
    {
        BuilderKind mode = (laneBuilders != null && laneIndex < laneBuilders.Length)
            ? laneBuilders[laneIndex]
            : BuilderKind.Random;

        switch (mode)
        {
            case BuilderKind.Small: return new SmallTargetBuilder();
            case BuilderKind.Big: return new BigTargetBuilder();
            case BuilderKind.Medium: return new MediumTargetBuilder();
            case BuilderKind.Random:
            default:
                int pick = Random.Range(0, 3);
                return pick switch
                {
                    0 => new SmallTargetBuilder(),
                    1 => new MediumTargetBuilder(),
                    _ => new BigTargetBuilder()
                };
        }
    }


}
