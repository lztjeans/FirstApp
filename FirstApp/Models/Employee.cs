using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;


namespace FirstApp.Models
{
    public class Employee : IdentityUser
    {
        [Required(ErrorMessage = "Please enter your name")]
        public override string UserName { get; set; }

        [Range(16, 60, ErrorMessage = "Ages 16-60 only")]
        public int Age { get; set; }

        [RegularExpression(@"\d+(\.\d{1,2})?", ErrorMessage = "Invalid, enter like # or #.##")]
        public decimal Salary { get; set; }

        public string Department { get; set; }

        [RegularExpression(@"^[MF]+$", ErrorMessage = "Select any one option")]
        public Char Sex { get; set; }

        [Required]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "E-mail is not valid")]
        public override  string Email { get; set; }

        [Required]
        public  string Password { get; set; }
    }
}