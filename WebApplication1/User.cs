using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1
{
    [Table("users")]
    public class User
    {
        public int Id { get; set; }
        public string Password { get; set; }

        public string UserName { get; set; }
        public string Role { get; set; }
    }
}
