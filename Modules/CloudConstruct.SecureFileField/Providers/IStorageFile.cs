
namespace CloudConstruct.SecureFileField.Providers
{
    public interface IStorageFile
    {
        long ContentLength { get; set; }
        string FileName { get; set; }
        string ContentType { get; set; }
        byte[] FileBytes { get; set; }
    }
}
