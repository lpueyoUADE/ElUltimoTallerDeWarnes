using EstructurasDeDatos.Lista;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EstructurasDeDatos.Pila
{
    public class PilaDinamica<T> : IPila<T> where T : IComparable
    {
        private ListaSimplementeEnlazada<T> elementos;
        
        public PilaDinamica()
        {
            elementos = new();
        }

        public void Apilar(T elemento)
        {
            if (elemento == null) return;

            elementos.Agregar(elemento);
        }

        public T Desapilar()
        {
            if (elementos.Cantidad() == 0)
            {
                throw new InvalidOperationException("La pila está vacía.");
            }

            T elementoDesapilado = elementos[elementos.Cantidad() - 1];
            elementos.Remover();
            return elementoDesapilado;
        }

        public void InicializarPila(int size)
        {
            // not implemented.
        }

        public bool IsPilaVacia()
        {
            return elementos.Cantidad() == 0;
        }

        public T? Tope()
        {
            if (IsPilaVacia())
                return default;

            return elementos[elementos.Cantidad() - 1];
        }
    }
}
