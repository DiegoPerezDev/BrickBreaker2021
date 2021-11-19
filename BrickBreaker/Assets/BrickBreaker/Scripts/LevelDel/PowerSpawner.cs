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

        SpawnRandomPower();
        StartCoroutine(SpawnIteration());
    }

    private void SpawnRandomPower()
    {
        PowersSystem.powersSpawned++;
        int power = Random.Range(1, 5);
        GameObject powerToSpawn = null;
        
        switch (power)
        {
            case 1:
                powerToSpawn = PowersSystem.fastPowerCapsule;
                break;

            case 2:
                powerToSpawn = PowersSystem.slowPowerCapsule;
                break;

            case 3:
                powerToSpawn = PowersSystem.smallPowerCapsule;
                break;

            case 4:
                powerToSpawn = PowersSystem.largePowerCapsule;
                break;

            default:
                return;
        }
        if (powerToSpawn != null)
            Instantiate(powerToSpawn, transform.position, Quaternion.identity);
    }

}