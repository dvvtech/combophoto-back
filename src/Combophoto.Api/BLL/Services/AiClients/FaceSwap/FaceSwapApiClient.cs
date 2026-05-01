using System.Net.Http.Headers;
using Combophoto.Api.BLL.Abstract;

namespace Combophoto.Api.BLL.Services.AiClients.FaceSwap
{
    public class FaceSwapApiClient : IFaceSwapApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly IStorageService _storageService;
        private readonly ILogger<FaceSwapApiClient> _logger;

        public FaceSwapApiClient(
            HttpClient httpClient,
            IStorageService storageService,
            ILogger<FaceSwapApiClient> logger)
        {
            _httpClient = httpClient;
            _storageService = storageService;
            _logger = logger;
        }

        public async Task<string?> SwapFacesAsync(
            string generatedImageUrl,
            string[] sourceImageUrls,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(generatedImageUrl) || sourceImageUrls.Length == 0)
            {
                return null;
            }

            using var form = new MultipartFormDataContent();

            var generatedImage = await DownloadImageAsync(generatedImageUrl, "generated.jpg", cancellationToken);
            if (generatedImage == null)
            {
                return null;
            }

            form.Add(CreateImageContent(generatedImage), "generated", generatedImage.FileName);

            for (var i = 0; i < sourceImageUrls.Length; i++)
            {
                var sourceImage = await DownloadImageAsync(sourceImageUrls[i], $"source-{i + 1}.jpg", cancellationToken);
                if (sourceImage == null)
                {
                    return null;
                }

                form.Add(CreateImageContent(sourceImage), "sources", sourceImage.FileName);
            }

            HttpResponseMessage response;
            try
            {
                response = await _httpClient.PostAsync("face-swap", form, cancellationToken);
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogError(ex, "Timeout while calling Face Swap API");
                return null;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP error while calling Face Swap API");
                return null;
            }

            using (response)
            {
                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync(cancellationToken);
                    _logger.LogError("Face Swap API returned {StatusCode}: {Error}", response.StatusCode, error);
                    return null;
                }

                var mediaType = response.Content.Headers.ContentType?.MediaType;
                if (mediaType == null || !mediaType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
                {
                    var error = await response.Content.ReadAsStringAsync(cancellationToken);
                    _logger.LogError("Face Swap API returned non-image response: {Response}", error);
                    return null;
                }

                var swappedImageBytes = await response.Content.ReadAsByteArrayAsync(cancellationToken);
                if (swappedImageBytes.Length == 0)
                {
                    _logger.LogError("Face Swap API returned empty image");
                    return null;
                }

                return await UploadResultAsync(swappedImageBytes, mediaType, cancellationToken);
            }
        }

        private async Task<DownloadedImage?> DownloadImageAsync(
            string imageUrl,
            string defaultFileName,
            CancellationToken cancellationToken)
        {
            try
            {
                using var response = await _httpClient.GetAsync(imageUrl, cancellationToken);
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Failed to download image {ImageUrl}. Status: {StatusCode}", imageUrl, response.StatusCode);
                    return null;
                }

                var bytes = await response.Content.ReadAsByteArrayAsync(cancellationToken);
                if (bytes.Length == 0)
                {
                    _logger.LogError("Downloaded image {ImageUrl} is empty", imageUrl);
                    return null;
                }

                var contentType = response.Content.Headers.ContentType?.MediaType ?? "image/jpeg";
                return new DownloadedImage(bytes, contentType, defaultFileName);
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogError(ex, "Timeout while downloading image {ImageUrl}", imageUrl);
                return null;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP error while downloading image {ImageUrl}", imageUrl);
                return null;
            }
        }

        private static ByteArrayContent CreateImageContent(DownloadedImage image)
        {
            var content = new ByteArrayContent(image.Bytes);
            content.Headers.ContentType = new MediaTypeHeaderValue(image.ContentType);
            return content;
        }

        private async Task<string> UploadResultAsync(
            byte[] imageBytes,
            string contentType,
            CancellationToken cancellationToken)
        {
            await using var stream = new MemoryStream(imageBytes);
            var fileName = $"{Guid.NewGuid()}.jpg";
            var objectKey = $"merge-results/{fileName}";

            var formFile = new FormFile(stream, 0, stream.Length, "file", fileName)
            {
                Headers = new HeaderDictionary(),
                ContentType = contentType
            };

            await _storageService.UploadFileAsync(formFile, objectKey, cancellationToken);
            return await _storageService.GetPresignedUrlAsync(objectKey);
        }

        private sealed record DownloadedImage(byte[] Bytes, string ContentType, string FileName);
    }
}
