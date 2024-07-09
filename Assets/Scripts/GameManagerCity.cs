using EstructurasDeDatos.AVL;
using EstructurasDeDatos.Grafo;
using EstructurasDeDatos.Lista;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManagerCity : MonoBehaviour
{
    [SerializeField] Transform mapPointsContainer;
    [SerializeField] ArrowController arrow;
    [SerializeField] AudioClip pickupSound;

    private GrafoEstatico<NodoController> grafo;
    private List<NodoController> mapPointsList;
    private AVL<DistanciaNodo> nodosByDistancia;

    public Transform car;

    public static GameManagerCity Instance { get; private set; }

    private List<NodoController> camino;

    public TMP_Text puntosText;
    private int points;
    private int count;

    private void Awake()
    {
        // Si ya hay una instancia y no es esta, destruye este objeto.
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            // Si esta es la primera instancia, asígnala y marca este objeto para no destruirlo al cargar una nueva escena.
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public struct DistanciaNodo : IComparable<DistanciaNodo>
    {
        public int distancia;
        public NodoController nodo;

        public DistanciaNodo(int distancia, NodoController nodo) : this()
        {
            this.distancia = distancia;
            this.nodo = nodo;
        }

        public int CompareTo(DistanciaNodo other)
        {
            return distancia - other.distancia;
        }
    }

    void populateNodosByDistance()
    {
        nodosByDistancia = new(); // TODO estaria bueno un update en AVL en vez de construirlo cada vez.

        foreach (NodoController nodo in grafo.GetVertices())
        {
            nodosByDistancia.Insert(new DistanciaNodo((int)(car.transform.position - nodo.transform.position).magnitude, nodo));
        };
    }

    private void AddPoints(int points)
    {
        this.points += points;
        puntosText.text = this.points.ToString();
    }

    // Start is called before the first frame update
    void Start()
    {
        points = 0;
        AddPoints(0);

        grafo = new GrafoEstatico<NodoController>(100);

        // Agrego todos los vertices.
        mapPointsList = mapPointsContainer.GetComponentsInChildren<NodoController>().ToList();

        foreach (Transform nodo in mapPointsContainer)
        {
            grafo.AddVertex(nodo.GetComponent<NodoController>());
        }
        // Agrego las aristas.
        foreach (NodoController nodo in mapPointsList)
        {
            foreach (NodoController vecino in nodo.Vecinos)
            {
                grafo.AddEdge(nodo, vecino, 1);
            }
        }

        GeneratePath();
    }

    public void Checkpoint(string name)
    {
        AudioSource.PlayClipAtPoint(pickupSound, transform.position);

        camino[count].SetAsInactive();

        count++;
        if(count < camino.Count)
            arrow.pointAt = camino[count].transform;

        if (count == camino.Count || name == camino[camino.Count - 1].name)
        {
            GeneratePath();
            AddPoints(100);
        } else
        {
            AddPoints(10);
        }
    }

    void GeneratePath()
    {
        count = 0;
        populateNodosByDistance();

        List<DistanciaNodo> inOrder = nodosByDistancia.InOrderTraversal();

        // Elijo de entre los elementos mas lejanos.
        NodoController masCercano = inOrder[0].nodo;
        NodoController masLejano = inOrder[inOrder.Count - 1].nodo;

        camino = grafo.Dijkstra(masCercano)[masLejano];
      
        foreach(NodoController nodo in grafo.GetVertices())
        {
            nodo.SetAsInactive();
        }
        
        foreach (NodoController nodo in camino)
        {
            nodo.SetAsPath();
        }
        
        camino[camino.Count - 1].SetAsDestination();

        arrow.pointAt = camino[0].transform;
    }

    // Update is called once per frame
    void Update()
    {

    }
}