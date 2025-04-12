[System.Serializable]
public class PlayerData
{
    public int healh;
    public int score;
    public float[] position = new float[3];

    public PlayerData(PlayerController player)
    { 
        healh = player.health;               
        score = player.score;
        position[0] = player.transform.position.x;
        position[1] = player.transform.position.y;
        position[2] = player.transform.position.z;
    }
}
