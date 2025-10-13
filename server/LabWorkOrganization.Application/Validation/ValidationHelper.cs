using System.ComponentModel.DataAnnotations;

namespace LabWorkOrganization.Application.Validation
{
    public static class ValidationHelper
    {
        public static List<string> Validate(object obj)
        {
            var context = new ValidationContext(obj);
            var results = new List<ValidationResult>();
            Validator.TryValidateObject(obj, context, results, true);

            var errors = new List<string>();
            foreach (var result in results)
                errors.Add(result.ErrorMessage ?? "Validation error");

            return errors;
        }
    }
}
