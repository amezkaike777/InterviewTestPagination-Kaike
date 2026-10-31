using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace InterviewTestPagination.Models.Todo
{
    public class TodoRepository : IModelRepository<Todo>
    {
        // A fonte de dados original foi mantida. Não é necessário instalar um banco.
        private static readonly IDictionary<long, Todo> DataSource = new ConcurrentDictionary<long, Todo>();

        static TodoRepository()
        {
            var startDate = DateTime.Today;

            for (var i = 1; i <= 55; i++)
            {
                var createdDate = startDate.AddDays(i);
                DataSource[i] = new Todo(i, "Dont forget to do " + i, createdDate);
            }
        }

        // A ordenação saiu daqui e passou para o serviço, que conhece a opção escolhida.
        public IEnumerable<Todo> All()
        {
            return DataSource.Values;
        }
    }
}
