using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SecretsSharingAPI.Models
{
    [Table("Files")]
    public class File
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; init; }

        [Column("userid")]
        public int UserId { get; init; }

        [Column("file_name")]
        [MaxLength(100)]
        public string Name { get; set; }

        [Column("file_type")]
        [MaxLength(100)]
        public string FileType { get; set; }

        [MaxLength]
        public byte[] DataFiles { get; set; }

        [Column("file_uri")]
        public string Uri { get; set; }

        [Column("is_once")]
        public bool IsOnceDowloaded { get; set; }

        /// <summary>
        /// Owner of a file
        /// </summary>
        public User User { get; set; }
    }
}
