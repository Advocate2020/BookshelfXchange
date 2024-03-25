namespace BookshelfXchange.ViewModels.POST
{
    using System.ComponentModel.DataAnnotations;

    public class LoginViewModel
    {

        [Required]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string? Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        public string? Password { get; set; }

    }

}
