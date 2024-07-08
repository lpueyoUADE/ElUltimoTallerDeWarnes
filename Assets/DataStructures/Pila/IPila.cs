namespace EstructurasDeDatos.Pila
{
    public interface IPila<T> {
        void InicializarPila(int size);
        void Apilar(T elemento);
        T Desapilar();
        T? Tope();
        bool IsPilaVacia();
    }
}
