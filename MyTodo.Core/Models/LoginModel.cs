using System.ComponentModel.DataAnnotations;

namespace MyTodo.Core.Models
{
    public class LoginModel : NotifyDataErrorInfo
    {
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
    }
}
