using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EstructurasDeDatos.AVL
{    public class Nodo<T> where T : IComparable<T>
    {
        public T Value { get; set; }
        public Nodo<T>? Left { get; set; }
        public Nodo<T>? Right { get; set; }
        public int Height { get; set; }

        public Nodo(T value)
        {
            Value = value;
            Left = null;
            Right = null;
            Height = 1;
        }
    }
}
