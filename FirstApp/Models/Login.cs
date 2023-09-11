using System.ComponentModel.DataAnnotations;

namespace FirstApp.Models
{
    public class Login
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        public string ReturnUrl { get; set; }

        public bool Remember { get; set; }

        public bool IsAuthenticated {  get; set; }
    }
}
