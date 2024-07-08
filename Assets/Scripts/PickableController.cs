using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickableController : MonoBehaviour, IInteractable
{
    [Header("Sound effects")]
    [SerializeField] AudioClip pickedSound;

    public void Interact()
    {
        AudioSource.PlayClipAtPoint(pickedSound, transform.position);
        Destroy(gameObject);
    }
}