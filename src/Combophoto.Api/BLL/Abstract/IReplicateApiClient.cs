namespace Combophoto.Api.BLL.Abstract
{
    public interface IReplicateApiClient
    {
        Task<string> ProcessPredictionAsync(string[] imageUrls);
    }
}
