using System.ComponentModel.DataAnnotations;

namespace BaseApi.DTOs.Requests
{
    public class UpdateUserRequest
    {
        [Required]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        
        [Required]
        public string Role { get; set; }

        [Required]
        public string Password { get; set; } = string.Empty;
    }
}

