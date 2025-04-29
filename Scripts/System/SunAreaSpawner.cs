using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class SunAreaSpawner : SingletonMonoBehaviour<SunAreaSpawner>
{
    [SerializeField] private Transform sunAreaContainer;
    [SerializeField] private int sunAreaCount = 5;
    [SerializeField] private GameObject sunAreaPrefab;
    [SerializeField] private Vector2 spawnPositionRange = new Vector2(-5, 5);
    [SerializeField] private Vector2 sunAreaSizeRange = new Vector2(1, 3);

    public void EnlargeSunArea() => sunAreaSizeRange *= 1.5f;
    
    public void SpawnSunArea()
    {
        for (var i = 0; i < sunAreaCount; i++)
        {
            var spawnPosition = new Vector3(
                Random.Range(-spawnPositionRange.x, spawnPositionRange.x),
                Random.Range(-spawnPositionRange.y, spawnPositionRange.y),
                0
            );

            var sunArea = Instantiate(sunAreaPrefab, spawnPosition, Quaternion.identity, sunAreaContainer);
            var sizeX = Random.Range(sunAreaSizeRange.x, sunAreaSizeRange.y);
            var sizeY = Random.Range(sunAreaSizeRange.x, sunAreaSizeRange.y);
            sunArea.transform.localScale = new Vector3(sizeX, sizeY, 1);
        }
    }

    public void Reset()
    {
        foreach (Transform child in sunAreaContainer)
        {
            Destroy(child.gameObject);
        }
    }
}
