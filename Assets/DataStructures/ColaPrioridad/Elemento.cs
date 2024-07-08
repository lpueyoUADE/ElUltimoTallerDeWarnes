using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EstructurasDeDatos.ColaPrioridad
{
    class Elemento<T> where T : IComparable<T>
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

            Elemento<T> other = (Elemento<T>)obj;

            if (this.prioridad > other.prioridad)
                return 1;
            else if (this.prioridad < other.prioridad)
                return -1;

            return 0;
        }
    }
}
