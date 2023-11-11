using RestSharp;

namespace Rsoi.Lab3.GatewayService.Services.Exceptions;

public class InternalServiceException : Exception
{
    public string ServiceName { get; set; }
    
    public RestRequest Request { get; set; }
    
    public int? ErrorCode { get; set; }
    
    public string? Description { get; set; }

    public InternalServiceException(RestRequest request, string serviceName, int? errorCode = null, string? description = null) : base()
    {
        Request = request;
        ServiceName = serviceName;
        ErrorCode = errorCode;
        Description = description;
    }
    
    public InternalServiceException(RestRequest request, string message, string serviceName, int? errorCode = null, string? description = null) : base(message)
    {
        ServiceName = serviceName;
        ErrorCode = errorCode;
        Description = description;
        Request = request;
    }
}