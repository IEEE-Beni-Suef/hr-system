using IEEE.Data;
using IEEE.DTO.TasksDto;
using IEEE.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IEEE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TasksController(AppDbContext context)
        {
            _context = context;
        }
        [HttpPost]
        public async Task<IActionResult> CreateTask(TaskCreateDto taskCreateDto)
        {
            if(!ModelState.IsValid) 
                {
                    return BadRequest(ModelState);
                }
            var newTask = new Tasks
            {
                Description = taskCreateDto.Description,
                HeadId = taskCreateDto.HeadId,
                Month = taskCreateDto.Month,
                CommitteeId = taskCreateDto.CommitteeId
             };
            await _context.Tasks.AddAsync(newTask);
            await _context.SaveChangesAsync();
            return Ok(newTask);
        }

        [HttpGet]
        public async Task<IActionResult> GetTasks()
        {
            var Tasks = _context.Tasks.Select(task => new TaskReadDto
            {
                Id = task.Id,
                Description = task.Description,
                HeadName = task.Head.UserName,
                Month = task.Month,
                CommiteeName = task.Committee.Name
                
            });
            if (Tasks == null)
                return BadRequest("no tasks found");
            return Ok(Tasks);
        }
        [HttpGet("Id")]
        public async Task<IActionResult> GetTask(int Id)
        {
            var Task = await _context.Tasks.Include(t=>t.Committee)
                                            .Include(t=>t.Head)
                                            .FirstOrDefaultAsync(t=>t.Id == Id);
            if (Task == null)
            {
                return NotFound($"task with Id {Id} not found");
            }
            var TaskReadDto = new TaskReadDto
            {
                Id = Task.Id,
                Description = Task.Description,
                Month = Task.Month,
                CommiteeName = Task.Committee.Name,
                HeadName = Task.Head.UserName

            };

            return Ok(TaskReadDto);
        }
        [HttpDelete("Id")]
        public async Task<IActionResult> DeleteTask(int Id)
        {

            var task = await _context.Tasks.FindAsync(Id);
            if (task == null)
            {
                return BadRequest($"task with Id {Id} not found");
            }
            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();
            return NoContent();
        }
        [HttpPost("{Id}/evaluations")]
        public async Task<IActionResult> SetTaskEvaluations(int Id,List<TaskEvaluation_CreateDto> EvaluationsDto)
        {
            if (EvaluationsDto == null)
            {
                return BadRequest();
            }

            var Evaluations = EvaluationsDto.Select(a => new Users_Tasks
            {
                UserId = a.AttendentId,
                TaskId = Id,
                Score = a.Score
             });
            await _context.Users_Tasks.AddRangeAsync(Evaluations);
            await _context.SaveChangesAsync();
            return Ok(Evaluations);
        }
        
        [HttpGet("{Id}/evaluations")]
        public async Task<IActionResult> GetTaskEvaluations(int Id)
        {
            var evaluations = await _context.Users_Tasks
                                    .Where(ut => ut.TaskId == Id)
                                    .Include(ut=>ut.User)
                                    .ToListAsync();
            if (!evaluations.Any())
            {
                return BadRequest("no evaluation found for this task");
            }
            var EvaluationsReadDto = evaluations.Select(e => new TaskEvaluation_GetDto
            {
                MemberName = e.User.UserName,
                Score = e.Score
            });

            return Ok(EvaluationsReadDto);

        }
    }
}
