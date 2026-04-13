namespace Combophoto.Api.BLL.Abstract
{
    public interface IStorageService
    {
        Task<string> UploadFileAsync(IFormFile file, string objectKey = null, CancellationToken cancellationToken = default);
    }
}
