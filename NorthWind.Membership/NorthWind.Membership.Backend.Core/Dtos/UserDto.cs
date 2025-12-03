namespace NorthWind.Membership.Backend.Core.Dtos
{
    public class UserDto(string email, string firstName, string lastName, string cedula, IList<string> roles)
    {
        public string Email => email;
        public string FirstName => firstName;
        public string LastName => lastName;
        public string Cedula => cedula;
        public IList<string> Roles => roles;
    }
}