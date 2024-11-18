using System.ComponentModel.DataAnnotations;

namespace Chords_site.Models
{
    public class Song
    {
        public Song(Guid id, string title, string artistName, string text, string key, DateTime createdAt, DateTime updatedAt)
        {
            Id = id;
            Title = title;
            ArtistName = artistName;
            Text = text;
            Key = key;
            CreatedAt = createdAt;
            UpdatedAt = updatedAt;
        }

        [Key]
        public Guid Id { get; set; }

        [Required, MaxLength(200)]
        public string Title { get; set; }

        [Required, MaxLength(100)]
        public string ArtistName { get; set; }

        public string Text { get; set; }

        [MaxLength(10)]
        public string Key { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Optional: List of associated chords
        public List<Chord> Chords { get; set; } = new();
    }
}
