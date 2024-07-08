using EstructurasDeDatos.Lista;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EstructurasDeDatos.ColaPrioridad
{
    public class ColaPrioridadDinamica<T> : IColaPrioridad<T>
    {
        class Elemento : IComparable
        {
            public T valor;
            public int prioridad;

            public Elemento(T valor, int prioridad)
            {
                this.valor = valor;
                this.prioridad = prioridad;
            }

            public int CompareTo(object obj)
            {
                if (this.GetType() != obj.GetType())
                {
                    throw (new ArgumentException("Elementos no comparables"));
                }

                Elemento other = (Elemento) obj;

                if(this.prioridad > other.prioridad)
                    return 1;
                else if (this.prioridad < other.prioridad)
                    return -1;
                
                return 0;
            }
        }

        private ListaSimplementeEnlazada<Elemento> elementos;

        public ColaPrioridadDinamica()
        {
            elementos = new();
        }

        public bool ColaVacia()
        {
            return elementos.Cantidad() == 0;
        }

        public T DesEncolar()
        {
            Elemento removido = elementos[elementos.Cantidad() - 1];
            elementos.Remover();

            return removido.valor;
        }

        public void Encolar(T elemento, int prioridad)
        {
            Elemento nuevo = new Elemento(elemento, prioridad);
            elementos.AgregarEnOrden(nuevo);
        }

        public void InicializarColaPrioridad(int size)
        {
            // Not implemented.
        }

        public T Primero()
        {
            return elementos[elementos.Cantidad() - 1].valor;
        }

        public int Prioridad()
        {
            return elementos[elementos.Cantidad() - 1].prioridad;
        }

        public int Cantidad()
        {
            return elementos.Cantidad();
        }
    }
}
