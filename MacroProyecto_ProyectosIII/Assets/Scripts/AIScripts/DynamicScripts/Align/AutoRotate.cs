using UnityEngine;

public class AutoRotate : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 90f; // grados por segundo

    void Update()
    {
        transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
    }
}