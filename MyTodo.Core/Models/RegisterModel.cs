using System.ComponentModel.DataAnnotations;

namespace MyTodo.Core.Models
{
    public class RegisterModel : NotifyDataErrorInfo
    {
        private string _name = string.Empty;

        [Required(ErrorMessage = "昵称不能为空")]
        public string Name
        {
            get { return _name; }
            set
            {
                if (_name != value)
                {
                    _name = value;
                    RaisePropertyChanged();
                    Validate(nameof(Name), value);
                }
            }
        }

        private string _email = string.Empty;

        [Required(ErrorMessage = "邮箱不能为空")]
        public string Email
        {
            get { return _email; }
            set
            {
                if (_email != value)
                {
                    _email = value;
                    RaisePropertyChanged();
                    Validate(nameof(Email), value);
                }
            }
        }

        private string _password = string.Empty;

        [Required(ErrorMessage = "密码不能为空")]
        public string Password
        {
            get { return _password; }
            set
            {
                if (_password != value)
                {
                    _password = value;
                    RaisePropertyChanged();
                    Validate(nameof(Password), value);
                }
            }
        }

        private string _confirmPassword = string.Empty;

        [Required(ErrorMessage = "密码不能为空")]
        public string ConfirmPassword
        {
            get { return _confirmPassword; }
            set
            {
                if (_confirmPassword != value)
                {
                    _confirmPassword = value;
                    RaisePropertyChanged();
                    Validate(nameof(ConfirmPassword), value);
                }
            }
        }
    }
}
