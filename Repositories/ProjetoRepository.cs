using Exo.WebApi.Contexts;
using Exo.WebApi.Exceptions;
using Exo.WebApi.Interfaces;
using Exo.WebApi.Models;
using System.Collections.Generic;
using System.Linq;

namespace Exo.WebApi.Repositories
{
    public class ProjetoRepository : IProjetoRepository
    {
        private readonly ExoContext _context;

        public ProjetoRepository(ExoContext context)
        {
            _context = context;
        }

        public List<Projeto> Listar()
        {
            return _context.Projetos.ToList();
        }

        public Projeto? BuscarPorId(int id)
        {
            if (id <= 0)
            {
                throw new DadosInvalidosException("O identificador informado deve ser um número inteiro positivo maior que zero.");
            }

            Projeto? projeto = _context.Projetos.Find(id);

            if (projeto == null)
            {
                throw new ProjetoNaoEncontradoException($"Projeto com ID {id} não foi encontrado.");
            }

            return projeto;
        }

        public void Cadastrar(Projeto projeto)
        {
            if (projeto == null)
            {
                throw new DadosInvalidosException("Os dados do projeto não foram informados.");
            }

            if (string.IsNullOrWhiteSpace(projeto.NomeDoProjeto))
            {
                throw new DadosInvalidosException("O nome do projeto é obrigatório.");
            }

            // Lançamento de Exceção: Verifica se já existe projeto com o mesmo nome (Regra de Negócio)
            bool duplicado = _context.Projetos.Any(p => p.NomeDoProjeto.ToLower() == projeto.NomeDoProjeto.Trim().ToLower());
            if (duplicado)
            {
                throw new ProjetoDuplicadoException($"Já existe um projeto cadastrado com o nome '{projeto.NomeDoProjeto}'.");
            }

            _context.Projetos.Add(projeto);
            _context.SaveChanges();
        }

        public void Atualizar(int id, Projeto projeto)
        {
            if (id <= 0)
            {
                throw new DadosInvalidosException("O ID informado para atualização deve ser maior que zero.");
            }

            if (projeto == null)
            {
                throw new DadosInvalidosException("Os dados do projeto para atualização não foram fornecidos.");
            }

            Projeto? projetoExistente = _context.Projetos.Find(id);

            if (projetoExistente == null)
            {
                // Lançamento de Exceção: Projeto não encontrado para atualização
                throw new ProjetoNaoEncontradoException($"Projeto com ID {id} não foi encontrado para atualização.");
            }

            // Lançamento de Exceção: Conflito caso novo nome pertença a outro projeto cadastrado
            bool nomeDuplicado = _context.Projetos.Any(p => p.Id != id && p.NomeDoProjeto.ToLower() == projeto.NomeDoProjeto.Trim().ToLower());
            if (nomeDuplicado)
            {
                throw new ProjetoDuplicadoException($"Já existe outro projeto cadastrado com o nome '{projeto.NomeDoProjeto}'.");
            }

            projetoExistente.NomeDoProjeto = projeto.NomeDoProjeto;
            projetoExistente.Area = projeto.Area;
            projetoExistente.Status = projeto.Status;

            _context.Projetos.Update(projetoExistente);
            _context.SaveChanges();
        }

        public void Deletar(int id)
        {
            if (id <= 0)
            {
                throw new DadosInvalidosException("O ID informado para exclusão deve ser maior que zero.");
            }

            Projeto? projetoExistente = _context.Projetos.Find(id);

            if (projetoExistente == null)
            {
                // Lançamento de Exceção: Projeto não encontrado para exclusão
                throw new ProjetoNaoEncontradoException($"Projeto com ID {id} não foi encontrado para exclusão.");
            }

            _context.Projetos.Remove(projetoExistente);
            _context.SaveChanges();
        }
    }
}
