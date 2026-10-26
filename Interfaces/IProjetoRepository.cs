using Exo.WebApi.Models;
using System.Collections.Generic;

namespace Exo.WebApi.Interfaces
{
    public interface IProjetoRepository
    {
        List<Projeto> Listar();
        Projeto? BuscarPorId(int id);
        void Cadastrar(Projeto projeto);
        void Atualizar(int id, Projeto projeto);
        void Deletar(int id);
    }
}
