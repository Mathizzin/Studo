
namespace Gordo.Models
{
    public class Tarefa
    {
        public int TarefaId { get; set; }
        public string? Descricao { get; set; }
        public bool Concluida { get; set; }

        public int UsuarioId { get; set; }
        public Usuario? Usuario { get; set; }
    }
}