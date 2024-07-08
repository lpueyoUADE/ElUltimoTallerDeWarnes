using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EstructurasDeDatos.Cola
{
    internal class ColaEstatica<T> : ICola<T>
    {
        private T[]? Cola;
        private int indice;
        public void InicializarCola(int size)
        {
            Cola = new T[size];
            indice = 0;
        }

        public bool ColaVacia()
        {
            return indice == 0;
        }

        public void Encolar(T elemento)
        {
            if (indice == Cola.Length - 1)
            {
                throw new Exception("La cola está completa");
            }

            // Desplazar a la derecha
            for (int i = indice; i > 0; i--)
            {
                Cola[i] = Cola[i - 1];
            }

            Cola[0] = elemento;
            indice++;
        }
        public T DesEncolar()
        {
            if (indice == 0)
            {
                throw new Exception("La cola está vacía");
            }

            T retorno = Cola[indice - 1];
            indice--;

            return retorno;
        }

        public T Primero()
        {
            if (indice == 0)
                throw new Exception("La cola está vacía");

            return Cola[indice - 1];
        }
    }
}
