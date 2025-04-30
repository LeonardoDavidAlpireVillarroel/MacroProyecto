using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProfileSpawner : MonoBehaviour
{
    public Transform newGameSpawn;
    public GameObject playerPrefab;

    void Start()
    {
        //Nueva Partida
        if (ProfileStorage.s_currentProfile.newGame)
        {
            Instantiate(this.playerPrefab, this.newGameSpawn.position, Quaternion.identity);
        }
        else
        {
            //Cargar Partida
            float x = ProfileStorage.s_currentProfile.x;
            float y = ProfileStorage.s_currentProfile.y;

            Vector3 pos = new Vector3(x, y, 0);

            Instantiate(this.playerPrefab, pos, Quaternion.identity);
        }
    }
}
