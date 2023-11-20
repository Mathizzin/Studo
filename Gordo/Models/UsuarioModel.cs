
using System.ComponentModel.DataAnnotations;

namespace Gordo.Models;

public class Usuario
{
    public int UsuarioId { get; set; }

    [Required(ErrorMessage = "O Campo nome é obrigatorio")]
    public string? Nome { get; set; }

    public List<Tarefa>? Tarefas { get; set; }
}