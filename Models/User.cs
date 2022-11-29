using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SecretsSharingAPI.Models
{
    [Table("users")]
    public class User
    {
        [Column("Id")]
        public int Id { get; init; }

        [Column("email")]
        [EmailAddress]
        public string Email { get; set; }

        [Column("password_hash")]
        public byte[] PasswordHash { get; set; }

        [Column("password_salt")]
        public byte[] PasswordSalt { get; set; }

        /// <summary>
        /// Files of user
        /// </summary>
        public List<File> Files { get; set; } = new();
    }
}
