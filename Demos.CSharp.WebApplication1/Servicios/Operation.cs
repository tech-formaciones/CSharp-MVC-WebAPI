namespace Demos.CSharp.WebApplication1.Servicios
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
