using UnityEngine;

public class BallSpawner : MonoBehaviour
{
    [Header("References")]
    public GameObject ballPrefab;
    public Transform player;

    [Header("Spawn Settings")]
    public float spawnOffsetX = 5f;      // ระยะห่างซ้าย/ขวาจาก Player
    public float minSpawnY = 2f;      // ความสูงต่ำสุด
    public float maxSpawnY = 6f;      // ความสูงสูงสุด
    public float spawnInterval = 2f;     // เวลาระหว่าง Spawn

    [Header("Spawn Chance")]
    [Range(0f, 1f)]
    public float spawnChance = 0.5f;   // 0 = ไม่เกิดเลย, 1 = เกิดทุกรอบ

    void Start()
    {
        InvokeRepeating(nameof(SpawnBall), 1f, spawnInterval);
    }

    void SpawnBall()
    {
        if (Random.value > spawnChance)
        {
            Debug.Log("NotSpawn >:3");
            return;
        }
        // สุ่มซ้าย (-1) หรือขวา (+1)
        Debug.Log("Spawn");
        int direction = Random.value > 0.5f ? 1 : -1;
        float randomY = Random.Range(minSpawnY, maxSpawnY);


        Vector3 spawnPos = new Vector3(
            player.position.x + (spawnOffsetX * direction),
            player.position.y + randomY,
            0f
        );

        GameObject ball = Instantiate(ballPrefab, spawnPos, Quaternion.identity);

        // ส่งตำแหน่ง Player ไปให้ลูกบอล
        ball.GetComponent<Ball>().Init(player.position);
    }
}
