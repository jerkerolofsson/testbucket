namespace TestBucket.Contracts.Automation.Api;
public record class HttpMessageTestResult(HttpRequestMessageDto Request, HttpResponseMessageDto Response)
{
    public List<HttpVerificationCheckResult> Checks { get; set; } = [];
    public double HttpRequestDuration { get; set; }
    public double HttpRequestSending { get; set; }
    public double HttpResponseReceiving { get; set; }
}
