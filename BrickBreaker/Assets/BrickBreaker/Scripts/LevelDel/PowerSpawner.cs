using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerSpawner : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(SpawnIteration());
    }

    void OnDestroy()
    {
        StopAllCoroutines();  
    }

    private IEnumerator SpawnIteration()
    {
        float delay = 0;
        while(delay < 2.5f)
        {
            float delay2 = 0;
            while (delay2 < 0.1f)
            {
                yield return null;
                delay += Time.deltaTime;
                delay2 += Time.deltaTime;
            }
        }

        SpawnPower();
        StartCoroutine(SpawnIteration());
    }

    private void SpawnPower()
    {
        // Spawn random power
        PowersSystem.powersSpawned++;
        int power = Random.Range(1, 5);
        GameObject powerToSpawn = null;
        string path = "Prefabs/LevelDev/Powers";
        switch (power)
        {
            case 1:
                powerToSpawn = Resources.Load<GameObject>($"{path}/PowerFast");
                break;

            case 2:
                powerToSpawn = Resources.Load<GameObject>($"{path}/PowerSlow");
                break;

            case 3:
                powerToSpawn = Resources.Load<GameObject>($"{path}/PowerSmall");
                break;

            case 4:
                powerToSpawn = Resources.Load<GameObject>($"{path}/PowerLarge");
                break;

            default:
                return;
        }
        if (powerToSpawn != null)
            Instantiate(powerToSpawn, transform.position, Quaternion.identity);
    }

}