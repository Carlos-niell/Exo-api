using Exo.WebApi.Models;
using System.Collections.Generic;

namespace Exo.WebApi.Interfaces
{
    public interface IUsuarioRepository
    {
        Usuario? Login(string email, string senha);
        List<Usuario> Listar();
        Usuario? BuscarPorId(int id);
        void Cadastrar(Usuario usuario);
        void Atualizar(int id, Usuario usuario);
        void Deletar(int id);
    }
}

