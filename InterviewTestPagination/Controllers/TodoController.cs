using System;
using System.Web.Http;
using InterviewTestPagination.Models;
using InterviewTestPagination.Models.Todo;

namespace InterviewTestPagination.Controllers
{
    public class TodoController : ApiController
    {
        private readonly IModelService<Todo> _todoService = new TodoService();

        // A rota é explícita. Os valores padrão também funcionam se a URL não tiver parâmetros.
        [HttpGet]
        [Route("api/todo/todos")]
        public IHttpActionResult Todos(int page = 1, int pageSize = 20,
            string sortBy = "createdDate", string sortDirection = "desc")
        {
            // O Web API registra aqui entradas como page=abc, que não podem virar um número.
            if (!ModelState.IsValid)
            {
                return BadRequest("The query parameters are invalid.");
            }

            try
            {
                // O controller chama o serviço, em vez de acessar o repositório diretamente.
                var result = _todoService.List(page, pageSize, sortBy, sortDirection);
                return Ok(result);
            }
            catch (ArgumentException exception)
            {
                // Uma opção inválida deve retornar HTTP 400, e não um erro interno do servidor.
                return BadRequest(exception.Message);
            }
        }
    }
}
