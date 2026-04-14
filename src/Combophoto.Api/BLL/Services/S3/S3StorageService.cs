using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Transfer;
using Combophoto.Api.BLL.Abstract;
using Combophoto.Api.Configuration;
using Microsoft.Extensions.Options;

namespace Combophoto.Api.BLL.Services.S3
{
    public class S3StorageService : IStorageService
    {
        private readonly IAmazonS3 _s3Client;

        private readonly string _bucketName;

        public S3StorageService(IOptions<S3Config> settings)
        {
            var config = settings.Value;
            _bucketName = config.BucketName;

            // Настройка клиента для S3-совместимых хранилищ
            var s3Config = new AmazonS3Config
            {
                // Используем кастомный эндпоинт (DigitalOcean, MinIO, VK, etc.)
                ServiceURL = config.Endpoint,
                ForcePathStyle = true,
                // Указываем регион, даже если он не используется провайдером,
                // SDK требует его для подписи запросов (Signature V4)
                AuthenticationRegion = config.Region ?? "us-east-1"
            };

            var fullAccessKey = $"{config.TenantId}:{config.AccessKey}";

            var credentials = new BasicAWSCredentials(fullAccessKey, config.SecretKey);
            _s3Client = new AmazonS3Client(credentials, s3Config);
        }

        /// <summary>
        /// Загрузка файла из HTTP Request (IFormFile)
        /// </summary>
        public async Task<string> UploadFileAsync(IFormFile file, string objectKey = null, CancellationToken cancellationToken = default)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("Файл не выбран или пуст.");

            // Если имя объекта не задано, берем оригинальное имя файла
            var key = string.IsNullOrEmpty(objectKey) ? file.FileName : objectKey;

            // Используем TransferUtility для автоматической оптимизации (multipart upload для больших файлов)
            using var transferUtility = new TransferUtility(_s3Client);

            using var stream = file.OpenReadStream();

            var uploadRequest = new TransferUtilityUploadRequest
            {
                InputStream = stream,
                BucketName = _bucketName,
                Key = key,
                // Опционально: контент-тайп для корректного отображения в браузере
                Headers = { ContentType = file.ContentType }
            };

            try
            {
                await transferUtility.UploadAsync(uploadRequest, cancellationToken);
                return key; // Возвращаем ключ (имя файла в хранилище)
            }
            catch (AmazonS3Exception ex)
            {
                // Обработка специфичных ошибок S3
                throw new Exception($"Ошибка S3: {ex.Message} (Код: {ex.ErrorCode})", ex);
            }
        }
    }
}
