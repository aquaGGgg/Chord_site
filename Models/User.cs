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

    public class User
    {
        public User(Guid id, string username, string email, string passwordHash, DateTime createdAt, UserRole role, UserStatus status)
        {
            Id = id;
            Username = username;
            Email = email;
            PasswordHash = passwordHash;
            CreatedAt = createdAt;
            UpdatedAt = createdAt;
            Role = role;
            Status = status;
        }

        public Guid Id { get; private set; }

        public string Username { get; private set; }

        public string Email { get; private set; }

        private string PasswordHash { get; set; }

        public DateTime CreatedAt { get; private set; }

        public DateTime UpdatedAt { get; private set; }

        public string? AvatarUrl { get; set; }

        public List<Guid> FavoriteSongs { get; private set; } = new();

        public UserStatus Status { get; private set; }

        public UserRole Role { get; private set; }

        public void UpdatePassword(string newPassword)
        {
            if (string.IsNullOrWhiteSpace(newPassword))
                throw new ArgumentException("Password cannot be empty", nameof(newPassword));

            PasswordHash = HashPassword(newPassword);
        }

        public bool CheckPassword(string password)
        {
            return HashPassword(password) == PasswordHash;
        }

        private string HashPassword(string password)
        {
            // Use your hashing logic here (e.g., BCrypt or SHA256).
            return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(password)); // Example only, replace with secure hashing.
        }

        public void AddFavoriteSong(Guid songId)
        {
            if (!FavoriteSongs.Contains(songId))
                FavoriteSongs.Add(songId);
        }

        public void RemoveFavoriteSong(Guid songId)
        {
            FavoriteSongs.Remove(songId);
        }
    }
}
