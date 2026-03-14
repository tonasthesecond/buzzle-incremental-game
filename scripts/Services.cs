using System;
using System.Collections.Generic;

public class Services
{
    private static readonly Dictionary<Type, object> _services = new();

    public static void Register(object service) => _services[service.GetType()] = service;

    public static void Unregister(object service) => _services.Remove(service.GetType());

    public static T? Get<T>()
        where T : class => _services.TryGetValue(typeof(T), out var s) ? (T)s : null;
}
