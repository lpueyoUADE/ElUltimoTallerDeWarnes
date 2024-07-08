using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EstructurasDeDatos.Conjunto
{
    public interface IConjunto<T>
    {
        bool Vacio();
        void Agregar(T elemento);
        void Quitar(T elemento);
        bool Pertenece(T elemento);
        T? Elegir();
        int Cantidad();
        List<T> ToList();
    }
}
