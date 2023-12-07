using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using WebApi_MongoDb.DTOs;
using WebApi_MongoDb.Models;

namespace WebApi_MongoDb.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public sealed class ValuesController : ControllerBase
    {
        private readonly IMongoCollection<Todo> _todo;
        public ValuesController()
        {
            var client = new MongoClient("mongodb://localhost:27017");
            var database = client.GetDatabase("tododb");

            _todo = database.GetCollection<Todo>("todos");
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var todos = await _todo.Find(todo => true).SortByDescending(s => s.Date).ToListAsync();
            return Ok(todos);
        }

        [HttpPost]
        public async Task<IActionResult> Save(TodoSaveDto request)
        {
            Todo todo = new()
            {
                Work = request.Value,
                Date = DateTime.Now.AddHours(3)
            };

            await _todo.InsertOneAsync(todo);
            return Ok(new { Message = "" });
        }

        [HttpPost]
        public async Task<IActionResult> RemoveById(TodoRemoveDto request)
        {
            await _todo.FindOneAndDeleteAsync(f => f._id == request._Id);

            return Ok(new { Message = "" });
        }

        [HttpPost]
        public async Task<IActionResult> Update(TodoUpdateDto request)
        {
            var update = Builders<Todo>.Update.Set(t => t.Work, request.Value).Set(t => t.Date, DateTime.Now.AddHours(3));
            await _todo.FindOneAndUpdateAsync(f => f._id == request._Id, update);

            return Ok(new { Message = "" });
        }
    }
}
