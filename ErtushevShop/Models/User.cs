namespace ErtushevShop.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Role { get; set; }

        public User(int id, string login, string password, string lastname, string firstname, string middlename, string email, string phone, string role)
        {
            Id = id;
            Login = login;
            Password = password;
            LastName = lastname;
            FirstName = firstname;
            MiddleName = middlename;
            Email = email;
            Phone = phone;
            Role = role;
        }
    }
}
