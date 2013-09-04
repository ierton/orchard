namespace CloudConstruct.SecureFileField.Settings
{
    public class SecureFileFieldSettings
    {
        public string Hint { get; set; }
        public string AllowedExtensions { get; set; }
        public bool Required { get; set; }
        public string SecureDirectoryName { get; set; }
        public string SecureBlobAccountName { get; set; }
        public string SecureSharedKey { get; set; }
        public string SecureBlobEndpoint { get; set; }
        public int SharedAccessExpirationMinutes { get; set; }
        public string Custom1 { get; set; }
        public string Custom2 { get; set; }
        public string Custom3 { get; set; }
    }
}
