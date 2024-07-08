using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EstructurasDeDatos.Pila
{
    internal class PilaEstatica<T> : IPila<T>
    {
        private T[] _elementos;
        private int indice;

        public PilaEstatica(int size)
        {
            InicializarPila(size);
        }

        public void InicializarPila(int size)
        {
            _elementos = new T[size];
            indice = -1;
        }

        public void Apilar(T elemento)
        {
            if(indice == _elementos.Length - 1)
            {
                throw new Exception("Pila Completa");
            }
            indice++;
            _elementos[indice] = elemento;
        }

        public T Desapilar()
        {
            if (indice == -1)
            {
                throw new Exception("Pila Vacía");
            }

            T retorno = _elementos[indice];
            indice--;
            return retorno;
        }

        public bool IsPilaVacia()
        {
            return indice == -1;
        }

        public T? Tope()
        {
            if (IsPilaVacia())
            {
                return default(T);
            }

            return _elementos[indice];
        }
    }
}
