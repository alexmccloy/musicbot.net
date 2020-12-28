using System.ComponentModel.DataAnnotations;

namespace Amccloy.MusicBot.Asp.Net.Models
{
    public class DiscordApiKeyModel
    {
        [Required]
        [MinLength(1, ErrorMessage = "Server Name is too short")]
        [StringLength(100, ErrorMessage = "Server Name is too long")]
        public string ServerName { get; set; }
        
        //TODO change these to better values BASED on what discord actually accepts
        [Required]
        [MinLength(1, ErrorMessage = "Api Key is too short")]
        [StringLength(100, ErrorMessage = "Api Key is too long")]
        public string ApiKey { get; set; }
    }
}