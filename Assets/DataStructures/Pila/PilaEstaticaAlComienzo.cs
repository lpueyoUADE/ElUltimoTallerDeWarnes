using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EstructurasDeDatos.Pila
{
    internal class PilaEstaticaAlComienzo<T>: IPila<T>
    {
        public T[] _elementos;
        private int indice;

        public PilaEstaticaAlComienzo(int size)
        {
            InicializarPila(size);
        }

        public void InicializarPila(int size)
        {
            _elementos = new T[size];
            indice = -1;
        }

        private void _DesplazarDerecha()
        {
            for (int i = indice - 1; i >= 0; i--)
            {
                _elementos[i+1] = _elementos[i];
            }
        }

        public void Apilar(T elemento)
        {
            if (indice == _elementos.Length - 1)
            {
                throw new Exception("Pila Completa");
            }
            indice++;

            _DesplazarDerecha();

            _elementos[0] = elemento;
        }

        private void _DesplazarIzquierda()
        {
            for (int i = 0; i < indice; i++)
            {
                _elementos[i] = _elementos[i + 1];
            }
        }
        public T Desapilar()
        {
            if (indice == -1)
            {
                throw new Exception("Pila Vacía");
            }

            T retorno = _elementos[0];
            _DesplazarIzquierda();
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
