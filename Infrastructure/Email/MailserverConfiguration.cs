using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Email
{
    public class MailserverConfiguration
    {
        public const string SectionName = "Mailserver";
        public string Hostname { get; set; } = "localhost";
        public int Port { get; set; } = 25;
        public bool UseSsl { get; set; } = false;
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string FromEmail { get; set; } = "noreply@userservice.local";
        public string FromName { get; set; } = "UserService";
        public string BaseUrl { get; set; } = "https://localhost:7000";
    }
}
