using Lumle.AuthServer.Data.Contexts;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Lumle.AuthServer.Data.Store
{
    public class TokenSnapShotStore : ITokenSnapShotStore
    {

        private readonly UserDbContext _userDbContext;

        public TokenSnapShotStore(UserDbContext userDbContext)
        {
            _userDbContext = userDbContext;
        }

        public async Task UpdateUserStatusAsync(string subjectId)
        {
            try
            {
                var tokens = _userDbContext.TokenSnapShots.Where(x => x.SubId == subjectId);
                foreach (var token in tokens)
                {
                    token.IsActive = false;
                }

                await _userDbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task UpdateUserStatusAsync(string subjectId, string jwtId)
        {
            try
            {
                var tokens = _userDbContext.TokenSnapShots.Where(x => x.SubId == subjectId && x.JwtId == jwtId);
                foreach (var token in tokens)
                {
                    token.IsActive = false;
                }

                await _userDbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
