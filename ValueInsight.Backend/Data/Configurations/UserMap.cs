using ValueInsight.Backend.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ValueInsight.Backend.Data.Configuration
{
    public class UserMap
    {

        public Guid Id { get; set; }

        public Guid UserId { get; set; }
        public Guid TeamId { get; set; }

        public string Role { get; set; }

        public DateTime CreatedAt { get; set; }

        public User User { get; set; }
        public Team Team { get; set; }
    }
}