using System.Threading.Tasks;

namespace Lumle.Module.Schedular.Services
{
    public interface ISendSystemHealthReportViaEmailService
    {
        Task SendHealthReportViaMailAsync();
    }
}
