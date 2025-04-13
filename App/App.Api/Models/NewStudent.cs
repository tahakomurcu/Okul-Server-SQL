using System.ComponentModel.DataAnnotations;

namespace App.Api.Models
{
    public class NewStudent
    {
        [Required, Range(0,5001)]
        public int Number { get; set; }

        [Required, StringLength(50, MinimumLength = 3)]        
        public string Name { get; set; }

        [Required, StringLength(50, MinimumLength = 3)]        
        public string Surname { get; set; }

        [Required, MaxLength(1)]        
        public string Class { get; set; }

        [Required, Range(0, 100)]        
        public decimal Grade { get; set; }
    }
}
