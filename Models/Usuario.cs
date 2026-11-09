using System.ComponentModel.DataAnnotations;

namespace Exo.WebApi.Models
{
    /// <summary>
    /// Entidade que representa um usuário do sistema para autenticação e controle de acesso (autorização).
    /// </summary>
    public class Usuario
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O e-mail do usuário é obrigatório.")]
        [StringLength(150, ErrorMessage = "O e-mail deve ter no máximo 150 caracteres.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "A senha do usuário é obrigatória.")]
        [StringLength(150, ErrorMessage = "A senha deve ter no máximo 150 caracteres.")]
        public string Senha { get; set; } = string.Empty;

        [StringLength(50, ErrorMessage = "O perfil deve ter no máximo 50 caracteres.")]
        public string Perfil { get; set; } = "Admin";
    }
}

