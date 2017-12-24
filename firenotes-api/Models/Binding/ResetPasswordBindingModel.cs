namespace firenotes_api.Models.Binding
{
    public class ResetPasswordBindingModel
    {
        public string Token { get; set; }

        public string Password { get; set; }

        public string ConfirmPassword { get; set; }
    }
}