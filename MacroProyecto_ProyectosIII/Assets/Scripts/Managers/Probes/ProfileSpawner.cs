using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProfileSpawner : MonoBehaviour
{
    public Transform newGameSpawn;
    public GameObject playerPrefab;

    void Start()
    {
        if (true)
        {
            Instantiate(this.playerPrefab, this.newGameSpawn.position, Quaternion.identity);
        }
        else
        {
            //Cargar partida
        }
    }
}
