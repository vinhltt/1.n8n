using System;

Console.WriteLine("=== Debug C# IsTrustedUrl Logic ===");

// Test URLs that are being used
string[] testUrls = {
    "http://localhost:3000/",
    "http://localhost:3000",
    "http://localhost:3001/",
    "https://localhost:3000/",
    "https://localhost:3001/"
};

foreach (var url in testUrls)
{
    try
    {
        var uri = new Uri(url);
        Console.WriteLine($"\nURL: {url}");
        Console.WriteLine($"  Host: {uri.Host}");
        Console.WriteLine($"  Port: {uri.Port}");
        Console.WriteLine($"  IsDefaultPort: {uri.IsDefaultPort}");
        Console.WriteLine($"  Scheme: {uri.Scheme}");
        
        // Check if localhost
        bool isLocalhost = uri.Host == "localhost" || uri.Host == "127.0.0.1";
        Console.WriteLine($"  Is localhost: {isLocalhost}");
        
        // Check port logic (mimic the C# code)
        var allowedPorts = new[] { 3000, 3001, 8080, 5173, 4200 };
        bool isPortAllowed = allowedPorts.Contains(uri.Port);
        Console.WriteLine($"  Is port allowed: {isPortAllowed}");
        Console.WriteLine($"  Final result: {isLocalhost && isPortAllowed}");
        
    }
    catch (Exception ex)
    {
        Console.WriteLine($"\nURL: {url} - ERROR: {ex.Message}");
    }
}

Console.WriteLine("\n=== Testing IsTrustedUrl method ===");
foreach (var url in testUrls)
{
    bool result = IsTrustedUrl(url);
    Console.WriteLine($"IsTrustedUrl(\"{url}\") = {result}");
}

// Copy of the actual IsTrustedUrl method from AuthController
static bool IsTrustedUrl(string url)
{
    if (string.IsNullOrEmpty(url))
        return false;

    try
    {
        var uri = new Uri(url);
        
        // Allow localhost URLs for development
        if (uri.Host == "localhost" || uri.Host == "127.0.0.1")
        {
            // Allow common development ports
            var allowedPorts = new[] { 3000, 3001, 8080, 5173, 4200 };
            return allowedPorts.Contains(uri.Port);
        }
        
        // Add more trusted domains here as needed
        var trustedHosts = new string[]
        {
            // Add production domains here
        };

        return trustedHosts.Any(host => string.Equals(host, uri.Host, StringComparison.OrdinalIgnoreCase));
    }
    catch
    {
        return false;
    }
}
