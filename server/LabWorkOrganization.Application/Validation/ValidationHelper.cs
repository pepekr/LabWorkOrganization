using System.ComponentModel.DataAnnotations;

namespace LabWorkOrganization.Application.Validation
{
    public static class ValidationHelper
    {
        public static List<string> Validate(object obj)
        {
            ValidationContext context = new(obj);
            List<ValidationResult> results = new();
            Validator.TryValidateObject(obj, context, results, true);

            List<string> errors = new();
            foreach (ValidationResult result in results)
            {
                errors.Add(result.ErrorMessage ?? "Validation error");
            }

            return errors;
        }
    }
}
