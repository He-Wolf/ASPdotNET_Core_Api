using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using TodoApi.Data;
using TodoApi.Models;

namespace web_api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TodoItemsController : ControllerBase
    {
        private readonly UserManager<WebApiUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly ILogger _logger;

        public TodoItemsController(
            UserManager<WebApiUser> userManager,
            ApplicationDbContext context,
            ILogger<TodoItemsController> logger)
        {
            _userManager = userManager;
            _context = context;
            _logger = logger;
        }

        // GET: api/TodoItems
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TodoItem>>> GetTodoItems()
        {
            var CurrentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var CurrentUser = await _userManager.FindByIdAsync(CurrentUserId);
            //_logger.LogInformation("Current user: {currentUserId}", CurrentUserId);
            //_logger.LogInformation("Current user: {currentUser.id}", CurrentUser.Id);

            return await _context.Entry(CurrentUser)
                                 .Collection(u => u.TodoItems)
                                 .Query()
                                 .ToListAsync();
        }

        // GET: api/TodoItems/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TodoItem>> GetTodoItem(long id)
        {
            var todoItem = await _context.TodoItems.FindAsync(id);

            if (todoItem == null)
            {
                return NotFound();
            }

            return todoItem;
        }

        // PUT: api/TodoItems/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTodoItem(long id, TodoItem todoItem)
        {
            if (id != todoItem.Id)
            {
                return BadRequest();
            }

            _context.Entry(todoItem).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TodoItemExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/TodoItems
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<TodoItem>> PostTodoItem(TodoItem todoItem)
        {
            var CurrentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            

            //with Identity usermanager
            var CurrentUser = await _userManager.Users
                                                .Include(u => u.TodoItems)
                                                .SingleAsync(u => u.Id == CurrentUserId);
            CurrentUser.TodoItems.Add(todoItem);
            await _userManager.UpdateAsync(CurrentUser);
            
            //with Entity context
            /*var CurrentUser = await _context.Users
                                              .Include(u => u.TodoItems)
                                              .SingleAsync(u => u.Id == CurrentUserId);

            CurrentUser.TodoItems.Add(todoItem);
            await _context.SaveChangesAsync();*/

            return CreatedAtAction("GetTodoItem", new { id = todoItem.Id }, todoItem);
        }

        // DELETE: api/TodoItems/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<TodoItem>> DeleteTodoItem(long id)
        {
            var todoItem = await _context.TodoItems.FindAsync(id);
            if (todoItem == null)
            {
                return NotFound();
            }

            _context.TodoItems.Remove(todoItem);
            await _context.SaveChangesAsync();

            return todoItem;
        }

        private bool TodoItemExists(long id)
        {
            return _context.TodoItems.Any(e => e.Id == id);
        }
    }
}
