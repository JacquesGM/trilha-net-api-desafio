using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using TrilhaApiDesafio.Context;
using TrilhaApiDesafio.Models;

namespace TrilhaApiDesafio.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TarefaController : ControllerBase
    {
        private readonly OrganizadorContext _context;
        private readonly ILogger<TarefaController> _logger;

        public TarefaController(OrganizadorContext context, ILogger<TarefaController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObterPorId(int id)
        {
            try
            {
                var tarefa = await _context.Tarefas.FindAsync(id);

                if (tarefa == null)
                    return NotFound("Tarefa não encontrada.");

                return Ok(tarefa);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter tarefa por ID");
                return StatusCode(500, "Ocorreu um erro ao processar sua solicitação.");
            }
        }

        [HttpGet("ObterTodos")]
        public async Task<IActionResult> ObterTodos()
        {
            try
            {
                var tarefas = await _context.Tarefas.ToListAsync();

                if (tarefas == null || !tarefas.Any())
                    return NotFound("Nenhuma tarefa encontrada.");

                return Ok(tarefas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter todas as tarefas");
                return StatusCode(500, "Ocorreu um erro ao processar sua solicitação.");
            }
        }

        [HttpGet("ObterPorTitulo")]
        public async Task<IActionResult> ObterPorTitulo(string titulo)
        {
            try
            {
                var tarefas = await _context.Tarefas.Where(t => t.Titulo.Contains(titulo)).ToListAsync();

                if (tarefas == null || !tarefas.Any())
                    return NotFound("Nenhuma tarefa encontrada com o título especificado.");

                return Ok(tarefas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter tarefas por título");
                return StatusCode(500, "Ocorreu um erro ao processar sua solicitação.");
            }
        }

        [HttpGet("ObterPorData")]
        public async Task<IActionResult> ObterPorData(DateTime data)
        {
            try
            {
                var tarefas = await _context.Tarefas.Where(x => x.Data.Date == data.Date).ToListAsync();

                if (tarefas == null || !tarefas.Any())
                    return NotFound("Nenhuma tarefa encontrada para a data especificada.");

                return Ok(tarefas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter tarefas por data");
                return StatusCode(500, "Ocorreu um erro ao processar sua solicitação.");
            }
        }

        [HttpGet("ObterPorStatus")]
        public async Task<IActionResult> ObterPorStatus(EnumStatusTarefa status)
        {
            try
            {
                var tarefas = await _context.Tarefas.Where(x => x.Status == status).ToListAsync();

                if (tarefas == null || !tarefas.Any())
                    return NotFound("Nenhuma tarefa encontrada para o status especificado.");

                return Ok(tarefas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter tarefas por status");
                return StatusCode(500, "Ocorreu um erro ao processar sua solicitação.");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Criar(Tarefa tarefa)
        {
            if (tarefa.Data == DateTime.MinValue)
                return BadRequest(new { Erro = "A data da tarefa não pode ser vazia" });

            try
            {
                await _context.Tarefas.AddAsync(tarefa);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(ObterPorId), new { id = tarefa.Id }, tarefa);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar nova tarefa");
                return StatusCode(500, "Ocorreu um erro ao processar sua solicitação.");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Atualizar(int id, Tarefa tarefa)
        {
            if (tarefa.Data == DateTime.MinValue)
                return BadRequest(new { Erro = "A data da tarefa não pode ser vazia" });

            try
            {
                var tarefaBanco = await _context.Tarefas.FindAsync(id);

                if (tarefaBanco == null)
                    return NotFound("Tarefa não encontrada.");

                tarefaBanco.Titulo = tarefa.Titulo;
                tarefaBanco.Descricao = tarefa.Descricao;
                tarefaBanco.Data = tarefa.Data;
                tarefaBanco.Status = tarefa.Status;

                _context.Tarefas.Update(tarefaBanco);
                await _context.SaveChangesAsync();

                return Ok(tarefaBanco);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar tarefa");
                return StatusCode(500, "Ocorreu um erro ao processar sua solicitação.");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Deletar(int id)
        {
            try
            {
                var tarefaBanco = await _context.Tarefas.FindAsync(id);

                if (tarefaBanco == null)
                    return NotFound("Tarefa não encontrada.");

                _context.Tarefas.Remove(tarefaBanco);
                await _context.SaveChangesAsync();
                
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao deletar tarefa");
                return StatusCode(500, "Ocorreu um erro ao processar sua solicitação.");
            }
        }
    }
}
