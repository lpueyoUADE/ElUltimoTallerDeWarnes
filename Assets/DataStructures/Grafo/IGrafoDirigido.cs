using EstructurasDeDatos.Conjunto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EstructurasDeDatos.Grafo
{
    // Interfaz para el Grafo dirigido con pesos enteros
    public interface IGrafoDirigido<T> where T : IComparable<T>
    {
        // Propiedades
        int VerticesCount { get; }

        // Métodos
        void AddVertex(T vertex);
        void AddEdge(T from, T to, int weight);
        bool HasVertex(T vertex);
        bool HasEdge(T from, T to);
        int GetEdgeWeight(T from, T to);
        HashSet<T> GetVertices();
        List<T> GetNeighbors(T vertex);
        Dictionary<T, List<T>> Dijkstra(T source);
    }
}
