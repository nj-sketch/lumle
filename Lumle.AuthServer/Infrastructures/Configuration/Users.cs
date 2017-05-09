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
                    PasswordHash = "GS+iZYlAFDx3STjWrgl8OdXEc4/9bNKH",
                    PasswordSalt = "MgWjDCjvnVXJ7ncFkjN0bh4cubYGL0dU",
                    Email = "janak@ekbana.com",
                    Gender = "male",
                    IsStaff = true,
                    IsBlocked = false,
                    IsEmailVerified = true,
                    Provider = "Self",
                    CreatedDate = DateTime.UtcNow,
                    LastUpdated = DateTime.UtcNow
                },
                                new CustomUser
                {
                    SubjectId = "0c4d5b43-f29c-4f7b-bc88-0041a06fd33p",
                    UserName = "EKbana",
                    PasswordHash = "GS+iZYlAFDx3STjWrgl8OdXEc4/9bNKH",
                    PasswordSalt = "MgWjDCjvnVXJ7ncFkjN0bh4cubYGL0dU",
                    Email = "test@ekbana.com",
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
