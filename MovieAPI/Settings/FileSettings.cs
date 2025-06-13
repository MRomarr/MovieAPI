namespace MoviesAPI.Settings
{
    public static class FileSettings
    {
        public const string AllowedExtensions = ".jpg,.png,.jpeg";
        public const int MaxFileSizeMB = 1; // 1 MB 
        public const int MaxFileSizeByte = MaxFileSizeMB * 1024 * 1024; // 1 MB
    }
}
