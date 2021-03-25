#pragma warning disable 1591

using Microsoft.AspNetCore.Mvc;

namespace PMAuth.Models.RequestModels
{
    public class RedirectionErrorModelFacebook
    {

        [FromQuery(Name = "error")]
        public string Error { get; set; }

        [FromQuery(Name = "error_description")]
        public string ErrorDescription { get; set; }

        [FromQuery(Name = "error_reason")]
        public string ErrorReason { get; set; }
    }
}