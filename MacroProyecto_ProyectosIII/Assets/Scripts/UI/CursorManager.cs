using UnityEngine;

public class CursorManager : MonoBehaviour
{
    void Update()
    {
        GameManager.Instance.UpdateCursorState();
    }
}
