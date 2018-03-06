using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace TasksService.Controllers
{
    [Route("api/[controller]")]
    public class TasksController : Controller
    {
        private readonly ITasksRepository _tasksRepository;

        public TasksController(ITasksRepository tasksRepository)
        {
            _tasksRepository = tasksRepository;
        }

        /// <summary>
        /// Get all active tasks
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IEnumerable<TodoWithId>> GetAllTasks()
        {
            return await _tasksRepository.GetAllTasks();
        }
        
        /// <summary>
        /// Get task by id (duh)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<Todo> GetTask(string id)
        {
            return await _tasksRepository.GetTaskById(id);
        }

        /// <summary>
        /// Create new tasks
        /// </summary>
        /// <param name="value"></param>
        /// <returns>The task id</returns>
        [HttpPost]
        public Task<string> CreateNewTask([FromBody]Todo value)
        {
            var newTaskId = _tasksRepository.CreateNewTask(value);

            return newTaskId;
        }
    }
}
