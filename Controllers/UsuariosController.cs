using Exo.WebApi.Exceptions;
using Exo.WebApi.Interfaces;
using Exo.WebApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace Exo.WebApi.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly IUsuarioRepository _usuarioRepository;

        public UsuariosController(IUsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }

        /// <summary>
        /// Realiza a autenticação do usuário e gera um token JWT para acesso aos recursos protegidos.
        /// </summary>
        /// <param name="dadosLogin">Credenciais do usuário (e-mail e senha).</param>
        /// <returns>Token JWT assinado e informações do usuário autenticado.</returns>
        /// <response code="200">Autenticação realizada com sucesso. Retorna o token JWT.</response>
        /// <response code="400">E-mail ou senha não informados no corpo da requisição.</response>
        /// <response code="401">Credenciais inválidas (e-mail ou senha incorretos).</response>
        /// <response code="500">Erro interno inesperado no servidor.</response>
        [HttpPost("login")]
        [HttpPost("/api/login")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
        public IActionResult Login([FromBody] Usuario dadosLogin)
        {
            if (dadosLogin == null || string.IsNullOrWhiteSpace(dadosLogin.Email) || string.IsNullOrWhiteSpace(dadosLogin.Senha))
            {
                return BadRequest(new ApiErrorResponse
                {
                    Status = StatusCodes.Status400BadRequest,
                    Erro = "Requisição Inválida",
                    Mensagem = "O e-mail e a senha são obrigatórios para realizar a autenticação."
                });
            }

            try
            {
                Usuario? usuarioEncontrado = _usuarioRepository.Login(dadosLogin.Email, dadosLogin.Senha);

                if (usuarioEncontrado == null)
                {
                    return Unauthorized(new ApiErrorResponse
                    {
                        Status = StatusCodes.Status401Unauthorized,
                        Erro = "Não Autorizado",
                        Mensagem = "E-mail ou senha inválidos. Verifique suas credenciais."
                    });
                }

                string perfilUsuario = string.IsNullOrWhiteSpace(usuarioEncontrado.Perfil)
                    ? "Admin"
                    : usuarioEncontrado.Perfil;

                // Definição das Claims (identidade e permissões do usuário no token JWT)
                var claims = new[]
                {
                    new Claim(JwtRegisteredClaimNames.Email, usuarioEncontrado.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, usuarioEncontrado.Id.ToString()),
                    new Claim(ClaimTypes.NameIdentifier, usuarioEncontrado.Id.ToString()),
                    new Claim(ClaimTypes.Role, perfilUsuario),
                    new Claim("role", perfilUsuario)
                };

                // Chave secreta simétrica para assinatura do token
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("exoapi-chave-autenticacao-senai-2026-segura"));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var expiracao = DateTime.UtcNow.AddMinutes(30);

                // Geração do Token JWT
                var meuToken = new JwtSecurityToken(
                    issuer: "exoapi.webapi",
                    audience: "exoapi.webapi",
                    claims: claims,
                    expires: expiracao,
                    signingCredentials: creds
                );

                string tokenGerado = new JwtSecurityTokenHandler().WriteToken(meuToken);

                return Ok(new
                {
                    sucesso = true,
                    mensagem = "Autenticação realizada com sucesso!",
                    token = tokenGerado,
                    expiracao = expiracao,
                    dados = new
                    {
                        id = usuarioEncontrado.Id,
                        email = usuarioEncontrado.Email,
                        perfil = perfilUsuario,
                        token = tokenGerado
                    },
                    dataHora = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiErrorResponse
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Erro = "Erro Interno do Servidor",
                    Mensagem = "Ocorreu um erro interno inesperado ao processar o login.",
                    Detalhes = new[] { ex.Message }
                });
            }
        }

        /// <summary>
        /// Lista todos os usuários cadastrados no sistema.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<List<Usuario>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
        public IActionResult Listar()
        {
            try
            {
                var usuarios = _usuarioRepository.Listar();
                return Ok(new ApiResponse<List<Usuario>>(
                    mensagem: "Usuários listados com sucesso.",
                    dados: usuarios,
                    total: usuarios.Count
                ));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiErrorResponse
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Erro = "Erro Interno do Servidor",
                    Mensagem = "Ocorreu um erro interno inesperado ao listar os usuários.",
                    Detalhes = new[] { ex.Message }
                });
            }
        }

        /// <summary>
        /// Busca um usuário específico pelo seu identificador único (ID).
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<Usuario>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
        public IActionResult BuscarPorId(int id)
        {
            try
            {
                var usuario = _usuarioRepository.BuscarPorId(id);
                return Ok(new ApiResponse<Usuario>(
                    mensagem: "Usuário localizado com sucesso.",
                    dados: usuario
                ));
            }
            catch (DadosInvalidosException ex)
            {
                return BadRequest(new ApiErrorResponse
                {
                    Status = StatusCodes.Status400BadRequest,
                    Erro = "Requisição Inválida",
                    Mensagem = ex.Message
                });
            }
            catch (UsuarioNaoEncontradoException ex)
            {
                return NotFound(new ApiErrorResponse
                {
                    Status = StatusCodes.Status404NotFound,
                    Erro = "Não Encontrado",
                    Mensagem = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiErrorResponse
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Erro = "Erro Interno do Servidor",
                    Mensagem = "Ocorreu um erro interno inesperado ao buscar o usuário.",
                    Detalhes = new[] { ex.Message }
                });
            }
        }

        /// <summary>
        /// Cadastra um novo usuário no sistema.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<Usuario>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
        public IActionResult Cadastrar([FromBody] Usuario usuario)
        {
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
                    Mensagem = "Os dados fornecidos para o cadastro do usuário são inválidos.",
                    Detalhes = erros
                });
            }

            try
            {
                _usuarioRepository.Cadastrar(usuario);

                var resposta = new ApiResponse<Usuario>(
                    mensagem: "Usuário cadastrado com sucesso!",
                    dados: usuario
                );

                return CreatedAtAction(nameof(BuscarPorId), new { id = usuario.Id }, resposta);
            }
            catch (DadosInvalidosException ex)
            {
                return BadRequest(new ApiErrorResponse
                {
                    Status = StatusCodes.Status400BadRequest,
                    Erro = "Requisição Inválida",
                    Mensagem = ex.Message
                });
            }
            catch (UsuarioDuplicadoException ex)
            {
                return Conflict(new ApiErrorResponse
                {
                    Status = StatusCodes.Status409Conflict,
                    Erro = "Conflito de Dados",
                    Mensagem = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiErrorResponse
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Erro = "Erro Interno do Servidor",
                    Mensagem = "Ocorreu um erro interno inesperado ao cadastrar o usuário.",
                    Detalhes = new[] { ex.Message }
                });
            }
        }

        /// <summary>
        /// Atualiza os dados de um usuário existente (Requer Autenticação).
        /// </summary>
        [HttpPut("{id}")]
        [Authorize]
        [ProducesResponseType(typeof(ApiResponse<Usuario>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
        public IActionResult Atualizar(int id, [FromBody] Usuario usuario)
        {
            if (id <= 0)
            {
                return BadRequest(new ApiErrorResponse
                {
                    Status = StatusCodes.Status400BadRequest,
                    Erro = "Requisição Inválida",
                    Mensagem = "O identificador informado na URL deve ser maior que zero."
                });
            }

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
                    Mensagem = "Os dados fornecidos para atualização do usuário são inválidos.",
                    Detalhes = erros
                });
            }

            try
            {
                _usuarioRepository.Atualizar(id, usuario);
                usuario.Id = id;

                return Ok(new ApiResponse<Usuario>(
                    mensagem: "Usuário atualizado com sucesso!",
                    dados: usuario
                ));
            }
            catch (DadosInvalidosException ex)
            {
                return BadRequest(new ApiErrorResponse
                {
                    Status = StatusCodes.Status400BadRequest,
                    Erro = "Requisição Inválida",
                    Mensagem = ex.Message
                });
            }
            catch (UsuarioNaoEncontradoException ex)
            {
                return NotFound(new ApiErrorResponse
                {
                    Status = StatusCodes.Status404NotFound,
                    Erro = "Não Encontrado",
                    Mensagem = ex.Message
                });
            }
            catch (UsuarioDuplicadoException ex)
            {
                return Conflict(new ApiErrorResponse
                {
                    Status = StatusCodes.Status409Conflict,
                    Erro = "Conflito de Dados",
                    Mensagem = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiErrorResponse
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Erro = "Erro Interno do Servidor",
                    Mensagem = "Ocorreu um erro interno inesperado ao atualizar o usuário.",
                    Detalhes = new[] { ex.Message }
                });
            }
        }

        /// <summary>
        /// Remove um usuário do sistema pelo seu ID (Restrito a usuários autenticados e autorizados com perfil Admin).
        /// </summary>
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
                _usuarioRepository.Deletar(id);

                return Ok(new ApiResponse<object>(
                    mensagem: "Usuário removido com sucesso!",
                    dados: new { idRemovido = id }
                ));
            }
            catch (DadosInvalidosException ex)
            {
                return BadRequest(new ApiErrorResponse
                {
                    Status = StatusCodes.Status400BadRequest,
                    Erro = "Requisição Inválida",
                    Mensagem = ex.Message
                });
            }
            catch (UsuarioNaoEncontradoException ex)
            {
                return NotFound(new ApiErrorResponse
                {
                    Status = StatusCodes.Status404NotFound,
                    Erro = "Não Encontrado",
                    Mensagem = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiErrorResponse
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Erro = "Erro Interno do Servidor",
                    Mensagem = "Ocorreu um erro interno inesperado ao excluir o usuário.",
                    Detalhes = new[] { ex.Message }
                });
            }
        }
    }
}

