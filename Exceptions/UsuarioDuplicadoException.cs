using System;

namespace Exo.WebApi.Exceptions
{
    /// <summary>
    /// Exceção lançada quando se tenta cadastrar ou atualizar um usuário com um e-mail já existente.
    /// </summary>
    public class UsuarioDuplicadoException : Exception
    {
        public UsuarioDuplicadoException(string mensagem) : base(mensagem)
        {
        }

        public UsuarioDuplicadoException(string mensagem, Exception innerException) : base(mensagem, innerException)
        {
        }
    }
}

