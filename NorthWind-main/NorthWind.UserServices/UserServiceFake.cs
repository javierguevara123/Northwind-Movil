using NorthWind.Entities.Interfaces;

namespace NorthWind.UserServices
{
    public class UserServiceFake : IUserService
    {
        // Un GUID fijo para simular un usuario en pruebas
        public string UserId => "d3f2a1b0-7e8c-4a9d-b5f6-1c2d3e4f5a6b";

        public bool IsAuthenticated => true;

        public string UserName => "TestUser";

        public string FullName => "Test User Full Name";
    }
}