using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EstructurasDeDatos.AVL
{
    public interface IAVL<T> where T : IComparable<T>
    {
        void Insert(T value);
        void Delete(T value);
        bool Search(T value);
        List<T> InOrderTraversal();
        List<T> PreOrderTraversal();
        List<T> PostOrderTraversal();
        int Length { get; }
    }
}
