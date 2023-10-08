using System.Reflection;
using System.Runtime.InteropServices;
using NLog;

namespace RecipeFriends;

public static class DotNetInfo
{
    public static void Info(ILogger logger)
    {
        ArgumentNullException.ThrowIfNull(logger);

        try{
            logger.Info("AppDomain:");
            logger.Info("BaseDirectory: {0}", AppDomain.CurrentDomain.BaseDirectory);
            var s = AppDomain.CurrentDomain.BaseDirectory;
            var p = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            logger.Info("Environment:");
            logger.Info("OS Platform: {0}", Environment.OSVersion.Platform);
            logger.Info("OS Version: {0}", Environment.OSVersion.ToString());
            logger.Info("Version: {0}", Environment.Version.ToString());
            logger.Info("Is64BitOperatingSystem: {0}", Environment.Is64BitOperatingSystem);
            logger.Info("Is64BitProcess: {0}", Environment.Is64BitProcess);
            

            logger.Info("Runtime Information:");
            logger.Info("FrameworkDescription: {FrameworkDescription}", RuntimeInformation.FrameworkDescription);
            logger.Info("OSArchitecture: {OSArchitecture}", RuntimeInformation.OSArchitecture);
            logger.Info("OSDescription: {OSDescription}", RuntimeInformation.OSDescription);
            logger.Info("ProcessArchitecture: {ProcessArchitecture}", RuntimeInformation.ProcessArchitecture);
            logger.Info("RuntimeIdentifier: {RuntimeIdentifier}", RuntimeInformation.RuntimeIdentifier);
            logger.Info("IsOSPlatform (Windows): {IsOSPlatform}", RuntimeInformation.IsOSPlatform(OSPlatform.Windows));
            logger.Info("IsOSPlatform (OSX): {IsOSPlatform}", RuntimeInformation.IsOSPlatform(OSPlatform.OSX));
            logger.Info("IsOSPlatform (Linux): {IsOSPlatform}", RuntimeInformation.IsOSPlatform(OSPlatform.Linux));
            logger.Info("IsOSPlatform (FreeBSD): {IsOSPlatform}", RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD));

            logger.Info("Runtime Environment:");        
            logger.Info("Runtime directory: {RuntimeDirectory}", RuntimeEnvironment.GetRuntimeDirectory());
            logger.Info("System version (CLR's version): {SystemVersion}", RuntimeEnvironment.GetSystemVersion());

            logger.Info("AssemblyInformation:");
            // The following is a bit more detailed then the information from the API -> RuntimeInformation.FrameworkDescription
            logger.Info("CoreCLR Build: {Build}", ((AssemblyInformationalVersionAttribute[])typeof(object).Assembly.GetCustomAttributes(typeof(AssemblyInformationalVersionAttribute), false))[0].InformationalVersion);
            // The following is a bit more detailed then the information from the API -> Environment.Version
            logger.Info("CoreFX Build: {Build}", ((AssemblyInformationalVersionAttribute[])typeof(Uri).Assembly.GetCustomAttributes(typeof(AssemblyInformationalVersionAttribute), false))[0].InformationalVersion);
        }catch(Exception){
            // swallow any exception to 
        }
    }

    public static void GCInfo(ILogger logger)
    {
        ArgumentNullException.ThrowIfNull(logger);
        var gc = GC.GetConfigurationVariables();
        foreach (var i in gc.OrderBy((x) => x.Key))
        {
            logger.Info("{Key}:{Value}", i.Key.Replace("\"", "", StringComparison.Ordinal), i.Value);
        }
    }
}
