using Gordo.Data;
using Gordo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class TarefaController : ControllerBase
{
    private readonly AppDataContext _context;

    public TarefaController(AppDataContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<object>>> GetTarefas()
    {
        var tarefas = await _context.Tarefas
            .Include(t => t.Usuario)
            .Select(t => new
            {
                t.TarefaId,
                t.Descricao,
                t.Concluida,
                t.UsuarioId,
                Usuario = new
                {
                    t.Usuario!.UsuarioId,
                    t.Usuario.Nome
                }
            })
            .ToListAsync();

        return tarefas;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<object>> GetTarefa(int id)
    {
        var tarefa = await _context.Tarefas
            .Include(t => t.Usuario)
            .FirstOrDefaultAsync(t => t.TarefaId == id);

        if (tarefa == null)
        {
            return NotFound();
        }

        // Construa um objeto com todas as informações necessárias
        var tarefaComUsuario = new
        {
            tarefa.TarefaId,
            tarefa.Descricao,
            tarefa.Concluida,
            tarefa.UsuarioId,
            Usuario = new
            {
                tarefa.Usuario!.UsuarioId,
                tarefa.Usuario.Nome
                // Adicione outras propriedades do usuário, se necessário
            }
        };

        return tarefaComUsuario;
    }

    [HttpPost]
    public async Task<ActionResult<Tarefa>> PostTarefa([FromBody] Tarefa tarefa)
    {
        if (tarefa == null)
        {
            return BadRequest("Dados de tarefa inválidos");
        }

        _context.Tarefas.Add(tarefa);
        await _context.SaveChangesAsync();

        // Busque o nome do usuário associado ao usuarioID
        var usuarioNome = await _context.Usuarios
            .Where(u => u.UsuarioId == tarefa.UsuarioId)
            .Select(u => u.Nome)
            .FirstOrDefaultAsync();

        // Construa o objeto retornado com o nome do usuário
        var tarefaComUsuario = new
        {
            tarefa.TarefaId,
            tarefa.Descricao,
            tarefa.Concluida,
            tarefa.UsuarioId,
            UsuarioNome = usuarioNome
        };

        return CreatedAtAction(nameof(GetTarefa), new { id = tarefa.TarefaId }, tarefaComUsuario);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<Tarefa>> PutTarefa(int id, [FromBody] Tarefa atualizacaoTarefa)
    {
        var tarefa = await _context.Tarefas.FindAsync(id);

        if (tarefa == null)
        {
            return NotFound();
        }

        // Atualize as propriedades da tarefa com base nos dados de atualização
        tarefa.Concluida = atualizacaoTarefa.Concluida;
        // Atualize outras propriedades, se necessário

        try
        {
            await _context.SaveChangesAsync();

            // Após salvar no banco de dados, recupere a tarefa atualizada
            var tarefaAtualizada = await _context.Tarefas
                .Include(t => t.Usuario)
                .FirstOrDefaultAsync(t => t.TarefaId == id);

            // Verifique se a tarefa foi encontrada
            if (tarefaAtualizada == null)
            {
                return NotFound();
            }

            // Retorne a tarefa atualizada
            return tarefaAtualizada;
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!TarefaExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTarefa(int id)
    {
        var tarefa = await _context.Tarefas.FindAsync(id);
        if (tarefa == null)
        {
            return NotFound();
        }

        _context.Tarefas.Remove(tarefa);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool TarefaExists(int id)
    {
        return _context.Tarefas.Any(e => e.TarefaId == id);
    }
}
