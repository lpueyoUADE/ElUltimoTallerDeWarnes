using EstructurasDeDatos.Conjunto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EstructurasDeDatos.Grafo
{
    public class GrafoEstatico<T> : IGrafoDirigido<T> where T : IComparable<T>
    {
        private List<T> vertices;
        private int[,] adjacencyMatrix;
        private int count; // Contador de vértices agregados

        public GrafoEstatico(int numVertices)
        {
            vertices = new List<T>(new T[numVertices]);
            adjacencyMatrix = new int[numVertices, numVertices];
            count = 0;
        }

        public int VerticesCount => count;

        public void AddVertex(T vertex)
        {
            if (count >= vertices.Count)
            {
                throw new InvalidOperationException($"Cannot add more vertices. Maximum number of vertices ({vertices.Count}) reached.");
            }

            vertices[count] = vertex;
            count++;
        }

        public void AddEdge(T from, T to, int weight)
        {
            int fromIndex = vertices.IndexOf(from);
            int toIndex = vertices.IndexOf(to);

            if (fromIndex == -1 || toIndex == -1)
            {
                throw new ArgumentException($"One or both vertices do not exist in the graph.");
            }

            adjacencyMatrix[fromIndex, toIndex] = weight;
        }

        public bool HasVertex(T vertex)
        {
            return vertices.Contains(vertex);
        }

        public bool HasEdge(T from, T to)
        {
            int fromIndex = vertices.IndexOf(from);
            int toIndex = vertices.IndexOf(to);

            if (fromIndex == -1 || toIndex == -1)
            {
                return false;
            }

            return adjacencyMatrix[fromIndex, toIndex] != 0;
        }

        public int GetEdgeWeight(T from, T to)
        {
            int fromIndex = vertices.IndexOf(from);
            int toIndex = vertices.IndexOf(to);

            if (fromIndex == -1 || toIndex == -1 || adjacencyMatrix[fromIndex, toIndex] == 0)
            {
                throw new ArgumentException($"Edge from {from} to {to} does not exist in the graph.");
            }

            return adjacencyMatrix[fromIndex, toIndex];
        }

        public HashSet<T> GetVertices()
        {
            var currentVertices = new HashSet<T>(vertices.GetRange(0, count)); // Solo devuelve los vértices hasta el count
            return currentVertices;
        }

        public List<T> GetNeighbors(T vertex)
        {
            int vertexIndex = vertices.IndexOf(vertex);

            if (vertexIndex == -1)
            {
                throw new ArgumentException($"Vertex {vertex} does not exist in the graph.");
            }

            List<T> neighbors = new List<T>();
            for (int j = 0; j < count; j++) // Solo recorre hasta count
            {
                if (adjacencyMatrix[vertexIndex, j] != 0)
                {
                    neighbors.Add(vertices[j]);
                }
            }

            return neighbors;
        }

        public Dictionary<T, List<T>> Dijkstra(T source)
        {
            int sourceIndex = vertices.IndexOf(source);
            if (sourceIndex == -1)
            {
                throw new ArgumentException($"Source vertex {source} does not exist in the graph.");
            }

            var distances = new List<int>(new int[count]);
            var previous = new List<T>(new T[count]);
            var unvisited = new HashSet<int>();

            for (int i = 0; i < count; i++) // Solo inicializa hasta count
            {
                distances[i] = int.MaxValue;
                previous[i] = default(T);
                unvisited.Add(i);
            }
            distances[sourceIndex] = 0;

            while (unvisited.Count > 0)
            {
                int current = -1;
                int shortestDistance = int.MaxValue;

                foreach (var vertexIndex in unvisited)
                {
                    if (distances[vertexIndex] < shortestDistance)
                    {
                        shortestDistance = distances[vertexIndex];
                        current = vertexIndex;
                    }
                }

                if (current == -1 || distances[current] == int.MaxValue)
                {
                    break;
                }

                unvisited.Remove(current);

                for (int i = 0; i < count; i++) // Solo recorre hasta count
                {
                    if (adjacencyMatrix[current, i] != 0)
                    {
                        int newDist = distances[current] + adjacencyMatrix[current, i];

                        if (newDist < distances[i])
                        {
                            distances[i] = newDist;
                            previous[i] = vertices[current];
                        }
                    }
                }
            }

            var paths = new Dictionary<T, List<T>>();
            foreach (var vertex in vertices.GetRange(0, count)) // Solo itera hasta count
            {
                var path = new List<T>();
                T at = vertex;
                while (at != null && !at.Equals(default(T)))
                {
                    path.Insert(0, at);
                    at = previous[vertices.IndexOf(at)];
                }

                if (path.Count > 0 && path[0].Equals(source))
                {
                    paths[vertex] = path;
                }
            }

            return paths;
        }
        public void PrintGraph()
        {
            Console.WriteLine("Vertices:");
            foreach (var vertex in vertices.GetRange(0, count)) // Solo itera hasta count
            {
                Console.WriteLine(vertex);
                Console.WriteLine("Neighbors:");
                var neighbors = GetNeighbors(vertex);
                foreach (var neighbor in neighbors)
                {
                    int weight = GetEdgeWeight(vertex, neighbor);
                    Console.WriteLine($"- {neighbor} (Weight: {weight})");
                }
                Console.WriteLine();
            }
        }
    }
}
