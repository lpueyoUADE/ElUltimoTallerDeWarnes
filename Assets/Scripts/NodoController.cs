using ArcadeVehicleController;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class NodoController : MonoBehaviour, IComparable<NodoController>

{

    [SerializeField] List<NodoController> vecinos;
    [SerializeField] GameObject destinationOrb;
    [SerializeField] GameObject pathOrb;
    public List<NodoController> Vecinos { get => vecinos; set => vecinos = value; }

    private void Start()
    {
        SetAsInactive();
    }

    public void SetAsDestination()
    {
        destinationOrb.SetActive(true);
        pathOrb.SetActive(false);
    }

    public void SetAsPath()
    {
        destinationOrb.SetActive(false);
        pathOrb.SetActive(true);
    }

    public void SetAsInactive()
    {
        destinationOrb.SetActive(false);
        pathOrb.SetActive(false);
    }

    public int CompareTo(NodoController other)
    {
        return (int)(this.transform.position-other.transform.position).magnitude;
    }

#if UNITY_EDITOR
#nullable enable
    private void OnDrawGizmosSelected()
    {
        foreach (NodoController controller in Vecinos)
        {
            if (controller != null) {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(this.transform.position,controller.transform.position);
            }
        }
    }
#nullable disable
#endif
}
