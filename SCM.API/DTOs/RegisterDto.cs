namespace SCM_System.DTOs.Auth
{
    public class RegisterDto
    {
        public string FullName { get; set; }= null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public int RoleId { get; set; }
        public bool IsActive { get; set; } = true;
    }
}