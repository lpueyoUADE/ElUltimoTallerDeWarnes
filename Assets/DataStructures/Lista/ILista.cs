using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EstructurasDeDatos.Lista
{
    public interface ILista<T>
    {
        int Cantidad();
        void Agregar(T item);
        void Remover();
        void RemoverEn(int indice);

        void Ordenar(bool desc=true);

        bool Contiene(T item);
        int IndexOf(T item);
        // Acceder y modificar
        public T this[int i] { get; set; }
    }
}
