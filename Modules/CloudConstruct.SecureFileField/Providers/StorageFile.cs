using System;

namespace CloudConstruct.SecureFileField.Providers
{
    public class StorageFile : IStorageFile
    {
        public StorageFile()
        { }

        public StorageFile(IStorageFile file)
        {
            Populate(file, this);
        }

        public long ContentLength { get; set; }

        public string FileName { get; set; }

        public string ContentType { get; set; }

        public byte[] FileBytes { get; set; }

        public T ConvertTo<T>() where T : IStorageFile, new()
        {
            T covertOut = new T();

            Populate(this, covertOut);

            return covertOut;
        }

        private static void Populate(IStorageFile inObject, IStorageFile objectToPopulate)
        {
            objectToPopulate.ContentType = inObject.ContentType;
            objectToPopulate.FileName = inObject.FileName;
            objectToPopulate.ContentLength = inObject.ContentLength;
            int length = (int)inObject.ContentLength;

            objectToPopulate.FileBytes = new byte[length];
            Array.Copy(inObject.FileBytes, objectToPopulate.FileBytes, length);
        }

        public static string GetContentType(string fileExt)
		{
            const string DEFAULT_TYPE = "application/octet-stream";
			string contentType = DEFAULT_TYPE;
            
            switch (fileExt.ToLower()) {
                case ".pdf":
                    contentType = "application/pdf";
                    break;
                default:
                    contentType = DEFAULT_TYPE;
                    break;
            }
            
			return contentType;
        }
    }
}
