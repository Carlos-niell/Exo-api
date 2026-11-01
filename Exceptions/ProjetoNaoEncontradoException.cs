namespace Exo.WebApi.Exceptions
{
    /// <summary>
    /// Exceção lançada quando um projeto com determinado identificador não é encontrado no sistema.
    /// </summary>
    public class ProjetoNaoEncontradoException : Exception
    {
        public ProjetoNaoEncontradoException(string mensagem) : base(mensagem)
        {
        }

        public ProjetoNaoEncontradoException(string mensagem, Exception innerException) : base(mensagem, innerException)
        {
        }
    }
}
