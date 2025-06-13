namespace MoviesAPI.Attributes
{
    public class AllowedExtensionsAttribute:ValidationAttribute
    {
        private readonly string _extensions;
        public AllowedExtensionsAttribute(string Extensions)
        {
            _extensions = Extensions;
        }
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var file = value as IFormFile;
            if (file is not null)
            {
                var extension = Path.GetExtension(file.FileName);
                var isAllowed = _extensions.Split(',').Contains(extension,StringComparer.OrdinalIgnoreCase);
                if (!isAllowed)
                {
                    return new ValidationResult($"Only {_extensions} are allowed!");
                }

            }
            return ValidationResult.Success;
        }
    }
}
