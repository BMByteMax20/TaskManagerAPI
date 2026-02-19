using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskManagerAPI.Data;

namespace TaskManagerAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TasksController : ControllerBase
    {
        private readonly ITaskRepository _repository;

        public TasksController(ITaskRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<ActionResult<List<Models.Task>>> GetAllTasks()
        {
            var tasks = await _repository.GetAllTasksAsync();
            return Ok(tasks);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Models.Task>> GetTask(int id)
        {
            var task = await _repository.GetTaskByIdAsync(id);
            if (task == null)
            {
                return NotFound($"Задача с ID {id} не найдена.");
            }
            return Ok(task);
        }

        [HttpPost]
        public async Task<ActionResult<Models.Task>> CreateTask(Models.Task task)
        {
            if (string.IsNullOrWhiteSpace(task.Title))
            {
                return BadRequest("Название задачи не может быть пустым.");
            }

            var createdTask = await _repository.CreateTaskAsync(task);
            return CreatedAtAction(nameof(GetTask), new { id = createdTask.Id }, createdTask);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Models.Task>> UpdateTask(int id, Models.Task task)
        {
            if (id != task.Id)
            {
                return BadRequest("ID в URL не совпадает с ID в теле запроса.");
            }

            var updatedTask = await _repository.UpdateTaskAsync(id, task);
            if (updatedTask == null)
            {
                return NotFound($"Задача с ID {id} не найдена.");
            }

            return Ok(updatedTask);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var deleted = await _repository.DeleteTaskAsync(id);
            if (!deleted)
            {
                return NotFound($"Задача с ID {id} не найдена.");
            }

            return NoContent();
        }
    }
}