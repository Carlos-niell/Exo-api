namespace Exo.WebApi.Exceptions
{
    /// <summary>
    /// Exceção lançada quando parâmetros ou dados de entrada são inconsistentes ou inválidos.
    /// </summary>
    public class DadosInvalidosException : Exception
    {
        public DadosInvalidosException(string mensagem) : base(mensagem)
        {
        }

        public DadosInvalidosException(string mensagem, Exception innerException) : base(mensagem, innerException)
        {
        }
    }
}
