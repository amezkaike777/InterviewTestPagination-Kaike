namespace InterviewTestPagination.Models
{
    // O serviço recebe as opções da lista e devolve uma página com seus totais.
    public interface IModelService<T>
    {
        IModelRepository<T> Repository { get; set; }

        PagedResult<T> List(int page, int pageSize, string sortBy, string sortDirection);
    }
}
