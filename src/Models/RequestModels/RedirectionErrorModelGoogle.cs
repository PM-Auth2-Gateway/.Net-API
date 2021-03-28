using Microsoft.AspNetCore.Mvc;
#pragma warning disable 1591

namespace PMAuth.Models.RequestModels
{
    public class RedirectionErrorModelGoogle
    {
        /// <summary>
        /// Error
        /// </summary>
        [FromQuery(Name = "error")]
        public string Error { get; set; }
        
        /// <summary>
        /// Detailed description of an error
        /// </summary>
        [FromQuery(Name = "error_description")]
        public string ErrorDescription { get; set; }
    }
}