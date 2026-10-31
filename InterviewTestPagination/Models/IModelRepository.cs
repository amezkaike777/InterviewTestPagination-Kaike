using System.Collections.Generic;

namespace InterviewTestPagination.Models
{
    // O repositório fornece os dados. Neste teste, eles ficam na memória.
    public interface IModelRepository<T>
    {
        IEnumerable<T> All();
    }
}
