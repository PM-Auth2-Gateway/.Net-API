using System.Text.Json.Serialization;

namespace PMAuth.Exceptions.Models
{
    /// <summary>
    /// Error model that will be returned if something will go wrong after user redirection to social network service 
    /// </summary>
    public class ErrorModel
    {
        /// <summary>
        /// Error code.
        /// 10 - Session id expired or doesn't exists
        /// 12 - User aborted authorization
        /// 14 - Error occured during authorization
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
        
        public static ErrorModel AuthAborted => new ErrorModel
        {
            ErrorCode = 12,
            Error = "User did not confirm authorization",
            ErrorDescription = "User dropped authorization process"
        };
        
        public static ErrorModel AuthError(string error, string description) => new ErrorModel
        {
            ErrorCode = 14,
            Error = error,
            ErrorDescription = description
        };
    }
}