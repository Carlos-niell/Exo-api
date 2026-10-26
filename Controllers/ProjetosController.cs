using Exo.WebApi.Models;
using Exo.WebApi.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Exo.WebApi.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class ProjetosController : ControllerBase
    {
        private readonly ProjetoRepository _projetoRepository;

        public ProjetosController(ProjetoRepository projetoRepository)
        {
            _projetoRepository = projetoRepository;
        }

        /// <summary>
        /// Lista todos os projetos cadastrados.
        /// </summary>
        /// <returns>Lista de projetos.</returns>
        [HttpGet]
        public IActionResult Listar()
        {
            try
            {
                var projetos = _projetoRepository.Listar();
                return Ok(projetos);
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensagem = "Erro ao listar projetos.", erro = ex.Message });
            }
        }

        /// <summary>
        /// Busca um projeto pelo seu identificador único (ID).
        /// </summary>
        /// <param name="id">ID do projeto.</param>
        /// <returns>Dados do projeto encontrado.</returns>
        [HttpGet("{id}")]
        public IActionResult BuscarPorId(int id)
        {
            try
            {
                Projeto? projeto = _projetoRepository.BuscarPorId(id);

                if (projeto == null)
                {
                    return NotFound(new { mensagem = $"Projeto com ID {id} não foi encontrado." });
                }

                return Ok(projeto);
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensagem = "Erro ao buscar projeto.", erro = ex.Message });
            }
        }

        /// <summary>
        /// Cadastra um novo projeto no banco de dados.
        /// </summary>
        /// <param name="projeto">Objeto com os dados do projeto a ser cadastrado.</param>
        /// <returns>Status 201 Created com os dados do projeto criado.</returns>
        [HttpPost]
        public IActionResult Cadastrar([FromBody] Projeto projeto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                _projetoRepository.Cadastrar(projeto);
                return CreatedAtAction(nameof(BuscarPorId), new { id = projeto.Id }, projeto);
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensagem = "Erro ao cadastrar projeto.", erro = ex.Message });
            }
        }

        /// <summary>
        /// Atualiza os dados de um projeto existente.
        /// </summary>
        /// <param name="id">ID do projeto a ser atualizado.</param>
        /// <param name="projeto">Novos dados do projeto.</param>
        /// <returns>Status 204 NoContent indicando sucesso na atualização.</returns>
        [HttpPut("{id}")]
        public IActionResult Atualizar(int id, [FromBody] Projeto projeto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                Projeto? projetoExistente = _projetoRepository.BuscarPorId(id);

                if (projetoExistente == null)
                {
                    return NotFound(new { mensagem = $"Projeto com ID {id} não foi encontrado para atualização." });
                }

                _projetoRepository.Atualizar(id, projeto);
                return StatusCode(204);
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensagem = "Erro ao atualizar projeto.", erro = ex.Message });
            }
        }

        /// <summary>
        /// Deleta um projeto do banco de dados pelo ID.
        /// </summary>
        /// <param name="id">ID do projeto a ser deletado.</param>
        /// <returns>Status 204 NoContent indicando sucesso na remoção.</returns>
        [HttpDelete("{id}")]
        public IActionResult Deletar(int id)
        {
            try
            {
                Projeto? projetoExistente = _projetoRepository.BuscarPorId(id);

                if (projetoExistente == null)
                {
                    return NotFound(new { mensagem = $"Projeto com ID {id} não foi encontrado para exclusão." });
                }

                _projetoRepository.Deletar(id);
                return StatusCode(204);
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensagem = "Erro ao deletar projeto.", erro = ex.Message });
            }
        }
    }
}
