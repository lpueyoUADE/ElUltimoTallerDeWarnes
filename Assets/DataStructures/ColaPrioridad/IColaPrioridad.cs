using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EstructurasDeDatos.ColaPrioridad
{
    public interface IColaPrioridad<T>
    {
        void InicializarColaPrioridad(int size);
        void Encolar(T elemento, int prioridad);
        T DesEncolar();
        T Primero();
        int Prioridad();
        bool ColaVacia();

        int Cantidad();
    }
}
