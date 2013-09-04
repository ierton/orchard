using System;
using System.IO;

namespace CloudConstruct.SecureFileField.Providers
{
    public class SecureFileStorageProvider : IStorageProvider
    {
        private string repositoryPath;
        private const string CurrentDirectory = ".";

        public SecureFileStorageProvider(string repositoryPath)
        {
            if (repositoryPath == CurrentDirectory)
            {
                this.repositoryPath = Environment.CurrentDirectory;
            }
            else
            {
                this.repositoryPath = repositoryPath;
            }
        }
        
        public bool Exists(string filename)
        {
            return File.Exists(GetFilePath(filename));            
        }

        public T Get<T>(string filename) where T : IStorageFile, new()
        {
            T file = new T();

            FileStream fs = File.Open(GetFilePath(filename), FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            try
            {
                file.FileName = filename;
                file.ContentLength = fs.Length;
                file.ContentType = StorageFile.GetContentType(Path.GetExtension(filename));

                file.FileBytes = new byte[fs.Length];
                fs.Read(file.FileBytes, 0, (int)fs.Length);

            }
            finally
            {
                fs.Close();
            }
            return file;
        }

        public bool Insert(string filename, byte[] buffer, string contentType, long contentLength)
        {
            try
            {
                File.WriteAllBytes(GetFilePath(filename), buffer);
            }
            catch
            {
                return false;
            }
            return true;
        }

        public bool Insert(string filename, byte[] buffer, string contentType, long contentLength, bool overwrite)
        {
            try
            {
                if (!overwrite && File.Exists(GetFilePath(filename)))
                {
                    return false;
                }
                else
                {
                    File.WriteAllBytes(GetFilePath(filename), buffer);
                }
            }
            catch
            {
                return false;
            }
            return true;
        }

        public bool Update(string filename, byte[] buffer, string contentType, long contentLength)
        {
            try
            {
                File.WriteAllBytes(GetFilePath(filename), buffer);
            }
            catch
            {
                return false;
            }
            return true;
        }
        
        public bool Delete(string filename)
        {
            try
            {
                if (File.Exists(GetFilePath(filename)))
                {
                    File.Delete(GetFilePath(filename));
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
            return true;
        }


        public string GetFilePath(string filename)
        {
            return Path.Combine(repositoryPath, filename);
        }
    }
}
