using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public GameState gameState;
    public GameObject player;
    public Camera camera;

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

    public enum WaveState {Fighting,Ready,Cooldown}
    public WaveState waveState = WaveState.Ready;
    public float waveCooldown = 3;
    private float lastWaveTime = 0;

    public int wave = 1;
    public int room = 0;

    private void Start()
    {
        GenerateRooms();
        foreach (Room room in rooms)
        {
            string s = "";
            s += room.CameraLocation.ToString();
            foreach (Vector3 spawnLoc in room.spawnLocations)
            {
                s += spawnLoc.ToString();
            }
            Debug.Log(s);
            foreach (Wave wave in room.waves)
            {
                Debug.Log("Wave:");
                s = "";
                foreach ((string, int) enemy in wave.enemies)
                {
                    s += enemy.ToString();
                }
                Debug.Log(s);
            }
        }
        Room currentRoom = rooms[room];
        camera.transform.position = currentRoom.CameraLocation;
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
            }
            else if (waveState == WaveState.Fighting)
            {
                if(enemies.transform.childCount == 0)
                {
                    Room currentRoom = rooms[room];
                    camera.transform.position = currentRoom.CameraLocation;
                    waveState = WaveState.Cooldown;
                    if (wave == 9)
                    {
                        wave = -1;
                        room++;

                        player.GetComponent<Player>().ClassUp();
                        player.GetComponent<Player>().maxHealth += 50;
                        player.GetComponent<Player>().health = player.GetComponent<Player>().maxHealth;

                        player.transform.position = rooms[room].PlayerSpawnLocation;
                    }
                    else if(wave == 4)
                    {
                        player.GetComponent<Player>().LevelUp();
                    }
                    wave++;
                }
                lastWaveTime = Time.time;
            }
            else if (waveState == WaveState.Cooldown)
            {
                if(Time.time > lastWaveTime + waveCooldown)
                {
                    waveState = WaveState.Ready;
                }
            }
        }
    }

    private Vector2 FindSpawnLocation()
    {
        if (room >= 0)
        {
            Room currentRoom = rooms[room];
            List<Vector3> spawnLocations = currentRoom.spawnLocations;
            int randomindex = Random.Range(0, spawnLocations.Count);
            Vector3 resultLocation = spawnLocations[randomindex];
            return new Vector2(resultLocation.x, resultLocation.y);
        }
        int random_x = Random.Range(7, 9);
        int random_y = Random.Range(3, 5);
        int random_xv = Random.Range(0, 2);
        int random_yv = Random.Range(0, 2);
        if (random_xv == 1)
        {
            random_x = -random_x;
        }
        if (random_yv == 1)
        {
            random_y = -random_y;
        }
        return new Vector2(random_x, random_y);
    }

    private GameObject CreateEnemy(GameObject prefab)
    {
        Vector2 spawnloc = FindSpawnLocation();
        GameObject enemy = Instantiate(prefab, spawnloc, Quaternion.identity);
        enemy.GetComponent<Enemy>().player = player;
        enemy.transform.SetParent(enemies.transform);
        return enemy;
    }

    /*void SpawnWave()
    {
        if(act == 1)
        {
            if (wave == 1)
            {
                for (int i = 0; i < 1; i++)
                {
                    CreateMeleeEnemy();
                }
            }
            else if (wave == 2)
            {
                for (int i = 0; i < 2; i++)
                {
                    CreateMeleeEnemy();
                }
            }
            else if (wave == 3)
            {
                for (int i = 0; i < 3; i++)
                {
                    CreateMeleeEnemy();
                }
            }
            else if (wave == 4)
            {
                for (int i = 0; i < 4; i++)
                {
                    CreateMeleeEnemy();
                }
            }
            else if (wave == 5)
            {
                for (int i = 0; i < 3; i++)
                {
                    CreateMeleeEnemy();
                }
                for (int i = 0; i < 1; i++)
                {
                    CreateRangedEnemy();
                }
            }
            else if (wave == 6)
            {
                player.GetComponent<Player>().LevelUp();
                for (int i = 0; i < 4; i++)
                {
                    CreateRangedEnemy();
                }
            }
            else if (wave == 7)
            {
                for (int i = 0; i < 3; i++)
                {
                    CreateMeleeEnemy();
                }
                for (int i = 0; i < 3; i++)
                {
                    CreateRangedEnemy();
                }
            }
            else if (wave == 8)
            {
                for (int i = 0; i < 4; i++)
                {
                    CreateFastEnemy();
                }
            }
            else if (wave == 9)
            {
                for (int i = 0; i < 2; i++)
                {
                    CreateFastEnemy();
                }
                for (int i = 0; i < 2; i++)
                {
                    CreateMeleeEnemy();
                }
                for (int i = 0; i < 2; i++)
                {
                    CreateRangedEnemy();
                }
            }
            else if (wave == 10)
            {
                for (int i = 0; i < 1; i++)
                {
                    CreateSplitBoss();
                }
            }
        }
        if(act == 2)
        {
            player.GetComponent<Player>().ClassUp();
            if (wave == 1)
            {
                for (int i = 0; i < 4; i++)
                {
                    CreateMeleeEnemy();
                }
            }
            else if (wave == 2)
            {
                for (int i = 0; i < 3; i++)
                {
                    CreateMeleeEnemy();
                }
                for (int i = 0; i < 2; i++)
                {
                    CreateRangedEnemy();
                }
            }
            else if (wave == 3)
            {
                for (int i = 0; i < 4; i++)
                {
                    CreateFastEnemy();
                }
            }
            else if (wave == 4)
            {
                for (int i = 0; i < 2; i++)
                {
                    CreateMeleeEnemy();
                }
                for (int i = 0; i < 2; i++)
                {
                    CreateRangedEnemy();
                }
                for (int i = 0; i < 2; i++)
                {
                    CreateFastEnemy();
                }
            }
            else if (wave == 5)
            {
                for (int i = 0; i < 3; i++)
                {
                    CreateMeleeEnemy();
                }
                for (int i = 0; i < 5; i++)
                {
                    CreateRangedEnemy();
                }
            }
            else if (wave == 6)
            {
                for (int i = 0; i < 3; i++)
                {
                    CreateMeleeEnemy();
                }
                for (int i = 0; i < 2; i++)
                {
                    CreateShotgunEnemy();
                }
            }
            else if (wave == 7)
            {
                for (int i = 0; i < 2; i++)
                {
                    CreateFastEnemy();
                }
                for (int i = 0; i < 3; i++)
                {
                    CreateMeleeEnemy();
                }
                for (int i = 0; i < 2; i++)
                {
                    CreateShotgunEnemy();
                }
            }
            else if (wave == 8)
            {
                for (int i = 0; i < 10; i++)
                {
                    CreateFastEnemy();
                }
            }
            else if (wave == 9)
            {
                for (int i = 0; i < 2; i++)
                {
                    CreateFastEnemy();
                }
                for (int i = 0; i < 3; i++)
                {
                    CreateMeleeEnemy();
                }
                for (int i = 0; i < 2; i++)
                {
                    CreateShotgunEnemy();
                }
                for (int i = 0; i < 3; i++)
                {
                    CreateRangedEnemy();
                }
            }
            else if (wave == 10)
            {
                for (int i = 0; i < 3; i++)
                {
                    CreateDashBoss();
                }
            }
        }
    }*/

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
        foreach ((string, int) enemy in enemies)
        {
            SpawnEnemies(enemy);
        }
    }

    public void SpawnEnemies((string, int) tuple)
    {
        string enemy = tuple.Item1;
        int amount = tuple.Item2;
        GameObject prefab = StringToPrefab(enemy);
        for (int i = 0; i < amount; i++)
        {
            CreateEnemy(prefab);
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
    
}
