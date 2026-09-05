using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomController : MonoBehaviour
{
    public enum RoomType { StartRoom, CombatRoom, ExitRoom }

    [Header("Room Settings")]
    [SerializeField] private RoomType roomType = RoomType.CombatRoom;
    [SerializeField] private GameObject doorTilemapGroup;

    [Header("Enemy Spawn Settings")]
    [SerializeField] private List<Transform> spawnPoints;
    [SerializeField] private List<GameObject> enemyPrefabs;
    [SerializeField] private int totalEnemiesToSpawn = 5;

    [Header("Room Status")]
    [SerializeField] private bool isRoomCleared = true;
    private bool isRoomActive = false;
    private bool hasBeenVisited = false; // ĐÁNH DẤU: Phòng này đã từng được kích hoạt chiến đấu chưa?

    private List<GameObject> activeEnemies = new List<GameObject>();

    // Properties cho TeleportPortal đọc
    public bool IsCleared => isRoomCleared;
    public bool HasBeenVisited => hasBeenVisited; // Thêm property này
    public RoomType CurrentRoomType => roomType;

    private void Start()
    {
        SetDoorsState(false);
        if (roomType == RoomType.StartRoom || roomType == RoomType.ExitRoom)
        {
            isRoomCleared = true;
            hasBeenVisited = true; // Phòng Start/Exit không tính là phòng combat
        }
    }

    private void Update()
    {
        if (isRoomActive && !isRoomCleared)
        {
            CheckActiveEnemies();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Khi người chơi bước vào phòng combat LẦN ĐẦU TIÊN
        if (other.CompareTag("Player") && roomType == RoomType.CombatRoom && !hasBeenVisited)
        {
            StartCombatRoom();
        }
    }

    private void StartCombatRoom()
    {
        isRoomActive = true;
        isRoomCleared = false;
        hasBeenVisited = true; // Xác nhận người chơi ĐÃ VÀO VÀ ĐANG ĐÁNH ở phòng này

        SetDoorsState(true);
        SpawnEnemies();
    }

    private void SpawnEnemies()
    {
        if (enemyPrefabs.Count == 0 || spawnPoints.Count == 0)
        {
            ClearRoom();
            return;
        }

        activeEnemies.Clear();

        for (int i = 0; i < totalEnemiesToSpawn; i++)
        {
            Transform spawnPoint = spawnPoints[i % spawnPoints.Count];
            GameObject randomEnemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Count)];

            GameObject enemyObj = Instantiate(randomEnemyPrefab, spawnPoint.position, Quaternion.identity);
            enemyObj.transform.parent = transform;
            
            activeEnemies.Add(enemyObj);

            if (enemyObj.TryGetComponent<CharacterStats>(out var stats))
            {
                stats.OnDeath += () => OnEnemyKilled(enemyObj);
            }
        }
    }

    private void OnEnemyKilled(GameObject enemy)
    {
        if (activeEnemies.Contains(enemy))
        {
            activeEnemies.Remove(enemy);
        }

        if (activeEnemies.Count == 0)
        {
            ClearRoom();
        }
    }

    private void CheckActiveEnemies()
    {
        activeEnemies.RemoveAll(enemy => enemy == null);

        if (activeEnemies.Count == 0)
        {
            ClearRoom();
        }
    }

    private void ClearRoom()
    {
        isRoomCleared = true;
        isRoomActive = false;
        SetDoorsState(false);

        LevelDungeonManager.Instance?.OnRoomCleared();
    }
    public void ResetRoomIfUncleared()
    {
        // Nếu phòng này người chơi chưa kịp đánh xong (chết giữa chừng)
        if (isRoomActive && !isRoomCleared)
        {
            isRoomActive = false;
            hasBeenVisited = false; // Reset lại để Player có thể vào đánh lại phòng này
            SetDoorsState(false);   // Mở lại cửa

            // Xóa hết quái dở dang còn sót lại trong phòng
            foreach (var enemy in activeEnemies)
            {
                if (enemy != null) Destroy(enemy);
            }
            activeEnemies.Clear();
        }
    }

    private void SetDoorsState(bool isClosed)
    {
        if (doorTilemapGroup != null)
        {
            doorTilemapGroup.SetActive(isClosed);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (spawnPoints != null)
        {
            foreach (var sp in spawnPoints)
            {
                if (sp != null) Gizmos.DrawWireSphere(sp.position, 0.5f);
            }
        }
    }
}