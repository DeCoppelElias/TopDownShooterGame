using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WaveManager : MonoBehaviour
{
    public GameStateManager gameState;
    public GameObject player;

    public GameObject meleeEnemy;
    public GameObject rangedEnemy;
    public GameObject fastEnemy;
    public GameObject shotgunEnemy;
    public GameObject chargeMeleeEnemy;
    public GameObject dodgeMeleeEnemy;
    public GameObject dodgeRangedEnemy;

    public GameObject dashBoss;
    public GameObject splitBoss;
    public GameObject rangedBoss;

    public GameObject enemies;

    public Tilemap groundTilemap;
    public Tilemap wallTilemap;
    public Tilemap warningTilemap;
    public Tile warningTile;

    public enum WaveState {Fighting, Ready, Cooldown, Done}
    public WaveState waveState = WaveState.Cooldown;
    public int waveCooldown = 5;
    private float lastWaveTime = 0;

    public int wave = 1;
    public int room = 0;

    private float minFightingDuration = 3f;
    private float fightingStart = 0;

    private UIManager uiManager;
    private GameStateManager gameStateManager;

    private void Start()
    {
        lastWaveTime = Time.time;

        uiManager = GameObject.Find("UIManager").GetComponent<UIManager>();
        gameStateManager = GameObject.Find("GameStateManager").GetComponent<GameStateManager>();

        GenerateRooms();
        int roomNumber = 0;
        foreach (Room room in rooms)
        {
            //Debug.Log("Camera location in room " + roomNumber + ": " + room.CameraLocation);

            string s = "";
            foreach (Vector3 spawnLoc in room.spawnLocations)
            {
                s += spawnLoc.ToString();
            }
            //Debug.Log("Spawn locations in room " + roomNumber + ": " + s);

            int waveNumber = 0;
            foreach (Wave wave in room.waves)
            {
                s = "";
                foreach ((string, int) enemy in wave.enemies)
                {
                    s += enemy.ToString();
                }
                //Debug.Log("Enemies in room " + roomNumber + " wave " + waveNumber + ": " + s);

                waveNumber++;
            }

            roomNumber++;
        }
        Room currentRoom = rooms[room];
        Camera.main.transform.position = currentRoom.CameraLocation;
        player.transform.position = currentRoom.PlayerSpawnLocation;
    }
    void Update()
    {
        if (wave >= 0)
        {
            if (waveState == WaveState.Ready)
            {
                SpawnWave();
                waveState = WaveState.Fighting;
                fightingStart = Time.time;
            }
            else if (waveState == WaveState.Fighting)
            {
                if(enemies.transform.childCount == 0 && Time.time - fightingStart > minFightingDuration)
                {
                    if (CheckRoomDone())
                    {
                        waveState = WaveState.Done;
                        uiManager.EnableLevelCompletedText(room + 1);
                        if (CheckGameDone())
                        {
                            StartCoroutine(PerformAfterDelay(5, () => gameStateManager.GameWon(player.GetComponent<Player>())));
                        }
                        else
                        {
                            StartCoroutine(PerformAfterDelay(5, GoToNextRoom));
                        }
                    }
                    else
                    {
                        waveState = WaveState.Cooldown;
                        lastWaveTime = Time.time;

                        bool boss = (wave == 8);
                        uiManager.PerformWaveCountdown(waveCooldown, boss);

                        if (wave == 4)
                        {
                            uiManager.EnableUpgradeUI();
                        }

                        wave++;
                    }
                }
            }
            else if (waveState == WaveState.Cooldown)
            {
                if(Time.time > lastWaveTime + waveCooldown)
                {
                    waveState = WaveState.Ready;
                    uiManager.DisableWaveUI();
                }
            }
        }
    }

    public void GoToNextRoom()
    {
        waveState = WaveState.Cooldown;
        lastWaveTime = Time.time;

        uiManager.DisableWaveUI();
        uiManager.PerformWaveCountdown(waveCooldown, false);

        wave = 0;
        room++;

        Room currentRoom = rooms[room];
        Camera.main.transform.position = currentRoom.CameraLocation;

        player.transform.position = rooms[room].PlayerSpawnLocation;
    }

    public Vector3 GetSafePosition()
    {
        return this.rooms[this.room].PlayerSpawnLocation;
    }

    private bool CheckRoomDone()
    {
        return (wave == 9);
    }

    private bool CheckGameDone()
    {
        if (room == rooms.Count - 1 && wave == 9) return true;
        return false;
    }

    private List<Vector3> FindSpawnLocations(int amount)
    {
        if (room < 0) throw new System.Exception("Room does not exist");

        Room currentRoom = rooms[room];
        List<Vector3> spawnLocations = new List<Vector3>(currentRoom.spawnLocations);
        if (spawnLocations.Count < amount) throw new System.Exception("Cannot spawn enemies, number of spawn locations is too small");

        return spawnLocations.OrderBy(x => UnityEngine.Random.Range(0, spawnLocations.Count)).Take(amount).ToList();
    }

    private void CreateEnemy(GameObject prefab, Vector3 spawnLocation)
    {
        warningTilemap.SetTile(Vector3Int.FloorToInt(spawnLocation), warningTile);
        StartCoroutine(CreateEnemyAfterDelay(prefab, spawnLocation, 1));
    }

    private IEnumerator CreateEnemyAfterDelay(GameObject prefab, Vector3 spawnLocation, int delay)
    {
        yield return new WaitForSeconds(delay);

        GameObject enemy = Instantiate(prefab, spawnLocation, Quaternion.identity);
        enemy.transform.SetParent(enemies.transform);

        warningTilemap.SetTile(Vector3Int.FloorToInt(spawnLocation), null);
    }

    [SerializeField]
    public class Wave
    {
        public List<(string,int)> enemies = new List<(string, int)>();
    }

    [SerializeField]
    public class Room
    {
        public List<Wave> waves = new List<Wave>();
        public Vector3 CameraLocation;
        public List<Vector3> spawnLocations = new List<Vector3>();
        public Vector3 PlayerSpawnLocation;
    }

    public List<Room> rooms = new List<Room>();
    public TextAsset roomsInfo;

    void SpawnWave()
    {
        Room currentRoom = rooms[room];
        Wave currentwave = currentRoom.waves[wave];
        List<(string, int)> enemies = currentwave.enemies;

        int enemyCount = 0;
        foreach ((string, int) enemy in enemies)
        {
            enemyCount += enemy.Item2;
        }

        List<Vector3> spawnLocations = FindSpawnLocations(enemyCount);

        int count = 0;
        foreach ((string, int) enemyTuple in enemies)
        {
            string enemyName = enemyTuple.Item1;
            int amount = enemyTuple.Item2;

            GameObject prefab = StringToPrefab(enemyName);
            for (int i = 0; i < amount; i++)
            {
                CreateEnemy(prefab, spawnLocations[count]);
                count += 1;
            }
        }
    }
    public void GenerateRooms()
    {
        string roomText = roomsInfo.text;

        int index = 0;
        while (index < roomText.Length)
        {
            Room newRoom = new Room();
            //ex = 0,0,0|
            string s1 = "";
            while (roomText[index] != ',')
            {
                s1 += roomText[index];
                index++;
            }
            index++;
            string s2 = "";
            while (roomText[index] != ',')
            {
                s2 += roomText[index];
                index++;
            }
            index++;
            string s3 = "";
            while (roomText[index] != '|')
            {
                s3 += roomText[index];
                index++;
            }
            newRoom.PlayerSpawnLocation = new Vector3(float.Parse(s1), float.Parse(s2), float.Parse(s3));

            //ex = 0,0,0|
            index++;
            s1 = "";
            while (roomText[index] != ',')
            {
                s1 += roomText[index];
                index++;
            }
            index++;
            s2 = "";
            while (roomText[index] != ',')
            {
                s2 += roomText[index];
                index++;
            }
            index++;
            s3 = "";
            while (roomText[index] != '|')
            {
                s3 += roomText[index];
                index++;
            }
            newRoom.CameraLocation = new Vector3(float.Parse(s1), float.Parse(s2), float.Parse(s3));
            //Debug.Log(newRoom.CameraLocation);

            //ex = 0,0,0&1,1,1|
            index++;
            while (roomText[index] != '|')
            {
                s1 = "";
                while (roomText[index] != ',')
                {
                    s1 += roomText[index];
                    index++;
                }
                index++;
                s2 = "";
                while (roomText[index] != ',')
                {
                    s2 += roomText[index];
                    index++;
                }
                index++;
                s3 = "";
                while (roomText[index] != '&' && roomText[index] != '|')
                {
                    s3 += roomText[index];
                    index++;
                }
                if (roomText[index] == '&')
                {
                    index++;
                }
                newRoom.spawnLocations.Add(new Vector3(float.Parse(s1), float.Parse(s2), float.Parse(s3)));
            }

            //ex = MeleeEnemy:5&MeleeEnemy:3,RangedEnemy:2\n
            index++;
            while (roomText[index] != '.')
            {
                Wave newWave = new Wave();
                while (roomText[index] != '\n' && roomText[index] != '.')
                {
                    string enemy = "";
                    while (roomText[index] != ':')
                    {
                        enemy += roomText[index];
                        index++;
                    }
                    index++;
                    s1 = "";
                    while (roomText[index] != ',' && roomText[index] != '\n' && roomText[index] != '.')
                    {
                        s1 += roomText[index];
                        index++;
                    }
                    if (roomText[index] == ',')
                    {
                        index++;
                    }
                    newWave.enemies.Add((enemy, int.Parse(s1)));
                }
                if (roomText[index] == '\n')
                {
                    index++;
                }
                newRoom.waves.Add(newWave);
            }
            while (roomText[index] != '\n' && index < roomText.Length)
            {
                index++;
            }
            index++;
            rooms.Add(newRoom);
        }
    }

    public GameObject StringToPrefab(string s)
    {
        if (s == "MeleeEnemy")
        {
            return meleeEnemy;
        }
        else if (s == "RangedEnemy")
        {
            return rangedEnemy;
        }
        else if (s == "FastEnemy")
        {
            return fastEnemy;
        }
        else if (s == "DodgeMeleeEnemy")
        {
            return dodgeMeleeEnemy;
        }
        else if (s == "ShotgunEnemy")
        {
            return shotgunEnemy;
        }
        else if (s == "DodgeRangedEnemy")
        {
            return dodgeRangedEnemy;
        }
        else if (s == "ChargeMeleeEnemy")
        {
            return chargeMeleeEnemy;

        }
        else if (s == "SplitBoss")
        {
            return splitBoss;
        }
        else if (s == "DashBoss")
        {
            return dashBoss;
        }
        else if (s == "RangedBoss")
        {
            return rangedBoss;
        }

        return meleeEnemy;
    }

    private IEnumerator PerformAfterDelay(float delay, Action action)
    {
        yield return new WaitForSeconds(delay);

        action();
    }
}
