using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EstructurasDeDatos.AVL
{
    public class AVL<T> : IAVL<T> where T : IComparable<T>
    {
        private Nodo<T>? root;
        private int count;

        // Constructor
        public AVL()
        {
            root = null;
            count = 0;
        }

        // Propiedad que devuelve la cantidad de elementos en el árbol
        public int Length => count;

        // Método para insertar un valor en el árbol
        public void Insert(T value)
        {
            root = Insert(root, value);
        }

        // Método auxiliar recursivo para insertar un valor en el árbol
        private Nodo<T> Insert(Nodo<T>? node, T value)
        {
            // Si el nodo actual es null, crear un nuevo nodo con el valor
            if (node == null)
            {
                count++;
                return new Nodo<T>(value);
            }

            // Comparar el valor con el valor del nodo actual
            int compare = value.CompareTo(node.Value);
            if (compare < 0)
                node.Left = Insert(node.Left, value); // Insertar en el subárbol izquierdo
            else if (compare > 0)
                node.Right = Insert(node.Right, value); // Insertar en el subárbol derecho
            else
                return node; // Si el valor es igual, no hacer nada

            // Actualizar la altura del nodo actual
            node.Height = 1 + Math.Max(GetHeight(node.Left), GetHeight(node.Right));

            // Obtener el balance del nodo actual
            int balance = GetBalance(node);

            // Realizar las rotaciones necesarias para mantener el balance del árbol
            if (balance > 1 && value.CompareTo(node.Left.Value) < 0)
                return RightRotate(node);

            if (balance < -1 && value.CompareTo(node.Right.Value) > 0)
                return LeftRotate(node);

            if (balance > 1 && value.CompareTo(node.Left.Value) > 0)
            {
                node.Left = LeftRotate(node.Left);
                return RightRotate(node);
            }

            if (balance < -1 && value.CompareTo(node.Right.Value) < 0)
            {
                node.Right = RightRotate(node.Right);
                return LeftRotate(node);
            }

            return node;
        }

        // Método para eliminar un valor del árbol
        public void Delete(T value)
        {
            root = Delete(root, value);
        }

        // Método auxiliar recursivo para eliminar un valor del árbol
        private Nodo<T> Delete(Nodo<T> node, T value)
        {
            // Si el nodo actual es null, no hacer nada
            if (node == null)
                return node;

            // Comparar el valor con el valor del nodo actual
            int compare = value.CompareTo(node.Value);
            if (compare < 0)
                node.Left = Delete(node.Left, value); // Eliminar del subárbol izquierdo
            else if (compare > 0)
                node.Right = Delete(node.Right, value); // Eliminar del subárbol derecho
            else
            {
                // Nodo con solo un hijo o sin hijos
                if ((node.Left == null) || (node.Right == null))
                {
                    Nodo<T> temp = null;
                    if (temp == node.Left)
                        temp = node.Right;
                    else
                        temp = node.Left;

                    // Caso sin hijos
                    if (temp == null)
                    {
                        temp = node;
                        node = null;
                        count--;
                    }
                    else
                    {
                        node = temp; // Caso con un hijo
                        count--;
                    }
                }
                else
                {
                    // Nodo con dos hijos
                    Nodo<T> temp = GetMinValueNode(node.Right);
                    node.Value = temp.Value;
                    node.Right = Delete(node.Right, temp.Value);
                }
            }

            // Si el nodo actual es null, no hacer nada
            if (node == null)
                return node;

            // Actualizar la altura del nodo actual
            node.Height = 1 + Math.Max(GetHeight(node.Left), GetHeight(node.Right));

            // Obtener el balance del nodo actual
            int balance = GetBalance(node);

            // Realizar las rotaciones necesarias para mantener el balance del árbol
            if (balance > 1 && GetBalance(node.Left) >= 0)
                return RightRotate(node);

            if (balance > 1 && GetBalance(node.Left) < 0)
            {
                node.Left = LeftRotate(node.Left);
                return RightRotate(node);
            }

            if (balance < -1 && GetBalance(node.Right) <= 0)
                return LeftRotate(node);

            if (balance < -1 && GetBalance(node.Right) > 0)
            {
                node.Right = RightRotate(node.Right);
                return LeftRotate(node);
            }

            return node;
        }

        // Método para buscar un valor en el árbol
        public bool Search(T value)
        {
            return Search(root, value) != null;
        }

        // Método auxiliar recursivo para buscar un valor en el árbol
        private Nodo<T> Search(Nodo<T> node, T value)
        {
            // Si el nodo actual es null o el valor del nodo actual es igual al valor buscado
            if (node == null || node.Value.CompareTo(value) == 0)
                return node;

            // Buscar en el subárbol izquierdo o derecho dependiendo del valor
            if (node.Value.CompareTo(value) > 0)
                return Search(node.Left, value);

            return Search(node.Right, value);
        }

        // Método para realizar el recorrido in-order del árbol
        public List<T> InOrderTraversal()
        {
            var result = new List<T>();
            InOrderTraversal(root, result);
            return result;
        }

        // Método auxiliar recursivo para el recorrido in-order del árbol
        private void InOrderTraversal(Nodo<T> node, List<T> result)
        {
            if (node != null)
            {
                InOrderTraversal(node.Left, result);
                result.Add(node.Value);
                InOrderTraversal(node.Right, result);
            }
        }

        // Método para realizar el recorrido pre-order del árbol
        public List<T> PreOrderTraversal()
        {
            var result = new List<T>();
            PreOrderTraversal(root, result);
            return result;
        }

        // Método auxiliar recursivo para el recorrido pre-order del árbol
        private void PreOrderTraversal(Nodo<T> node, List<T> result)
        {
            if (node != null)
            {
                result.Add(node.Value);
                PreOrderTraversal(node.Left, result);
                PreOrderTraversal(node.Right, result);
            }
        }

        // Método para realizar el recorrido post-order del árbol
        public List<T> PostOrderTraversal()
        {
            var result = new List<T>();
            PostOrderTraversal(root, result);
            return result;
        }

        // Método auxiliar recursivo para el recorrido post-order del árbol
        private void PostOrderTraversal(Nodo<T> node, List<T> result)
        {
            if (node != null)
            {
                PostOrderTraversal(node.Left, result);
                PostOrderTraversal(node.Right, result);
                result.Add(node.Value);
            }
        }

        // Método auxiliar para obtener la altura de un nodo
        private int GetHeight(Nodo<T> node)
        {
            return node == null ? 0 : node.Height;
        }

        // Método auxiliar para obtener el balance de un nodo
        private int GetBalance(Nodo<T> node)
        {
            return node == null ? 0 : GetHeight(node.Left) - GetHeight(node.Right);
        }

        // Método auxiliar para realizar una rotación a la derecha
        private Nodo<T> RightRotate(Nodo<T> y)
        {
            Nodo<T> x = y.Left;
            Nodo<T> T2 = x.Right;

            x.Right = y;
            y.Left = T2;

            y.Height = Math.Max(GetHeight(y.Left), GetHeight(y.Right)) + 1;
            x.Height = Math.Max(GetHeight(x.Left), GetHeight(x.Right)) + 1;

            return x;
        }

        // Método auxiliar para realizar una rotación a la izquierda
        private Nodo<T> LeftRotate(Nodo<T> x)
        {
            Nodo<T> y = x.Right;
            Nodo<T> T2 = y.Left;

            y.Left = x;
            x.Right = T2;

            x.Height = Math.Max(GetHeight(x.Left), GetHeight(x.Right)) + 1;
            y.Height = Math.Max(GetHeight(y.Left), GetHeight(y.Right)) + 1;

            return y;
        }

        // Método auxiliar para obtener el nodo con el valor mínimo en un subárbol
        private Nodo<T> GetMinValueNode(Nodo<T> node)
        {
            Nodo<T> current = node;

            while (current.Left != null)
                current = current.Left;

            return current;
        }

        // Método para mostrar el árbol completo por consola
        public void ConsoleDisplay()
        {
            Console.WriteLine("AVL Tree:");
            Console.WriteLine("------------------");
            ConsoleDisplay(root, 0);
            Console.WriteLine("------------------");
        }

        // Método auxiliar recursivo para mostrar el árbol por consola
        private void ConsoleDisplay(Nodo<T> node, int depth)
        {
            if (node == null)
                return;

            ConsoleDisplay(node.Right, depth + 1);
            Console.WriteLine($"{new string(' ', depth * 3)}{node.Value}");
            ConsoleDisplay(node.Left, depth + 1);
        }
    }
}
