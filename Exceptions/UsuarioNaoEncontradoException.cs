using System;

namespace Exo.WebApi.Exceptions
{
    /// <summary>
    /// Exceção lançada quando um usuário com determinado identificador não é encontrado no sistema.
    /// </summary>
    public class UsuarioNaoEncontradoException : Exception
    {
        public UsuarioNaoEncontradoException(string mensagem) : base(mensagem)
        {
        }

        public UsuarioNaoEncontradoException(string mensagem, Exception innerException) : base(mensagem, innerException)
        {
        }
    }
}

