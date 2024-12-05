using Prism.Mvvm;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MyTodo.Core.Models
{
    public abstract class NotifyDataErrorInfo : BindableBase, INotifyDataErrorInfo
    {
        Dictionary<string, List<string>> Errors = new Dictionary<string, List<string>>();

        public bool HasErrors => Errors.Count > 0;

        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

        public IEnumerable GetErrors(string? propertyName)
        {
            if (Errors.ContainsKey(propertyName))
            {
                return Errors[propertyName];
            }
            else
            {
                return Enumerable.Empty<string>();
            }
        }

        public void Validate(string propertyName, object propertyValue)
        {
            var results = new List<ValidationResult>();

            Validator.TryValidateProperty(
                propertyValue,
                new ValidationContext(this) { MemberName = propertyName },
                results
            );

            if (results.Count > 0)
            {
                Errors[propertyName] = results.Select(x => x.ErrorMessage).ToList();
                ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
            }
            else
            {
                Errors.Remove(propertyName);
            }
        }
    }
}
