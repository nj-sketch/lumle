using System.Threading.Tasks;
using Lumle.Infrastructure.Utilities.Abstracts;
using Microsoft.Extensions.Options;

namespace Lumle.Infrastructure
{
    public class ConfigSmtpOptionsProvider : ISmtpOptionsProvider
    {
        public ConfigSmtpOptionsProvider(IOptions<SmtpOptions> smtpOptionsAccessor)
        {
            _smtpSettings = smtpOptionsAccessor.Value;
        }

        private readonly SmtpOptions _smtpSettings;

        public Task<SmtpOptions> GetSmtpOptions()
        {
            return Task.FromResult(_smtpSettings);
        }
    }
}
