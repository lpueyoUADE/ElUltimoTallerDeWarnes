using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EstructurasDeDatos.Lista
{
    public class ListaSimplementeEnlazada<T> : ILista<T> where T : IComparable
    {
        private Nodo<T>? cabeza;
        private int cantidad;

        public ListaSimplementeEnlazada()
        {
            cantidad = 0;
        }

        public T this[int indice]
        {
            get
            {
                if (indice < 0 || indice >= Cantidad())
                {
                    throw new ArgumentOutOfRangeException("El índice está fuera del rango de la lista.");
                }

                Nodo<T> actual = cabeza;
                for (int i = 0; i < indice; i++)
                {
                    actual = actual.Siguiente;
                }

                return actual.Valor;
            }
            set
            {
                if (indice < 0 || indice >= Cantidad())
                {
                    throw new ArgumentOutOfRangeException("El índice está fuera del rango de la lista.");
                }

                Nodo<T> actual = cabeza;
                for (int i = 0; i < indice; i++)
                {
                    actual = actual.Siguiente;
                }

                actual.Valor = value;
            }
        }

        public void Agregar(T item)
        {
            Nodo<T> nuevoNodo = new(item);

            if (cabeza == null)
            {
                cabeza = nuevoNodo;
            }
            else
            {
                Nodo<T> actual = cabeza;
                while (actual.Siguiente != null)
                {
                    actual = actual.Siguiente;
                }
                actual.Siguiente = nuevoNodo;
            }

            cantidad++;
        }

        public void AgregarEnOrden(T item)
        {
            Nodo<T> nuevoNodo = new(item);

            if (cabeza == null || item.CompareTo(cabeza.Valor) < 0)
            {
                nuevoNodo.Siguiente = cabeza;
                cabeza = nuevoNodo;
                cantidad++;
                return;
            }

            Nodo<T> actual = cabeza;
            while (actual.Siguiente != null && item.CompareTo(actual.Siguiente.Valor) >= 0)
            {
                actual = actual.Siguiente;
            }

            nuevoNodo.Siguiente = actual.Siguiente;
            actual.Siguiente = nuevoNodo;
            cantidad++;
        }

        public int Cantidad()
        {
            return cantidad;
        }

        public List<T> ToList()
        {
            List<T> listaConvertida = new();
            Nodo<T>? actual = cabeza;
            while (actual != null)
            {
                listaConvertida.Add(actual.Valor);
                actual = actual.Siguiente;
            }
            return listaConvertida;
        }

        private void ToLSE(List<T> lista)
        {
            cabeza = null;

            foreach (T elemento in lista)
            {
                this.Agregar(elemento);
            }
        }

        private Nodo<T> GetTail(Nodo<T> node)
        {
            while (node != null && node.Siguiente != null)
            {
                node = node.Siguiente;
            }
            return node;
        }

        private Nodo<T> Partition(Nodo<T> head, Nodo<T> end, out Nodo<T> newHead, out Nodo<T> newEnd, bool desc)
        {
            Nodo<T> pivot = end;
            Nodo<T> prev = null, cur = head, tail = pivot;

            newHead = null;
            newEnd = null;

            while (cur != pivot)
            {
                if (!desc &&(cur.Valor.CompareTo(pivot.Valor) < 0) || desc && (cur.Valor.CompareTo(pivot.Valor) > 0))
                {
                    if (newHead == null)
                    {
                        newHead = cur;
                    }
                    prev = cur;
                    cur = cur.Siguiente;
                }
                else
                {
                    if (prev != null)
                    {
                        prev.Siguiente = cur.Siguiente;
                    }
                    Nodo<T> tmp = cur.Siguiente;
                    cur.Siguiente = null;
                    tail.Siguiente = cur;
                    tail = cur;
                    cur = tmp;
                }
            }

            if (newHead == null)
            {
                newHead = pivot;
            }

            newEnd = tail;

            return pivot;
        }

        private Nodo<T> QuickSortRecur(Nodo<T> head, Nodo<T> end, bool desc)
        {
            if (head == null || head == end)
            {
                return head;
            }

            Nodo<T> newHead = null, newEnd = null;
            Nodo<T> pivot = Partition(head, end, out newHead, out newEnd, desc);

            if (newHead != pivot)
            {
                Nodo<T> tmp = newHead;
                while (tmp.Siguiente != pivot)
                {
                    tmp = tmp.Siguiente;
                }
                tmp.Siguiente = null;

                newHead = QuickSortRecur(newHead, tmp, desc);

                tmp = GetTail(newHead);
                tmp.Siguiente = pivot;
            }

            pivot.Siguiente = QuickSortRecur(pivot.Siguiente, newEnd, desc);

            return newHead;
        }

        private void QuickSort(bool desc)
        {
            cabeza = QuickSortRecur(cabeza, GetTail(cabeza), desc);
        }

        public void Ordenar(bool desc = true)
        {
            /*
             * List<T> lista = ToList();
            
            lista.Sort(); // TODO Eventualmente cambiar esta cochinada por un QuickSort caserito.

            if (desc)
            {
                lista.Reverse();
            }

            ToLSE(lista);
            */

            QuickSort(desc);
        }

        public void Remover()
        {
            RemoverEn(Cantidad() - 1);
        }

        public void RemoverEn(int indice)
        {
            if (indice < 0 || indice >= Cantidad())
                throw new ArgumentOutOfRangeException("El índice está fuera del rango de la lista.");


            Nodo<T>? anterior = null;
            Nodo<T>? actual = cabeza;

            if (indice == 0)
            {
                cabeza = cabeza.Siguiente;
                cantidad--;
                return;
            }

            for (int i = 0; i < indice; i++)
            {
                anterior = actual;
                actual = actual.Siguiente;
            }

            anterior.Siguiente = actual.Siguiente;

            cantidad--;
        }

        public bool Contiene(T item)
        {
            return IndexOf(item) != -1;
        }

        public int IndexOf(T item)
        {
            if (cabeza == null)
                return -1;

            Nodo<T>? actual = cabeza;
            int indice = 0;

            while (actual != null)
            {
                if (item.CompareTo(actual.Valor) == 0)
                    return indice;

                actual = actual.Siguiente;
                indice++;
            }

            return -1;
        }
    }
}
