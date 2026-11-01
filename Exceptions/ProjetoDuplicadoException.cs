namespace Exo.WebApi.Exceptions
{
    /// <summary>
    /// Exceção lançada quando ocorre tentativa de cadastrar ou alterar um projeto com nome já existente no sistema.
    /// </summary>
    public class ProjetoDuplicadoException : Exception
    {
        public ProjetoDuplicadoException(string mensagem) : base(mensagem)
        {
        }

        public ProjetoDuplicadoException(string mensagem, Exception innerException) : base(mensagem, innerException)
        {
        }
    }
}
