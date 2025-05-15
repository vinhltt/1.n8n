using Castle.DynamicProxy;
using Microsoft.Extensions.DependencyInjection;

namespace CoreFinance.Contracts.Extensions;

public static class ProxyExtension
{
    public static void AddProxiedScoped<TInterface, TImplementation>(this IServiceCollection services)
        where TInterface : class
        where TImplementation : class, TInterface
    {
        services.AddScoped<TImplementation>();
        services.AddScoped(typeof(TInterface), serviceProvider =>
        {
            var proxyGenerator = serviceProvider.GetRequiredService<IProxyGenerator>();
            var actual = serviceProvider.GetRequiredService<TImplementation>();
            var interceptors = serviceProvider.GetServices<IAsyncInterceptor>().ToArray();
            return proxyGenerator.CreateInterfaceProxyWithTarget(typeof(TInterface), actual, interceptors);
        });
    }

    public static void AddProxiedScoped(this IServiceCollection services, Type @interface, Type implementation)
    {
        services.AddScoped(implementation);
        services.AddScoped(@interface, serviceProvider =>
        {
            var proxyGenerator = serviceProvider.GetRequiredService<IProxyGenerator>();
            var actual = serviceProvider.GetRequiredService(implementation);
            var interceptors = serviceProvider.GetServices<IAsyncInterceptor>().ToArray();
            return proxyGenerator.CreateInterfaceProxyWithTarget(@interface, actual, interceptors);
        });
    }

    public static void AddProxiedTransient<TInterface, TImplementation>(this IServiceCollection services)
        where TInterface : class
        where TImplementation : class, TInterface
    {
        services.AddTransient<TImplementation>();
        services.AddTransient(typeof(TInterface), serviceProvider =>
        {
            var proxyGenerator = serviceProvider.GetRequiredService<IProxyGenerator>();
            var actual = serviceProvider.GetRequiredService<TImplementation>();
            var interceptors = serviceProvider.GetServices<IAsyncInterceptor>().ToArray();
            return proxyGenerator.CreateInterfaceProxyWithTarget(typeof(TInterface), actual, interceptors);
        });
    }

    public static void AddProxiedTransient(this IServiceCollection services, Type @interface, Type implementation)
    {
        services.AddTransient(implementation);
        services.AddTransient(@interface, serviceProvider =>
        {
            var proxyGenerator = serviceProvider.GetRequiredService<IProxyGenerator>();
            var actual = serviceProvider.GetRequiredService(implementation);
            var interceptors = serviceProvider.GetServices<IAsyncInterceptor>().ToArray();
            return proxyGenerator.CreateInterfaceProxyWithTarget(@interface, actual, interceptors);
        });
    }

    public static void AddProxiedSingleton<TInterface, TImplementation>(this IServiceCollection services)
        where TInterface : class
        where TImplementation : class, TInterface
    {
        services.AddSingleton<TImplementation>();
        services.AddSingleton(typeof(TInterface), serviceProvider =>
        {
            var proxyGenerator = serviceProvider.GetRequiredService<IProxyGenerator>();
            var actual = serviceProvider.GetRequiredService<TImplementation>();
            var interceptors = serviceProvider.GetServices<IAsyncInterceptor>().ToArray();
            return proxyGenerator.CreateInterfaceProxyWithTarget(typeof(TInterface), actual, interceptors);
        });
    }

    public static void AddProxiedSingleton(this IServiceCollection services, Type @interface, Type implementation)
    {
        services.AddSingleton(implementation);
        services.AddSingleton(@interface, serviceProvider =>
        {
            var proxyGenerator = serviceProvider.GetRequiredService<IProxyGenerator>();
            var actual = serviceProvider.GetRequiredService(implementation);
            var interceptors = serviceProvider.GetServices<IAsyncInterceptor>().ToArray();
            return proxyGenerator.CreateInterfaceProxyWithTarget(@interface, actual, interceptors);
        });
    }
}