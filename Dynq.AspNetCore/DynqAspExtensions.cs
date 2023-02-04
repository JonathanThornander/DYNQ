using Microsoft.Extensions.DependencyInjection;

namespace Dynq.AspNetCore
{
    public static class DynqAspExtensions
    {
        public static void AddDynqServices(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IDynqService, DynqService>();
        }
    }
}
