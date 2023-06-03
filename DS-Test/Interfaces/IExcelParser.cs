namespace DS_Test.Interfaces
{
    public interface IExcelParser<T>
    {
        Task<List<T>> ParseAsync(IFormFile file);
        Task<List<T>> ParseAsync(IEnumerable<IFormFile> files);
    }
}
