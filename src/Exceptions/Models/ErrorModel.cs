using System.Text.Json.Serialization;
#pragma warning disable 1591

namespace PMAuth.Exceptions.Models
{
    /// <summary>
    /// Error model that will be returned if something will go wrong after user redirection to social network service 
    /// </summary>
    public class ErrorModel
    {
        /// <summary>
        /// Error code. <br/>
        /// 10 - Session id expired or doesn't exists <br/>
        /// 12 - User aborted authorization <br/>
        /// 14 - Error occured during authorization <br/>
        /// 16 - Invalid id <br/>
        /// 18 - Token error <br/>
        /// 20 - Unauthorized access <br/>
        /// </summary>
        public int ErrorCode { get; set; }
        
        /// <summary>
        /// Error
        /// </summary>
        [JsonPropertyName("error")]
        public string Error { get; set; }
        
        /// <summary>
        /// Detailed description of an error
        /// </summary>
        [JsonPropertyName("error_description")]
        public string ErrorDescription { get; set; }
        
        public static ErrorModel SessionIdError => new ErrorModel
        {
            ErrorCode = 10,
            Error = "Session id expired or doesn't exists",
            ErrorDescription = "There is no profile related to provided session id"
        };
        
        public static ErrorModel AuthorizationAborted => new ErrorModel
        {
            ErrorCode = 12,
            Error = "User did not confirm authorization",
            ErrorDescription = "User dropped authorization process"
        };
        
        public static ErrorModel AuthorizationError(string description) => new ErrorModel
        {
            ErrorCode = 14,
            Error = "Error occured during user authorization",
            ErrorDescription = description
        };
        public static ErrorModel IdErrorModel(string description) => new ErrorModel
        {
            ErrorCode = 16,
            Error = "Invalid id",
            ErrorDescription = description
        };
        public static ErrorModel TokenErrorModel(string description) => new ErrorModel
        {
            ErrorCode = 18,
            Error = "Token error",
            ErrorDescription = description
        };
        
        public static ErrorModel UnauthorizedAccessModel(string description) => new ErrorModel
        {
            ErrorCode = 20,
            Error = "Unauthorized access",
            ErrorDescription = description
        };
    }
}
