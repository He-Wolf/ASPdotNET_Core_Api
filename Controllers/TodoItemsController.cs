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
    [Produces("application/json")]
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

        /// <summary>
        /// Lists all the todo items which was added to the current user.
        /// JWT needed in request header.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     method: GET
        ///     route:  api/TodoItems
        ///     body:   none
        ///     additional header:
        ///             key: authorization,
        ///             value: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiI5ZmI4N2Q0Zi0wNGZkLTQwNGUtOTUzZC1hODJmYjU4NjNmMGQiLCJqdGkiOiIxOTViNGM4Ni02M2JmLTQ4YzgtYTkyOC01ZjRjNjRjZmQ4Y2EiLCJleHAiOjE1ODc0ODg1MDEsImlzcyI6Imh0dHA6Ly9sb2NhbGhvc3Q6NTAwMCIsImF1ZCI6Imh0dHA6Ly9sb2NhbGhvc3Q6NTAwMCJ9.VnZcQD04z0nRlHOwtCtiBXheQGcO80BLYtKOYewZ4Mo
        /// </remarks>
        /// <param></param>
        /// <returns>List of todo items beloning to the current user.</returns>
        /// <response code="200">Displays all the todo items belonging to the currently logged-in user.</response>
        /// <response code="404">Returns a message if currently logged-in user does not have todo items.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<TodoViewModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(MessageViewModel), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetTodoItems()
        {
            var CurrentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var CurrentUser = await _userManager.Users
                                                .Include(u => u.TodoItems)
                                                .SingleAsync(u => u.Id == CurrentUserId);
            //_logger.LogInformation("Current user: {currentUserId}", CurrentUserId);
            //_logger.LogInformation("Current user: {currentUser.id}", CurrentUser.Id);
            
            var todoItems = CurrentUser.TodoItems.ToList();
            if(todoItems == null)
            {
                return NotFound(new MessageViewModel("ToDo items not found.", DateTime.Now));
            }
            return Ok(_mapper.Map<List<TodoItem>, List<TodoViewModel>>(todoItems));

            /*var CurrentUser = await _userManager.FindByIdAsync(CurrentUserId);
            return await _context.Entry(CurrentUser)
                                 .Collection(u => u.TodoItems)
                                 .Query()
                                 .ToListAsync();*/
        }

        /// <summary>
        /// Displays the todo item of the current user whose item id was added to request path.
        /// JWT needed in request header.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     method: GET
        ///     route:  api/TodoItems/5
        ///     body:   none
        ///     additional header:
        ///             key: authorization,
        ///             value: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiI5ZmI4N2Q0Zi0wNGZkLTQwNGUtOTUzZC1hODJmYjU4NjNmMGQiLCJqdGkiOiIxOTViNGM4Ni02M2JmLTQ4YzgtYTkyOC01ZjRjNjRjZmQ4Y2EiLCJleHAiOjE1ODc0ODg1MDEsImlzcyI6Imh0dHA6Ly9sb2NhbGhvc3Q6NTAwMCIsImF1ZCI6Imh0dHA6Ly9sb2NhbGhvc3Q6NTAwMCJ9.VnZcQD04z0nRlHOwtCtiBXheQGcO80BLYtKOYewZ4Mo
        /// </remarks>
        /// <param name="id">Item ID of the requested todo item.</param>
        /// <returns>Todo item with the requested id beloning to the current user.</returns>
        /// <response code="200">Displays todo item of the current user.</response>
        /// <response code="404">Returns a message if currently logged-in user does not have the todo item.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(TodoViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(MessageViewModel), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetTodoItem([FromRoute] long id)
        {
            var CurrentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var CurrentUser = await _userManager.Users
                                                .Include(u => u.TodoItems)
                                                .SingleAsync(u => u.Id == CurrentUserId);

            var todoItem = CurrentUser.TodoItems.Single(t => t.Id == id);

            if (todoItem == null)
            {
                return NotFound(new MessageViewModel("ToDo item not found.", DateTime.Now));
            }

            return Ok(_mapper.Map<TodoViewModel>(todoItem));
        }

        /// <summary>
        /// Modifies the todo item of the current user whose item id was added to request path with the data given in the request body.
        /// JWT needed in request header.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     method: PUT
        ///     route:  api/TodoItems/5
        ///     body:   {
        ///             "id": 5,
        ///             "name": "Eat a sandwich",
        ///             "isComplete": true
        ///             }
        ///     additional header:
        ///             key: authorization,
        ///             value: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiI5ZmI4N2Q0Zi0wNGZkLTQwNGUtOTUzZC1hODJmYjU4NjNmMGQiLCJqdGkiOiIxOTViNGM4Ni02M2JmLTQ4YzgtYTkyOC01ZjRjNjRjZmQ4Y2EiLCJleHAiOjE1ODc0ODg1MDEsImlzcyI6Imh0dHA6Ly9sb2NhbGhvc3Q6NTAwMCIsImF1ZCI6Imh0dHA6Ly9sb2NhbGhvc3Q6NTAwMCJ9.VnZcQD04z0nRlHOwtCtiBXheQGcO80BLYtKOYewZ4Mo
        /// </remarks>
        /// <param name="id">Item ID of the requested todo item.</param>
        /// <param name="todoItemVM">Modified data of the requested todo item.</param>
        /// <returns>Modified data of the todo item with the requested id beloning to the current user.</returns>
        /// <response code="200">Displays modified todo item of the current user.</response>
        /// <response code="404">Returns a message if the item id given in the body and in the path are not identical.</response>
        /// <response code="400">Returns a message if currently logged-in user does not have todo item with the requested id.</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(TodoViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(MessageViewModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(MessageViewModel), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PutTodoItem([FromRoute] long id, [FromBody] TodoViewModel todoItemVM)
        {
            var CurrentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var CurrentUser = await _userManager.Users
                                                .Include(u => u.TodoItems)
                                                .SingleAsync(u => u.Id == CurrentUserId);

            if (id != todoItemVM.Id)
            {
                return BadRequest(new MessageViewModel("ToDo item not valid.", DateTime.Now));;
            }

            TodoItem todoItem = CurrentUser.TodoItems.Single(t => t.Id == id);

            if (todoItem == null)
            {
                return NotFound(new MessageViewModel("ToDo item not found.", DateTime.Now));;
            }
            
            todoItem.Name = todoItemVM.Name;
            todoItem.IsComplete = todoItemVM.IsComplete;

            _context.Entry(todoItem).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(_mapper.Map<TodoViewModel>(todoItem));
        }

        /// <summary>
        /// Creates a new todo item for the current user with the data given in the request body.
        /// JWT needed in request header.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     method: POST
        ///     route:  api/TodoItems
        ///     body:   {
        ///             "name": "Eat a sandwich",
        ///             "isComplete": true
        ///             }
        ///     additional header:
        ///             key: authorization,
        ///             value: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiI5ZmI4N2Q0Zi0wNGZkLTQwNGUtOTUzZC1hODJmYjU4NjNmMGQiLCJqdGkiOiIxOTViNGM4Ni02M2JmLTQ4YzgtYTkyOC01ZjRjNjRjZmQ4Y2EiLCJleHAiOjE1ODc0ODg1MDEsImlzcyI6Imh0dHA6Ly9sb2NhbGhvc3Q6NTAwMCIsImF1ZCI6Imh0dHA6Ly9sb2NhbGhvc3Q6NTAwMCJ9.VnZcQD04z0nRlHOwtCtiBXheQGcO80BLYtKOYewZ4Mo
        /// </remarks>
        /// <param name="todoItemVM">Data of the requested todo item.</param>
        /// <returns>Data of the todo item created for the current user.</returns>
        /// <response code="201">Displays new todo item of the current user.</response>
        [HttpPost]
        [ProducesResponseType(typeof(TodoViewModel), StatusCodes.Status201Created)]
        public async Task<IActionResult> PostTodoItem([FromBody] TodoViewModel todoItemVM)
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

        /// <summary>
        /// Deletes the todo item of the current user whose item id was added to request path.
        /// JWT needed in request header.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     method: DELETE
        ///     route:  api/TodoItems/5
        ///     body:   none
        ///     additional header:
        ///             key: authorization,
        ///             value: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiI5ZmI4N2Q0Zi0wNGZkLTQwNGUtOTUzZC1hODJmYjU4NjNmMGQiLCJqdGkiOiIxOTViNGM4Ni02M2JmLTQ4YzgtYTkyOC01ZjRjNjRjZmQ4Y2EiLCJleHAiOjE1ODc0ODg1MDEsImlzcyI6Imh0dHA6Ly9sb2NhbGhvc3Q6NTAwMCIsImF1ZCI6Imh0dHA6Ly9sb2NhbGhvc3Q6NTAwMCJ9.VnZcQD04z0nRlHOwtCtiBXheQGcO80BLYtKOYewZ4Mo
        /// </remarks>
        /// <param name="id">Item ID of the requested todo item.</param>
        /// <returns>Message whether the deletion was successful.</returns>
        /// <response code="200">Message about the successful deletion.</response>
        /// <response code="404">Message about the unsuccessful deletion if item cannot be found.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(MessageViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(MessageViewModel), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteTodoItem([FromRoute] long id)
        {
            var CurrentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var CurrentUser = await _userManager.Users
                                                .Include(u => u.TodoItems)
                                                .SingleAsync(u => u.Id == CurrentUserId);

            var todoItem = CurrentUser.TodoItems.Single(t => t.Id == id);

            if (todoItem == null)
            {
                return NotFound(new MessageViewModel("ToDo item not found.", DateTime.Now));
            }

            CurrentUser.TodoItems.Remove(todoItem);
            await _userManager.UpdateAsync(CurrentUser);

            return Ok(new MessageViewModel("ToDo item deleted successfully.", DateTime.Now));
        }
    }
}