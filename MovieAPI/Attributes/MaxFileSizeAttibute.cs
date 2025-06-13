namespace MoviesAPI.Attributes
{
    public class MaxFileSizeAttibute : ValidationAttribute
    {
        private readonly int _extensions;
        public MaxFileSizeAttibute(int Extensions)
        {
            _extensions = Extensions;
        }
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var file = value as IFormFile;
            if (file is not null && file.Length > _extensions)
            {
                return new ValidationResult($"File size should be less than {_extensions} MB");
            }
            return ValidationResult.Success;
        }
    }
}
