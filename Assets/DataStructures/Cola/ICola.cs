namespace EstructurasDeDatos.Cola
{
    public interface ICola<T>
    {
        void InicializarCola(int size);
        void Encolar(T elemento);
        T DesEncolar();
        T? Primero();
        bool ColaVacia();
    }
}
