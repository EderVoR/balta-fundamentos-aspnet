﻿namespace Blog
{
	public static class Configuration
	{
		public static string JwtKey = "E6E17C4F4D5E437FBEA16EEC9F089A05";
		public static string ApiKeyName = "api_key";
		public static string ApiKey = "curso_api_123456789";
		public static SmtpConfiguration Smtp = new();

		public class SmtpConfiguration()
		{
            public string Host { get; set; }
            public int Port { get; set; }
            public string UserName { get; set; }
            public string Password { get; set; }
        }
    }
}
