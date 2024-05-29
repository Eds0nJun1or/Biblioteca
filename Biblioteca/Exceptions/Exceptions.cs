using System.Runtime.Serialization;

namespace Biblioteca.Services.Exceptions
{
    public class EmprestimoNotFoundException : Exception
    {
        public EmprestimoNotFoundException(string message) : base(message)
        {
        }
    }

    public class LimiteEmprestimosExcedidoException : Exception
    {
        public LimiteEmprestimosExcedidoException(string message) : base(message)
        {
        }
    }

    public class ExemplarNaoDisponivelException : Exception
    {
        public ExemplarNaoDisponivelException(string message) : base(message)
        {
        }
    }

    public class UsuarioNotFoundException : Exception
    {
        public UsuarioNotFoundException(string message) : base(message)
        {
        }
    }

    public class EmprestimoJaDevolvidoException : Exception
    {
        public EmprestimoJaDevolvidoException(string message) : base(message)
        {
        }
    }

    [Serializable]
    internal class UsuarioSemPermissaoException : Exception
    {
        public UsuarioSemPermissaoException()
        {
        }

        public UsuarioSemPermissaoException(string? message) : base(message)
        {
        }

        public UsuarioSemPermissaoException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected UsuarioSemPermissaoException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
       
        }
    }
}