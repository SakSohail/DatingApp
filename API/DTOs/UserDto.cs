namespace API.DTOs
{
    public class UserDto
    {
        public string Username { get; set; }
        public string Token { get; set; }
        public string PhotoUrl { get; set; } //isMain photo
        public string KnownAs { get; set; }
        public string Gender { get; set; }
    }
}
