using Exo.WebApi.Exceptions;
using Exo.WebApi.Interfaces;
using Exo.WebApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Exo.WebApi.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class ProjetosController : ControllerBase
    {
        private readonly IProjetoRepository _projetoRepository;

        public ProjetosController(IProjetoRepository projetoRepository)
        {
            _projetoRepository = projetoRepository;
        }

        /// <summary>
        /// Lista todos os projetos cadastrados.
        /// </summary>
        /// <returns>Lista de projetos com mensagem padronizada de sucesso.</returns>
        /// <response code="200">Retorna a lista de projetos cadastrados.</response>
        /// <response code="500">Erro interno inesperado no servidor.</response>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<List<Projeto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
        public IActionResult Listar()
        {
            try
            {
                var projetos = _projetoRepository.Listar();

                // Mensagem de Sucesso 1: Resposta padronizada para listagem
                return Ok(new ApiResponse<List<Projeto>>(
                    mensagem: "Projetos listados com sucesso.",
                    dados: projetos,
                    total: projetos.Count
                ));
            }
            catch (Exception ex)
            {
                // Erro 500: Erro interno no servidor
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiErrorResponse
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Erro = "Erro Interno do Servidor",
                    Mensagem = "Ocorreu um erro interno inesperado ao listar os projetos.",
                    Detalhes = new[] { ex.Message }
                });
            }
        }

        /// <summary>
        /// Busca um projeto específico pelo seu identificador único (ID).
        /// </summary>
        /// <param name="id">ID do projeto.</param>
        /// <returns>Dados do projeto encontrado.</returns>
        /// <response code="200">Projeto localizado com sucesso.</response>
        /// <response code="400">Identificador inválido (menor ou igual a zero).</response>
        /// <response code="404">Projeto não encontrado para o ID informado.</response>
        /// <response code="500">Erro interno inesperado no servidor.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<Projeto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
        public IActionResult BuscarPorId(int id)
        {
            try
            {
                var projeto = _projetoRepository.BuscarPorId(id);

                // Mensagem de Sucesso 2: Resposta padronizada para busca por ID
                return Ok(new ApiResponse<Projeto>(
                    mensagem: "Projeto localizado com sucesso.",
                    dados: projeto
                ));
            }
            // Captura e Tratamento da Exceção de Dados Inválidos (HTTP 400)
            catch (DadosInvalidosException ex)
            {
                return BadRequest(new ApiErrorResponse
                {
                    Status = StatusCodes.Status400BadRequest,
                    Erro = "Requisição Inválida",
                    Mensagem = ex.Message
                });
            }
            // Captura e Tratamento da Exceção de Projeto Não Encontrado (HTTP 404)
            catch (ProjetoNaoEncontradoException ex)
            {
                return NotFound(new ApiErrorResponse
                {
                    Status = StatusCodes.Status404NotFound,
                    Erro = "Não Encontrado",
                    Mensagem = ex.Message
                });
            }
            // Captura de erro inesperado (HTTP 500)
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiErrorResponse
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Erro = "Erro Interno do Servidor",
                    Mensagem = "Ocorreu um erro interno inesperado ao buscar o projeto.",
                    Detalhes = new[] { ex.Message }
                });
            }
        }

        /// <summary>
        /// Cadastra um novo projeto no sistema.
        /// </summary>
        /// <param name="projeto">Dados do projeto a ser cadastrado.</param>
        /// <returns>Projeto criado com o status 201 Created.</returns>
        /// <response code="201">Projeto cadastrado com sucesso.</response>
        /// <response code="400">Dados inválidos fornecidos no corpo da requisição.</response>
        /// <response code="409">Conflito: já existe um projeto cadastrado com o mesmo nome.</response>
        /// <response code="500">Erro interno inesperado no servidor.</response>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<Projeto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
        public IActionResult Cadastrar([FromBody] Projeto projeto)
        {
            // Erro 400: Validação de modelo (ModelState)
            if (!ModelState.IsValid)
            {
                var erros = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return BadRequest(new ApiErrorResponse
                {
                    Status = StatusCodes.Status400BadRequest,
                    Erro = "Requisição Inválida",
                    Mensagem = "Os dados fornecidos para o projeto são inválidos.",
                    Detalhes = erros
                });
            }

            try
            {
                _projetoRepository.Cadastrar(projeto);

                // Mensagem de Sucesso 3: Resposta padronizada para criação (201 Created)
                var resposta = new ApiResponse<Projeto>(
                    mensagem: "Projeto cadastrado com sucesso!",
                    dados: projeto
                );

                return CreatedAtAction(nameof(BuscarPorId), new { id = projeto.Id }, resposta);
            }
            // Captura e Tratamento da Exceção de Dados Inválidos (HTTP 400)
            catch (DadosInvalidosException ex)
            {
                return BadRequest(new ApiErrorResponse
                {
                    Status = StatusCodes.Status400BadRequest,
                    Erro = "Requisição Inválida",
                    Mensagem = ex.Message
                });
            }
            // Captura e Tratamento da Exceção de Projeto Duplicado (HTTP 409)
            catch (ProjetoDuplicadoException ex)
            {
                return Conflict(new ApiErrorResponse
                {
                    Status = StatusCodes.Status409Conflict,
                    Erro = "Conflito de Dados",
                    Mensagem = ex.Message
                });
            }
            // Captura de erro inesperado (HTTP 500)
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiErrorResponse
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Erro = "Erro Interno do Servidor",
                    Mensagem = "Ocorreu um erro interno inesperado ao cadastrar o projeto.",
                    Detalhes = new[] { ex.Message }
                });
            }
        }

        /// <summary>
        /// Atualiza os dados de um projeto existente.
        /// </summary>
        /// <param name="id">ID do projeto a ser atualizado.</param>
        /// <param name="projeto">Novos dados do projeto.</param>
        /// <returns>Projeto atualizado com status 200 OK.</returns>
        /// <response code="200">Projeto atualizado com sucesso.</response>
        /// <response code="400">Identificador inválido ou inconsistência nos dados.</response>
        /// <response code="404">Projeto não encontrado para atualização.</response>
        /// <response code="409">Conflito: nome do projeto já utilizado por outro registro.</response>
        /// <response code="500">Erro interno inesperado no servidor.</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<Projeto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
        public IActionResult Atualizar(int id, [FromBody] Projeto projeto)
        {
            // Erro 400: Validação de ID na rota
            if (id <= 0)
            {
                return BadRequest(new ApiErrorResponse
                {
                    Status = StatusCodes.Status400BadRequest,
                    Erro = "Requisição Inválida",
                    Mensagem = "O identificador informado na URL deve ser maior que zero."
                });
            }

            // Erro 400: Inconsistência entre ID da URL e do corpo
            if (projeto != null && projeto.Id != 0 && projeto.Id != id)
            {
                return BadRequest(new ApiErrorResponse
                {
                    Status = StatusCodes.Status400BadRequest,
                    Erro = "Requisição Inválida",
                    Mensagem = "O identificador da URL não corresponde ao ID enviado no corpo da requisição."
                });
            }

            // Erro 400: Validação de campos obrigatórios
            if (!ModelState.IsValid)
            {
                var erros = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return BadRequest(new ApiErrorResponse
                {
                    Status = StatusCodes.Status400BadRequest,
                    Erro = "Requisição Inválida",
                    Mensagem = "Os dados fornecidos para atualização são inválidos.",
                    Detalhes = erros
                });
            }

            try
            {
                _projetoRepository.Atualizar(id, projeto!);

                // Mensagem de Sucesso 4: Resposta padronizada para atualização (200 OK)
                return Ok(new ApiResponse<Projeto>(
                    mensagem: "Projeto atualizado com sucesso!",
                    dados: projeto
                ));
            }
            // Captura e Tratamento da Exceção de Dados Inválidos (HTTP 400)
            catch (DadosInvalidosException ex)
            {
                return BadRequest(new ApiErrorResponse
                {
                    Status = StatusCodes.Status400BadRequest,
                    Erro = "Requisição Inválida",
                    Mensagem = ex.Message
                });
            }
            // Captura e Tratamento da Exceção de Projeto Não Encontrado (HTTP 404)
            catch (ProjetoNaoEncontradoException ex)
            {
                return NotFound(new ApiErrorResponse
                {
                    Status = StatusCodes.Status404NotFound,
                    Erro = "Não Encontrado",
                    Mensagem = ex.Message
                });
            }
            // Captura e Tratamento da Exceção de Projeto Duplicado (HTTP 409)
            catch (ProjetoDuplicadoException ex)
            {
                return Conflict(new ApiErrorResponse
                {
                    Status = StatusCodes.Status409Conflict,
                    Erro = "Conflito de Dados",
                    Mensagem = ex.Message
                });
            }
            // Captura de erro inesperado (HTTP 500)
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiErrorResponse
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Erro = "Erro Interno do Servidor",
                    Mensagem = "Ocorreu um erro interno inesperado ao atualizar o projeto.",
                    Detalhes = new[] { ex.Message }
                });
            }
        }

        /// <summary>
        /// Remove um projeto do sistema pelo seu identificador único (ID).
        /// Restrito exclusivamente a usuários autenticados e autorizados (perfil Admin).
        /// </summary>
        /// <param name="id">ID do projeto a ser deletado.</param>
        /// <returns>Mensagem de sucesso na remoção.</returns>
        /// <response code="200">Projeto removido com sucesso.</response>
        /// <response code="400">Identificador inválido (menor ou igual a zero).</response>
        /// <response code="401">Usuário não autenticado (Token JWT ausente, inválido ou expirado).</response>
        /// <response code="403">Usuário autenticado, porém sem permissão para excluir registros.</response>
        /// <response code="404">Projeto não encontrado para exclusão.</response>
        /// <response code="500">Erro interno inesperado no servidor.</response>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
        public IActionResult Deletar(int id)
        {
            // Erro 400: Validação de ID na rota
            if (id <= 0)
            {
                return BadRequest(new ApiErrorResponse
                {
                    Status = StatusCodes.Status400BadRequest,
                    Erro = "Requisição Inválida",
                    Mensagem = "O identificador informado para exclusão deve ser maior que zero."
                });
            }

            try
            {
                _projetoRepository.Deletar(id);

                // Mensagem de Sucesso 5: Resposta padronizada para exclusão (200 OK)
                return Ok(new ApiResponse<object>(
                    mensagem: "Projeto removido com sucesso!",
                    dados: new { idRemovido = id }
                ));
            }
            // Captura e Tratamento da Exceção de Dados Inválidos (HTTP 400)
            catch (DadosInvalidosException ex)
            {
                return BadRequest(new ApiErrorResponse
                {
                    Status = StatusCodes.Status400BadRequest,
                    Erro = "Requisição Inválida",
                    Mensagem = ex.Message
                });
            }
            // Captura e Tratamento da Exceção de Projeto Não Encontrado (HTTP 404)
            catch (ProjetoNaoEncontradoException ex)
            {
                return NotFound(new ApiErrorResponse
                {
                    Status = StatusCodes.Status404NotFound,
                    Erro = "Não Encontrado",
                    Mensagem = ex.Message
                });
            }
            // Captura de erro inesperado (HTTP 500)
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiErrorResponse
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Erro = "Erro Interno do Servidor",
                    Mensagem = "Ocorreu um erro interno inesperado ao excluir o projeto.",
                    Detalhes = new[] { ex.Message }
                });
            }
        }
    }
}
