namespace Payroll.Common.FtpUtility
{  
    public static class FtpSettings
    {
        public static string Host { get; } = "ftp://192.168.7.213/Importdocs/";
        public static string Username { get; } = "Administrator";
        public static string Password { get; } = "Mantra@2023";
        public static int Port { get; } = 21; // default FTP port
    }
}
