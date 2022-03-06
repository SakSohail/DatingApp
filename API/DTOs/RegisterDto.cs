using System.ComponentModel.DataAnnotations;

namespace API.DTOs
{
    //Dto - data rnafer objects are classes to map data ,trnafer data
    public class RegisterDto
    {
        [Required] //dto will take incoming  data, so we validate in dtos, with help od DataAnnotations namepcase,
                    //Required means username and pass should be or it will throw 400 error
        public string Username { get; set; }
        [Required]
        [StringLength(8, MinimumLength = 4)] //added new validation , it will throw validation errors if not providede
        public string  Password { get; set; }
    }
}
