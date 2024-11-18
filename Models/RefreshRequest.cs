using System.ComponentModel.DataAnnotations;

namespace Chords_site.Models
{
    public class RefreshRequest
    {
        [Required]
        public string RefreshToken { get; set; }
    }
}
