using System;
using System.Collections.Generic;
using Lumle.AuthServer.Data.Entities;
using static Lumle.AuthServer.Infrastructures.Enums.AuthEnums;

namespace Lumle.AuthServer.Infrastructures.Configuration
{
    public class Users
    {

        public static IEnumerable<MobileUser> GetDummyCustomUsers()
        {
            return new[]
            {
                new MobileUser
                {
                    SubjectId = "0c4d5b43-f29c-4f7b-bc88-0041a06fd33p",
                    UserName = "EKbana",
                    PasswordHash = "GS+iZYlAFDx3STjWrgl8OdXEc4/9bNKH",
                    PasswordSalt = "MgWjDCjvnVXJ7ncFkjN0bh4cubYGL0dU",
                    Email = "test@ekbana.com",
                    Gender = (int)Gender.Male,
                    IsStaff = true,
                    IsBlocked = false,
                    IsEmailVerified = true,
                    Provider = "application",
                    CreatedDate = DateTime.UtcNow,
                    LastUpdated = DateTime.UtcNow
                }
            };
        }
        
    }
}
