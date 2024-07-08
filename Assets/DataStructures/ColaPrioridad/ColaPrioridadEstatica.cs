using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EstructurasDeDatos.ColaPrioridad
{
    public class ColaPrioridadEstatica<T>:IColaPrioridad<T>
    {
        struct ElementoCola
        {
            public T elemento;
            public int prioridad;
        }
        
        private ElementoCola[]? ColaPrioridad;
        private int indice;
        public void InicializarColaPrioridad(int size)
        {
            ColaPrioridad = new ElementoCola[size];
            indice = 0;
        }

        public bool ColaVacia()
        {
            return indice == 0;
        }

        private void DesplazarDerechaDesde(int begin)
        {
            for (int i = indice; i > begin; i--)
            {
                ColaPrioridad[i] = ColaPrioridad[i - 1];
            }
        }
        public void Encolar(T elemento, int prioridad)
        {
            if (indice == ColaPrioridad.Length - 1)
            {
                throw new Exception("La cola está completa");
            }

            for (int i = 0; i <= indice; i++)
            {
                if (prioridad <= ColaPrioridad[i].prioridad )
                {
                    // Desplazar a la derecha Desde
                    DesplazarDerechaDesde(i);
                    break;
                }
            }

            ColaPrioridad[indice].elemento = elemento;
            ColaPrioridad[indice].prioridad = prioridad;
            indice++;
        }
        public T DesEncolar()
        {
            if (indice == 0)
            {
                throw new Exception("La cola está vacía");
            }

            T retorno = ColaPrioridad[indice - 1].elemento;
            indice--;

            return retorno;
        }

        public T Primero()
        {
            if (indice == 0)
                throw new Exception("La cola está vacía");

            return ColaPrioridad[indice - 1].elemento;
        }

        public int Prioridad()
        {
            if (indice == 0)
                throw new Exception("La cola está vacía");

            return ColaPrioridad[indice - 1].prioridad;
        }

        public int Cantidad()
        {
            throw new NotImplementedException();
        }
    }
}
