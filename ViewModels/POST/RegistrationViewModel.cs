namespace BookshelfXchange.ViewModels.POST
{
    using System.ComponentModel.DataAnnotations;

    public class RegistrationViewModel
    {
        [Required]
        [StringLength(150, ErrorMessage = "First name is too long")]
        public string? FirstName { get; set; }

        [Required]
        [StringLength(150, ErrorMessage = "Last name is too long")]
        public string? LastName { get; set; }

        [Required]
        [Phone]
        [RegularExpression(@"^(?:\+27|0)[1-9]\d{8}$", ErrorMessage = "Invalid South African phone number")]
        public string? PhoneNumber { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string? Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        public string? Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string? ConfirmPassword { get; set; }
    }

}
