using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleParallaxer : MonoBehaviour
{
    class PoolObject
    {
        public Transform transform;
        public bool inUse;
        public PoolObject(Transform t) { transform = t; }
        public void Use() { inUse = true; }
        public void Dispose() { inUse = false; }

    }

    [System.Serializable]
    public struct SpawnObject
    {

    }

    public GameObject prefab;
    public Sprite sprite1;
    public Sprite sprite2;
    public int poolSize;
    public float shiftSpeed;
    public float spawnRate;

    public Vector3 defaultPillarSpawnPos;
    public Vector3 defaultHeadSpawnPos;
    public bool spawnImmediate;
    public Vector3 immediateSpawnPos;
    public Vector2 targetAspectRatio;

    float spawnTimer;
    float targetAspect;
    PoolObject[] poolObjects;

    GameManager game;

    void Awake()
    {
        Configure();
    }

    void Start()
    {
        game = GameManager.Instance;
    }

    void OnEnable()
    {
        GameManager.OnGameOverConfirmed += OnGameOverConfirmed;
    }

    void OnDisable()
    {
        GameManager.OnGameOverConfirmed -= OnGameOverConfirmed;
    }

    void OnGameOverConfirmed()
    {
        for (int i = 0; i < poolObjects.Length; i++)
        {
            poolObjects[i].Dispose();
            poolObjects[i].transform.position = Vector3.one * 1000;
        }
        if (spawnImmediate)
        {
            SpawnImmediate();
        }
    }

    void Update()
    {
        if (game.GameOver) return;
        if (game.IsPaused) return;


        Shift();

        if (game.InBossFight)
        {
            BossFightDisposeAll();
            return;
        }

        spawnTimer += Time.deltaTime;
        if (spawnTimer > spawnRate)
        {
            Spawn();
            spawnTimer = 0;
        }
    }

    void Configure()
    {
        targetAspect = targetAspectRatio.x / targetAspectRatio.y;
        poolObjects = new PoolObject[poolSize];
        for (int i = 0; i < poolObjects.Length; i++)
        {
            GameObject go = Instantiate(prefab) as GameObject;
            Transform t = go.transform;
            t.SetParent(transform);
            t.position = Vector3.one * 1000;
            poolObjects[i] = new PoolObject(t);
        }

        if (spawnImmediate)
        {
            SpawnImmediate();
        }
    }

    void Spawn()
    {
        Transform t = GetPoolObject();
        if (t == null) return;
        Vector3 pos = Vector3.zero;
        
        System.Random random = new System.Random();
        int rand = random.Next(1, 50);
        if (rand > 25)
        {
            t.GetComponent<SpriteRenderer>().sprite = sprite1;
            t.rotation = Quaternion.Euler(0, 0, -74);
            pos.x = defaultHeadSpawnPos.x;
            pos.y = defaultHeadSpawnPos.y;
            t.position = pos;

        } else
        {
            t.GetComponent<SpriteRenderer>().sprite = sprite2;
            t.rotation = Quaternion.Euler(0, 0, 0);
            pos.x = defaultPillarSpawnPos.x;
            pos.y = defaultPillarSpawnPos.y;
            t.position = pos;
        }
    }

    void SpawnImmediate()
    {
        Transform t = GetPoolObject();
        if (t == null) return;
        Vector3 pos = Vector3.zero;
        pos.x = immediateSpawnPos.x;
        pos.y = immediateSpawnPos.y;
        t.position = pos;
        Spawn();
    }

    void Shift()
    {
        for (int i = 0; i < poolObjects.Length; i++)
        {
            poolObjects[i].transform.position += -Vector3.right * shiftSpeed * Time.deltaTime;
            CheckDisposeObject(poolObjects[i]);
        }
    }

    void CheckDisposeObject(PoolObject poolObject)
    {
        if (poolObject.transform.position.x < -defaultHeadSpawnPos.x)
        {
            poolObject.Dispose();
            poolObject.transform.position = Vector3.one * 1000;
        }
    }

    void BossFightDisposeAll()
    {
        for (int i = 1; i < poolObjects.Length; i++)
        {
            poolObjects[i].Dispose();
            poolObjects[i].transform.position = Vector3.one * 1000;
        }
    }

    Transform GetPoolObject()
    {
        for (int i = 0; i < poolObjects.Length; i ++)
        {
            if (!poolObjects[i].inUse)
            {
                poolObjects[i].Use();
                return poolObjects[i].transform;
            }
        }
        return null;
    }

}
