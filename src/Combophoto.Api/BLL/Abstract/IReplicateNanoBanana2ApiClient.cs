namespace Combophoto.Api.BLL.Abstract
{
    public interface IReplicateNanoBanana2ApiClient
    {
        Task<string> ProcessPredictionAsync(string[] imageUrls);
    }
}
