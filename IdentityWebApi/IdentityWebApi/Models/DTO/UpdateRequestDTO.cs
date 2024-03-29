﻿using System.ComponentModel.DataAnnotations;

namespace IdentityWebApi.Models.DTO
{
    public class UpdateRequestDTO
    {
        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string EmailAddress { get; set; } = string.Empty;

        [Required]
        public decimal Otp { get; set; } = 000000;

        [Required]
        public string Password { get; set; } = string.Empty;
    }
}

