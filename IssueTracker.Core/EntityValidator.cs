using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace IssueTracker.Core.Validation
{
    public static class EntityValidator
    {
        public static bool TryValidate<T>(T entity, out List<ValidationResult> results)
        {
            results = new List<ValidationResult>();
            var context = new ValidationContext(entity, serviceProvider: null, items: null);

            // validateAllProperties MUST be true to check every Data Annotation attribute
            return Validator.TryValidateObject(entity, context, results, validateAllProperties: true);
        }
    }
}