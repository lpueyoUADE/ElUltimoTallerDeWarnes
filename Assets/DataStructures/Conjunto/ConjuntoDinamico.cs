using EstructurasDeDatos.Lista;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EstructurasDeDatos.Conjunto
{
    public class ConjuntoDinamico<T> : IConjunto<T> where T : IComparable
    {
        private ListaSimplementeEnlazada<T> elementos;

        public ConjuntoDinamico()
        {
            elementos = new();
        }

        public void Agregar(T elemento)
        {
            if (elementos.Contiene(elemento))
                return;

            elementos.Agregar(elemento);
        }

        public int Cantidad()
        {
            return elementos.Cantidad();
        }

        public T? Elegir()
        {
            if (elementos.Cantidad() == 0)
                return default;

            return elementos[elementos.Cantidad() - 1];
        }

        public bool Pertenece(T elemento)
        {
            return elementos.Contiene(elemento);
        }

        public void Quitar(T elemento)
        {
            int indice = elementos.IndexOf(elemento);
            if (indice == -1)
                return;

            elementos.RemoverEn(indice);
        }

        public List<T> ToList()
        {
            return elementos.ToList();
        }

        public bool Vacio()
        {
            return elementos.Cantidad() == 0;
        }
    }
}
