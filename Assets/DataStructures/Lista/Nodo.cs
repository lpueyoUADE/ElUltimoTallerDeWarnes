using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EstructurasDeDatos.Lista
{
    public class Nodo<T>
    {
        private T valor;
        private Nodo<T>? anterior;
        private Nodo<T>? siguiente;

        public T Valor { get => valor; set => valor = value; }
        public Nodo<T>? Anterior { get => anterior; set => anterior = value; }
        public Nodo<T>? Siguiente { get => siguiente; set => siguiente = value; }

        public Nodo(T valor, Nodo<T>? siguiente = null, Nodo<T>? anterior = null)
        {
            if (valor == null) throw new ArgumentNullException(nameof(valor));

            this.Valor = valor;
            this.Siguiente = siguiente;
            this.Anterior = anterior;
        }
    }
}
