using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eccommerce.Domain.Entities
{
    public class User
    {
        public Guid Id { get; private set; }
        public string Email { get; private set; }
        public string PasswordHash { get; private set; }
        public string Role { get; private set; }

        private User() { }

        public User(string email, string passwordHash, string role)
        {
            if (string.IsNullOrWhiteSpace(email)) 
                throw new ArgumentNullException("Email is required");

            Id = Guid.NewGuid();
            Email = email;
            PasswordHash = passwordHash;
            Role = role;
        }
    }
}
