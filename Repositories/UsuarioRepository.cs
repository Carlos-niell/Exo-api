using Exo.WebApi.Contexts;
using Exo.WebApi.Exceptions;
using Exo.WebApi.Interfaces;
using Exo.WebApi.Models;
using System.Collections.Generic;
using System.Linq;

namespace Exo.WebApi.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly ExoContext _context;

        public UsuarioRepository(ExoContext context)
        {
            _context = context;
        }

        public Usuario? Login(string email, string senha)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(senha))
            {
                return null;
            }

            string emailNormalizado = email.Trim().ToLower();
            return _context.Usuarios.FirstOrDefault(u => u.Email.ToLower() == emailNormalizado && u.Senha == senha);
        }

        public List<Usuario> Listar()
        {
            return _context.Usuarios.ToList();
        }

        public Usuario? BuscarPorId(int id)
        {
            if (id <= 0)
            {
                throw new DadosInvalidosException("O identificador do usuário deve ser um número inteiro positivo maior que zero.");
            }

            Usuario? usuario = _context.Usuarios.Find(id);

            if (usuario == null)
            {
                throw new UsuarioNaoEncontradoException($"Usuário com ID {id} não foi encontrado.");
            }

            return usuario;
        }

        public void Cadastrar(Usuario usuario)
        {
            if (usuario == null)
            {
                throw new DadosInvalidosException("Os dados do usuário não foram informados.");
            }

            if (string.IsNullOrWhiteSpace(usuario.Email) || string.IsNullOrWhiteSpace(usuario.Senha))
            {
                throw new DadosInvalidosException("O e-mail e a senha do usuário são obrigatórios.");
            }

            if (string.IsNullOrWhiteSpace(usuario.Perfil))
            {
                usuario.Perfil = "Admin";
            }

            bool duplicado = _context.Usuarios.Any(u => u.Email.ToLower() == usuario.Email.Trim().ToLower());
            if (duplicado)
            {
                throw new UsuarioDuplicadoException($"Já existe um usuário cadastrado com o e-mail '{usuario.Email}'.");
            }

            _context.Usuarios.Add(usuario);
            _context.SaveChanges();
        }

        public void Atualizar(int id, Usuario usuario)
        {
            if (id <= 0)
            {
                throw new DadosInvalidosException("O ID informado para atualização deve ser maior que zero.");
            }

            if (usuario == null)
            {
                throw new DadosInvalidosException("Os dados do usuário para atualização não foram fornecidos.");
            }

            Usuario? usuarioBuscado = _context.Usuarios.Find(id);

            if (usuarioBuscado == null)
            {
                throw new UsuarioNaoEncontradoException($"Usuário com ID {id} não foi encontrado para atualização.");
            }

            bool emailDuplicado = _context.Usuarios.Any(u => u.Id != id && u.Email.ToLower() == usuario.Email.Trim().ToLower());
            if (emailDuplicado)
            {
                throw new UsuarioDuplicadoException($"Já existe outro usuário cadastrado com o e-mail '{usuario.Email}'.");
            }

            usuarioBuscado.Email = usuario.Email;
            usuarioBuscado.Senha = usuario.Senha;

            if (!string.IsNullOrWhiteSpace(usuario.Perfil))
            {
                usuarioBuscado.Perfil = usuario.Perfil;
            }

            _context.Usuarios.Update(usuarioBuscado);
            _context.SaveChanges();
        }

        public void Deletar(int id)
        {
            if (id <= 0)
            {
                throw new DadosInvalidosException("O ID informado para exclusão deve ser maior que zero.");
            }

            Usuario? usuarioBuscado = _context.Usuarios.Find(id);

            if (usuarioBuscado == null)
            {
                throw new UsuarioNaoEncontradoException($"Usuário com ID {id} não foi encontrado para exclusão.");
            }

            _context.Usuarios.Remove(usuarioBuscado);
            _context.SaveChanges();
        }
    }
}

