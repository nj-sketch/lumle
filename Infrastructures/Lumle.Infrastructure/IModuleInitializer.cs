using Microsoft.Extensions.DependencyInjection;

namespace Lumle.Infrastructure
{
    public interface IModuleInitializer
    {
        void Init(IServiceCollection serviceCollection);
    }
}
