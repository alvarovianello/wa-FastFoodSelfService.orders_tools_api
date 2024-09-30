namespace Application.Interfaces.UseCases
{
    public interface IDeleteOrder
    {
        Task ExecuteAsync(int id);
    }
}
