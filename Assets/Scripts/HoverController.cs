using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverController : MonoBehaviour
{



    public float amplitude = 0.5f; // La altura del movimiento
    public float frequency = 1f; // La velocidad del movimiento

    private Vector3 startPosition;
    private AudioSource audioSource;

    void Start()
    {
        startPosition = transform.position; // Guarda la posición inicial
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        float newY = startPosition.y + Mathf.Sin(Time.time * frequency) * amplitude;
        transform.position = new Vector3(startPosition.x, newY, startPosition.z);
    }
    private void OnTriggerEnter(Collider other)
    {
        AudioSource.PlayClipAtPoint(audioSource.clip, startPosition);
        GameManagerCity.Instance.Checkpoint(name);
    }
}
