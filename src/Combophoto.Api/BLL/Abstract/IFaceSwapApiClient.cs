namespace Combophoto.Api.BLL.Abstract
{
    public interface IFaceSwapApiClient
    {
        Task<string?> SwapFacesAsync(
            string generatedImageUrl,
            string[] sourceImageUrls,
            CancellationToken cancellationToken = default);
    }
}
