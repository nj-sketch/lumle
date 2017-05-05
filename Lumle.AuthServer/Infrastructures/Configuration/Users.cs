using System;
using System.Collections.Generic;
using Lumle.AuthServer.Data.Entities;

namespace Lumle.AuthServer.Infrastructures.Configuration
{
    public class Users
    {

        public static IEnumerable<CustomUser> GetDummyCustomUsers()
        {
            return new[]
            {
                new CustomUser
                {
                    SubjectId = "0c4d5b43-f29c-4f7b-bc88-0041a06fdc78",
                    UserName = "janak",
                    PasswordHash = "janak100",
                    PasswordSalt = "janaks200",
                    Email = "janak@ekbana.com",
                    Gender = "male",
                    IsStaff = true,
                    IsBlocked = false,
                    IsEmailVerified = true,
                    Provider = "Self",
                    CreatedDate = DateTime.UtcNow,
                    LastUpdated = DateTime.UtcNow
                }
            };
        }
        
    }
}
