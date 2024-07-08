using EstructurasDeDatos.Lista;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace EstructurasDeDatos.Cola
{
    public class ColaDinamica<T> : ICola<T> where T : IComparable
    {
        private ListaSimplementeEnlazada<T> elementos;

        public ColaDinamica()
        {
            elementos = new();
        }

        public bool ColaVacia()
        {
            return elementos.Cantidad() == 0;
        }

        public T DesEncolar()
        {
            if (elementos.Cantidad() == 0)
            {
                throw new InvalidOperationException("La cola está vacía.");
            }

            T elementoDesencolado = elementos[0];
            elementos.RemoverEn(0);
            return elementoDesencolado;
        }

        public void Encolar(T elemento)
        {
            elementos.Agregar(elemento);
        }

        public void InicializarCola(int size)
        {
            // not implemented.
        }

        public T? Primero()
        {
            if (ColaVacia())
                return default;

            return elementos[0];
        }
    }
}
