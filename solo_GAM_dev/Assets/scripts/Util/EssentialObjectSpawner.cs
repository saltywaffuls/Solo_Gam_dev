using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EssentialObjectSpawner : MonoBehaviour
{
    [SerializeField] GameObject essentialObjectPreFab;

    private void Awake()
    {
        var existingObjects = FindObjectsOfType<EssentialObjects>();
        if (existingObjects.Length == 0)
        {
            // if there is a grid spawn at center
            var spawnPos = new Vector3(0, 0, 0);

            var grid = FindObjectOfType<Grid>();
            if (grid != null)
                spawnPos = grid.transform.position;

            Instantiate(essentialObjectPreFab, spawnPos, Quaternion.identity);
        }
    }
}
