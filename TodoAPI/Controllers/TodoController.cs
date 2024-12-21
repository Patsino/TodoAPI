using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;

[ApiController]
[Route("api/[controller]")]
public class TodoController : ControllerBase
{
    private readonly ITodoRepository _repository;
    private readonly IMapper _mapper;

    public TodoController(ITodoRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    // GET: api/todo
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TodoItemDTO>>> GetAllAsync()
    {
        var items = await _repository.GetAllAsync();
        var itemsDTO = _mapper.Map<IEnumerable<TodoItemDTO>>(items);
        return Ok(itemsDTO);
    }

    // GET: api/todo/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<TodoItemDTO>> GetByIdAsync(int id)
    {
        var item = await _repository.GetByIdAsync(id);
        if (item == null)
        {
            return NotFound();
        }
        var itemDTO = _mapper.Map<TodoItemDTO>(item);
        return Ok(itemDTO);
    }

    // POST: api/todo
    [HttpPost]
    public async Task<ActionResult<TodoItemDTO>> CreateAsync(TodoItemDTO itemDTO)
    {
        var item = _mapper.Map<TodoItem>(itemDTO);
        var createdItem = await _repository.AddAsync(item);
        var createdItemDTO = _mapper.Map<TodoItemDTO>(createdItem);
        return CreatedAtAction(nameof(GetByIdAsync), new { id = createdItemDTO.Id }, createdItemDTO);
    }

    // PUT: api/todo/{id}
    [HttpPut("{id}")]
    public async Task<ActionResult<TodoItemDTO>> UpdateAsync(int id, TodoItemDTO itemDTO)
    {
        if (id != itemDTO.Id)
        {
            return BadRequest();
        }

        var item = _mapper.Map<TodoItem>(itemDTO);
        var updatedItem = await _repository.UpdateAsync(item);
        var updatedItemDTO = _mapper.Map<TodoItemDTO>(updatedItem);
        return Ok(updatedItemDTO);
    }

    // DELETE: api/todo/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync(int id)
    {
        await _repository.DeleteAsync(id);
        return NoContent();
    }
}