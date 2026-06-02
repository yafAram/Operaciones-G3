using API.G3Operaciones.Garcias.MX.Domain.Entities.Enums;
using System.Data;

namespace API.G3Operaciones.Garcias.MX.Domain.Entities.Users
{
    public class User
    {
        public Guid Id { get; private set; }

        public string FirstName { get; private set; } = default!;
        public string LastName { get; private set; } = default!;

        public DateTime BirthDate { get; private set; }
        public string PhoneNumber { get; private set; } = default!;

        public Guid RoleId { get; private set; }
        public Role Role { get; private set; } = default!;

        public UserStatus Status { get; private set; }

        // Constructor público necesario para instanciar User en DataSeeder
        public User()
        {
        }

        public User(
            string firstName,
            string lastName,
            DateTime birthDate,
            string phoneNumber,
            Guid roleId)
        {
            Id = Guid.NewGuid();
            FirstName = firstName;
            LastName = lastName;
            BirthDate = birthDate;
            PhoneNumber = phoneNumber;
            RoleId = roleId;
            Status = UserStatus.Active;
        }

        public void UpdatePersonalInfo(string FullName, string phoneNumber)
        {
            FirstName = FullName;
            PhoneNumber = phoneNumber;
        }

        public void Deactivate()
        {
            Status = UserStatus.Inactive;
        }

        public void Activate()
        {
            Status = UserStatus.Active;
        }

        public void ChangeRole(Guid roleId)
        {
            RoleId = roleId;
        }


        public void ChangeBirthDate(DateTime birthDate)
        {
            BirthDate = birthDate;
        }


    }

}
