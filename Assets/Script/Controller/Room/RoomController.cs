using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomController : MonoBehaviour
{
    public enum RoomType { StartRoom, CombatRoom, ExitRoom }

    [Header("Room Settings")]
    [SerializeField] private RoomType roomType = RoomType.CombatRoom;
    [SerializeField] private GameObject doorTilemapGroup; // GameObject chứa Tilemap Cửa (Collider)
    
    [Header("Enemy Spawn Settings")]
    [SerializeField] private List<Transform> spawnPoints;  // Danh sách các điểm Spawn quái
    [SerializeField] private List<GameObject> enemyPrefabs; // Danh sách các Prefab Quái
    [SerializeField] private int totalEnemiesToSpawn = 5;   // Tổng số quái cần sinh ra ở phòng này

    private List<GameObject> activeEnemies = new List<GameObject>();
    private bool isRoomCleared = false;
    private bool isRoomActive = false;

    private void Start()
    {
        // Nếu là phòng Start hoặc Exit, mở sẵn cửa và đánh dấu đã Cleared
        if (roomType == RoomType.StartRoom || roomType == RoomType.ExitRoom)
        {
            isRoomCleared = true;
            SetDoorsState(false); // Mở cửa
        }
        else
        {
            SetDoorsState(false); // Ban đầu cửa mở để Player đi vào
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Khi Player bước vào phòng chiến đấu chưa dọn sạch
        if (other.CompareTag("Player") && !isRoomActive && !isRoomCleared)
        {
            StartCombatRoom();
        }
    }

    private void StartCombatRoom()
    {
        isRoomActive = true;
        SetDoorsState(true); // ĐÓNG CỬA TILEMAP LẠI

        SpawnEnemies();
    }

    private void SpawnEnemies()
    {
        if (enemyPrefabs.Count == 0 || spawnPoints.Count == 0) return;

        for (int i = 0; i < totalEnemiesToSpawn; i++)
        {
            // Lựa chọn ngẫu nhiên vị trí spawn và loại quái
            Transform spawnPoint = spawnPoints[i % spawnPoints.Count];
            GameObject randomEnemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Count)];

            // Sinh quái
            GameObject enemyObj = Instantiate(randomEnemyPrefab, spawnPoint.position, Quaternion.identity);
            activeEnemies.Add(enemyObj);

            // Đăng ký sự kiện khi quái chết (Dùng CharacterStats hoặc BaseCharacter)
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

        // Khi toàn bộ quái trong phòng đã bị tiêu diệt
        if (activeEnemies.Count == 0)
        {
            ClearRoom();
        }
    }

    private void ClearRoom()
    {
        isRoomCleared = true;
        isRoomActive = false;
        SetDoorsState(false); // MỞ CỬA TILEMAP RA

        // Thông báo cho LevelDungeonManager biết đã xong 1 phòng
        LevelDungeonManager.Instance?.OnRoomCleared();
    }

    // Bật/Tắt Tilemap Cửa
    private void SetDoorsState(bool isClosed)
    {
        if (doorTilemapGroup != null)
        {
            doorTilemapGroup.SetActive(isClosed);
        }
    }

    private void OnDrawGizmos()
    {
        // Vẽ các điểm Spawn trong cửa sổ Scene để dễ căn chỉnh
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