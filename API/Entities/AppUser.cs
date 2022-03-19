﻿using API.Extensions;
using System;
using System.Collections.Generic;

namespace API.Entities
{
    public class AppUser
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        //add migration to add new column to table

        public DateTime DateOfBirth { get; set; }
        public string KnownAs { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;
        public DateTime LastActive { get; set; } = DateTime.Now;
        public string Gender { get; set; }
        public string Introduction { get; set; }
        public string LookingFor { get; set; }
        public string Interests { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public ICollection<Photo> Photos { get; set; } //1 to many - 1 user and many photos

        //add-migration AddExtendedUsrEntity
        //update-database

        //We are using this one in AutoMapperProfiles, so no need here
        /*public int GetAge()
         {
             return DateOfBirth.CalculateAge();//calling extension method
         }*/
    }
}
