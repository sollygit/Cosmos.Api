using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Cosmos.Model
{
    public class CandidateForm
    {
        [Required] public int FormId { get; set; }
        [Required] public IFormFile CandidateFile { get; set; }
    }
}
