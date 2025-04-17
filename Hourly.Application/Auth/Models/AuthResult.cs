// Hourly.Application/Auth/Models/AuthResult.cs
using System;

namespace Hourly.Application.Auth.Models
{
    public class AuthResult
    {
        public bool Success { get; set; }
        public string Token { get; set; }
        public Guid UserId { get; set; }
        public string ErrorMessage { get; set; }
    }
}