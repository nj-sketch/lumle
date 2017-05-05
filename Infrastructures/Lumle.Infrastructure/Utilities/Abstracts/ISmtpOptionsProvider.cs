using System.Threading.Tasks;

namespace Lumle.Infrastructure.Utilities.Abstracts
{
    public interface ISmtpOptionsProvider
    {
        Task<SmtpOptions> GetSmtpOptions();
    }
}
