namespace Combophoto.Api.Configuration
{
    public class S3Config
    {
        public const string SectionName = "S3Config";

        public string Endpoint { get; init; }

        public string TenantId { get; init; }
        public string AccessKey { get; init; }
        public string SecretKey { get; init; }
        public string BucketName { get; init; }
        public string Region { get; init; }
    }
}
