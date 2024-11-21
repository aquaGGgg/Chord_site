using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Chords_site.Models
{
    public enum UserStatus
    {
        Active,
        Banned,
        Pending
    }

    public enum UserRole
    {
        User,
        Admin
    }

    [Table("Users")]
    public class User
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [MaxLength(50)]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public string Email { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public UserRole Role { get; set; } = UserRole.User;

        [Required]
        public UserStatus Status { get; set; } = UserStatus.Active;

        public string? AvatarUrl { get; set; }

        public List<Guid> FavoriteSongs { get; set; } = new();

        public void AddFavoriteSong(Guid songId)
        {
            if (!FavoriteSongs.Contains(songId))
                FavoriteSongs.Add(songId);
        }

        public void RemoveFavoriteSong(Guid songId)
        {
            FavoriteSongs.Remove(songId);
        }

        public void UpdatePassword(string newPassword)
        {
            if (string.IsNullOrWhiteSpace(newPassword))
                throw new ArgumentException("Password cannot be empty", nameof(newPassword));

            PasswordHash = HashPassword(newPassword);
        }

        public bool CheckPassword(string password)
        {
            return BCrypt.Net.BCrypt.Verify(password, PasswordHash);
        }

        private string HashPassword(string password)
        {
            // Используем безопасный алгоритм хеширования (например, BCrypt)
            return BCrypt.Net.BCrypt.HashPassword(password);
        }
    }
}
