using Microsoft.Azure.CosmosRepository;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace Cosmos.Model
{
    public class Candidate : Item
    {
        [Required]
        public string LastName { get; set; }
        [Required]
        public string FirstName { get; set; }
        public string FullName => $"{FirstName ?? string.Empty} {LastName ?? string.Empty}";
        [Required]
        public string Email { get; set; }
        public decimal Balance { get; set; }
        public int Points { get; set; }
        public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;
        public string[] Technologies { get; set; } = Array.Empty<string>();

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
