using System.ComponentModel.DataAnnotations;

namespace Exo.WebApi.Models
{
    public class Projeto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome do projeto é obrigatório.")]
        [StringLength(150, ErrorMessage = "O nome do projeto deve ter no máximo 150 caracteres.")]
        public string NomeDoProjeto { get; set; } = string.Empty;

        [Required(ErrorMessage = "A área do projeto é obrigatória.")]
        [StringLength(100, ErrorMessage = "A área deve ter no máximo 100 caracteres.")]
        public string Area { get; set; } = string.Empty;

        public bool Status { get; set; }
    }
}
