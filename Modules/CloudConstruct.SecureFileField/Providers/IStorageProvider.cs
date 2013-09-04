
namespace CloudConstruct.SecureFileField.Providers
{
    public interface IStorageProvider
    {
        bool Exists(string filename);
        T Get<T>(string filename) where T : IStorageFile, new();
        bool Insert(string filename, byte[] buffer, string contentType, long contentLength);
        bool Insert(string filename, byte[] buffer, string contentType, long contentLength, bool overwrite);
        bool Update(string filename, byte[] buffer, string contentType, long contentLength);
        bool Delete(string filename);
    }
}
