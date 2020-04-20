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
using AutoMapper;

namespace web_api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TodoItemsController : ControllerBase
    {
        private readonly UserManager<WebApiUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public TodoItemsController(
            UserManager<WebApiUser> userManager,
            ApplicationDbContext context,
            IMapper mapper,
            ILogger<TodoItemsController> logger)
        {
            _userManager = userManager;
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        // GET: api/TodoItems
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TodoViewModel>>> GetTodoItems()
        {
            var CurrentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var CurrentUser = await _userManager.Users
                                                .Include(u => u.TodoItems)
                                                .SingleAsync(u => u.Id == CurrentUserId);
            //_logger.LogInformation("Current user: {currentUserId}", CurrentUserId);
            //_logger.LogInformation("Current user: {currentUser.id}", CurrentUser.Id);
            
            List<TodoItem> todoItems = CurrentUser.TodoItems.ToList();
            List<TodoViewModel> todoItemsVM = _mapper.Map<List<TodoItem>, List<TodoViewModel>>(todoItems);

            return todoItemsVM;

            /*var CurrentUser = await _userManager.FindByIdAsync(CurrentUserId);
            return await _context.Entry(CurrentUser)
                                 .Collection(u => u.TodoItems)
                                 .Query()
                                 .ToListAsync();*/
        }

        // GET: api/TodoItems/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TodoViewModel>> GetTodoItem(long id)
        {
            var CurrentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var CurrentUser = await _userManager.Users
                                                .Include(u => u.TodoItems)
                                                .SingleAsync(u => u.Id == CurrentUserId);

            var todoItem = CurrentUser.TodoItems.Single(t => t.Id == id);

            if (todoItem == null)
            {
                return NotFound();
            }

            return _mapper.Map<TodoViewModel>(todoItem);
        }

        // PUT: api/TodoItems/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTodoItem(long id, TodoViewModel todoItemVM)
        {
            var CurrentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var CurrentUser = await _userManager.Users
                                                .Include(u => u.TodoItems)
                                                .SingleAsync(u => u.Id == CurrentUserId);

            if (id != todoItemVM.Id)
            {
                return BadRequest();
            }

            TodoItem todoItem = CurrentUser.TodoItems.Single(t => t.Id == id);

            if (todoItem == null)
            {
                return NotFound();
            }
            
            todoItem.Name = todoItemVM.Name;
            todoItem.IsComplete = todoItemVM.IsComplete;

            _context.Entry(todoItem).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/TodoItems
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<TodoViewModel>> PostTodoItem(TodoViewModel todoItemVM)
        {
            var CurrentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            //with Identity usermanager
            var CurrentUser = await _userManager.Users
                                                .Include(u => u.TodoItems)
                                                .SingleAsync(u => u.Id == CurrentUserId);
            
            TodoItem todoItem = _mapper.Map<TodoItem>(todoItemVM);

            CurrentUser.TodoItems.Add(todoItem);
            await _userManager.UpdateAsync(CurrentUser);
            
            //with Entity context
            /*var CurrentUser = await _context.Users
                                              .Include(u => u.TodoItems)
                                              .SingleAsync(u => u.Id == CurrentUserId);

            CurrentUser.TodoItems.Add(todoItem);
            await _context.SaveChangesAsync();*/

            return CreatedAtAction("GetTodoItem", new { id = todoItem.Id }, _mapper.Map<TodoViewModel>(todoItem));
        }

        // DELETE: api/TodoItems/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<TodoViewModel>> DeleteTodoItem(long id)
        {
            var CurrentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var CurrentUser = await _userManager.Users
                                                .Include(u => u.TodoItems)
                                                .SingleAsync(u => u.Id == CurrentUserId);

            var todoItem = CurrentUser.TodoItems.Single(t => t.Id == id);

            if (todoItem == null)
            {
                return NotFound();
            }

            CurrentUser.TodoItems.Remove(todoItem);

            await _userManager.UpdateAsync(CurrentUser);

            return _mapper.Map<TodoViewModel>(todoItem);
        }
    }
}
