/*
IOperation es la interfaz base que define un ID de operación y un contador.

IOperationSingleton, IOperationScoped y IOperationTransient son interfaces especializadas 
para diferentes ciclos de vida de los servicios en la DI de ASP.NET Core.

Operation implementa estas interfaces y tiene un comportamiento diferente dependiendo de su ciclo de vida:
 - Singleton: Una sola instancia durante toda la aplicación.
 - Scoped: Una nueva instancia por solicitud HTTP.
 - Transient: Una nueva instancia cada vez que se solicita el servicio.

Esto te permite controlar cómo se gestionan las instancias de tus servicios a lo largo del ciclo de vida de la aplicación.
*/

namespace Demos.CSharp.WebApi1.Services
{
    public interface IOperation
    {
        string OperationId { get; }
        int Cont { get; }
    }

    public interface IOperationSingleton : IOperation { }
    public interface IOperationScoped : IOperation { }
    public interface IOperationTransient : IOperation { }

    public class Operation : IOperationSingleton, IOperationScoped, IOperationTransient
    {
        private int cont = 0;
        public string OperationId { get; }
        public int Cont
        {
            get
            {
                cont++;

                return cont;
            }
        }

        public Operation()
        {
            OperationId = Guid.NewGuid().ToString()[^4..];
        }
    }
}
