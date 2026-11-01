using System;
using System.Collections.Generic;

namespace Exo.WebApi.Models
{
    /// <summary>
    /// Resposta padronizada de sucesso para as operações da API.
    /// </summary>
    /// <typeparam name="T">Tipo do dado encapsulado na resposta.</typeparam>
    public class ApiResponse<T>
    {
        /// <summary>
        /// Indica se a operação foi bem-sucedida.
        /// </summary>
        public bool Sucesso { get; set; } = true;

        /// <summary>
        /// Mensagem textual descritiva da operação realizada.
        /// </summary>
        public string Mensagem { get; set; } = string.Empty;

        /// <summary>
        /// Dados retornados pela operação (quando houver).
        /// </summary>
        public T? Dados { get; set; }

        /// <summary>
        /// Quantidade de registros retornados (aplicável para listagens).
        /// </summary>
        public int? Total { get; set; }

        /// <summary>
        /// Carimbo de data/hora (UTC) em que a resposta foi processada.
        /// </summary>
        public DateTime DataHora { get; set; } = DateTime.UtcNow;

        public ApiResponse() { }

        public ApiResponse(string mensagem, T? dados = default, int? total = null)
        {
            Sucesso = true;
            Mensagem = mensagem;
            Dados = dados;
            Total = total;
            DataHora = DateTime.UtcNow;
        }
    }

    /// <summary>
    /// Resposta padronizada para cenários de erro da API.
    /// </summary>
    public class ApiErrorResponse
    {
        /// <summary>
        /// Indica que a operação falhou.
        /// </summary>
        public bool Sucesso { get; set; } = false;

        /// <summary>
        /// Código de status HTTP representativo do erro.
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// Categoria ou título resumido do erro.
        /// </summary>
        public string Erro { get; set; } = string.Empty;

        /// <summary>
        /// Mensagem explicativa do erro ocorrido.
        /// </summary>
        public string Mensagem { get; set; } = string.Empty;

        /// <summary>
        /// Lista opcional com detalhes ou validações específicas.
        /// </summary>
        public IEnumerable<string>? Detalhes { get; set; }

        /// <summary>
        /// Carimbo de data/hora (UTC) do erro.
        /// </summary>
        public DateTime DataHora { get; set; } = DateTime.UtcNow;
    }
}
