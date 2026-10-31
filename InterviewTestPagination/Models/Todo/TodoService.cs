using System;
using System.Collections.Generic;
using System.Linq;

namespace InterviewTestPagination.Models.Todo
{
    public class TodoService : IModelService<Todo>
    {
        // A propriedade permite substituir o repositório por dados de teste.
        public IModelRepository<Todo> Repository { get; set; }

        public TodoService()
        {
            Repository = new TodoRepository();
        }

        public PagedResult<Todo> List(int page, int pageSize, string sortBy, string sortDirection)
        {
            // Validações simples impedem páginas negativas e opções não previstas no teste.
            if (page < 1)
            {
                throw new ArgumentException("Page must be greater than zero.");
            }

            // O número zero representa a opção 'all' do seletor.
            if (pageSize != 0 && pageSize != 10 && pageSize != 20 && pageSize != 30)
            {
                throw new ArgumentException("Page size must be 10, 20, 30 or 0 (all).");
            }

            if (sortBy != "id" && sortBy != "task" && sortBy != "createdDate")
            {
                throw new ArgumentException("Sort by must be id, task or createdDate.");
            }

            if (sortDirection != "asc" && sortDirection != "desc")
            {
                throw new ArgumentException("Sort direction must be asc or desc.");
            }

            // Primeiro ordena a lista inteira. Ordenar só a página daria um resultado incorreto.
            IEnumerable<Todo> todos = Repository.All();

            switch (sortBy)
            {
                case "id":
                    if (sortDirection == "asc")
                        todos = todos.OrderBy(todo => todo.Id);
                    else
                        todos = todos.OrderByDescending(todo => todo.Id);
                    break;

                case "task":
                    if (sortDirection == "asc")
                        todos = todos.OrderBy(todo => todo.Task, StringComparer.OrdinalIgnoreCase).ThenBy(todo => todo.Id);
                    else
                        todos = todos.OrderByDescending(todo => todo.Task, StringComparer.OrdinalIgnoreCase).ThenBy(todo => todo.Id);
                    break;

                case "createdDate":
                    if (sortDirection == "asc")
                        todos = todos.OrderBy(todo => todo.CreatedDate).ThenBy(todo => todo.Id);
                    else
                        todos = todos.OrderByDescending(todo => todo.CreatedDate).ThenBy(todo => todo.Id);
                    break;
            }

            // Materializa a lista uma vez. ThenBy, acima, usa o Id para desempatar valores iguais.
            var orderedTodos = todos.ToList();
            var totalItems = orderedTodos.Count;
            var totalPages = 1;

            if (pageSize > 0)
            {
                totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            }

            // Uma lista vazia é representada como página 1 de 1, sem registros.
            if (totalPages < 1)
                totalPages = 1;

            // Se a página pedida ultrapassar o total, devolve a última página disponível.
            if (page > totalPages)
                page = totalPages;

            List<Todo> items;

            if (pageSize == 0)
            {
                page = 1;
                items = orderedTodos;
            }
            else
            {
                // Exemplo: página 2 com 20 itens pula 20 e pega os próximos 20.
                var skip = (page - 1) * pageSize;
                items = orderedTodos.Skip(skip).Take(pageSize).ToList();
            }

            // Os totais permitem que a tela monte os controles sem baixar a lista completa.
            return new PagedResult<Todo>
            {
                Items = items,
                Page = page,
                PageSize = pageSize,
                TotalItems = totalItems,
                TotalPages = totalPages
            };
        }
    }
}
