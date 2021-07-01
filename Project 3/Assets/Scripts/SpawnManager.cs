using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public GameObject[] carPrefabs;
    public GameObject[] humanPrefabs;

    private GameObject player;

    // For human
    private List<Vector3> humanSpawnPos = new List<Vector3>();
    private float humanStartDelay = 0;//10;
    private float humanRepeatRate = 60;

    // For car
    private Vector3 spawnPos = new Vector3(102.5f, 0, 82.3f);
    private float spawnTimes = 9;
    private float startDelay = 0;
    private float repeatRate = 5;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("[VRTK_SDKManager]/[VRTK_SDKSetups]/SteamVR/[CameraRig]");
        InvokeRepeating("SpawnCar", startDelay, repeatRate);
        humanSpawnPos.Add(new Vector3(94.389f, 0.0f, 85.439f));
        humanSpawnPos.Add(new Vector3(94.389f, 0.0f, 134.45f));
        humanSpawnPos.Add(new Vector3(94.389f, 0.0f, 182.45f));
        humanSpawnPos.Add(new Vector3(45.64f, 0.0f, 85.439f));
        humanSpawnPos.Add(new Vector3(45.64f, 0.0f, 134.45f));
        humanSpawnPos.Add(new Vector3(45.64f, 0.0f, 182.45f));
        humanSpawnPos.Add(new Vector3(-14.61f, 0.0f, 85.439f));
        humanSpawnPos.Add(new Vector3(-14.61f, 0.0f, 134.45f));
        humanSpawnPos.Add(new Vector3(-14.61f, 0.0f, 182.45f));
        InvokeRepeating("SpawnHuman", humanStartDelay, humanRepeatRate);
    }

    void SpawnCar()
    {
        int rand = Mathf.FloorToInt(Random.Range(0, 3.99f));
        Instantiate(carPrefabs[rand], spawnPos, carPrefabs[rand].transform.rotation);
        if(--spawnTimes == 0)
        {
            CancelInvoke("SpawnCar");
        }
    }

    void SpawnHuman()
    {
        int rand = Mathf.FloorToInt(Random.Range(0, 1.99f));
        float minDistance = float.MaxValue;
        int minIndex = -1;
        for(int i = 0; i < humanSpawnPos.Count; i++)
        {
            float currentDistance = Vector3.Distance(player.transform.position, humanSpawnPos[i]);
            if(currentDistance < minDistance)
            {
                minDistance = currentDistance;
                minIndex = i;
            }
        }
        Instantiate(humanPrefabs[rand], humanSpawnPos[minIndex], humanPrefabs[rand].transform.rotation);
    }
    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Z))
        {
            Instantiate(humanPrefabs[0],GameManager.gm.smallleopard.transform.position + GameManager.gm.smallleopard.transform.forward*5,humanPrefabs[0].transform.rotation);
        }
        if(Input.GetKeyDown(KeyCode.X))
        {
            Instantiate(humanPrefabs[1],GameManager.gm.smallleopard.transform.position + GameManager.gm.smallleopard.transform.forward*5,humanPrefabs[1].transform.rotation);
        }    
    }
}
