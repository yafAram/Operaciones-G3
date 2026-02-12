using API.G3Operaciones.Garcias.MX.Domain.Entities.Users;

namespace API.G3Operaciones.Garcias.MX.Domain.Entities
{
    namespace API.G3Operaciones.Garcias.MX.Domain.Entities
    {
        public class UserCredential
        {
            public Guid Id { get; private set; }

            public Guid UserId { get; private set; }
            public User User { get; private set; }

            public string Username { get; private set; }
            public string PasswordHash { get; private set; }

            public bool MustChangePassword { get; private set; }
            public bool IsLocked { get; private set; }

            protected UserCredential() { } // EF

            public UserCredential(
                Guid userId,
                string username,
                string passwordHash)
            {
                Id = Guid.NewGuid();
                UserId = userId;
                Username = username ?? throw new ArgumentNullException(nameof(username));
                PasswordHash = passwordHash ?? throw new ArgumentNullException(nameof(passwordHash));
                MustChangePassword = true;
                IsLocked = false;
            }

            public void UpdatePassword(string newHash)
            {
                PasswordHash = newHash;
                MustChangePassword = false;
            }


            public void ForcePasswordReset(string temporaryPasswordHash)
            {
                PasswordHash = temporaryPasswordHash;
                MustChangePassword = true;
                IsLocked = false;
            }
        }

    }
    }
