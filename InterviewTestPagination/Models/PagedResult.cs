using System.Collections.Generic;

namespace InterviewTestPagination.Models
{
    // Este objeto é a resposta da API. T permite usar a mesma estrutura em outras listas.
    public class PagedResult<T>
    {
        public List<T> Items { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
    }
}
