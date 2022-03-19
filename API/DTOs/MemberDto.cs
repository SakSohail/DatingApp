using System;
using System.Collections.Generic;

namespace API.DTOs
{
    //use of DTO - for 1-to-many relationships, when we get related data(eager loading) there will be circular data cycle,cos user have list of photos, and photo have userid ,for that reason we have to use DTO

    //automapper map all properties given below, even if its lower case, and in Entity we have GetAge() method that return age, but we have Age property in Dto, then automapper map this as well
    public class MemberDto
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string PhotoUrl { get; set; }
        public int Age { get; set; }
        public string KnownAs { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastActive { get; set; }
        public string Gender { get; set; }
        public string Introduction { get; set; }
        public string LookingFor { get; set; }
        public string Interests { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public ICollection<PhotoDto> Photos { get; set; }
    }
}
