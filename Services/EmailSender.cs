using RestSharp;
using RestSharp.Authenticators;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace Inventory_Management.Services;

public class EmailSender : IEmailSender
{
    private readonly string _apiKey;
    private readonly string _domain;
    private readonly ILogger<EmailSender> _logger;

    public EmailSender(IConfiguration configuration, ILogger<EmailSender> logger)
    {
        _apiKey = configuration["MailGun:ApiKey"]
                  ?? throw new ArgumentNullException(nameof(configuration), "MailGun API key is not configured.");
        _domain = configuration["MailGun:Domain"]
                  ?? throw new ArgumentNullException(nameof(configuration), "MailGun domain is not configured.");
        _logger = logger;
    }

    public async Task SendEmailAsync(string email, string subject, string message)
    {
        var options = new RestClientOptions("https://api.mailgun.net")
        {
            Authenticator = new HttpBasicAuthenticator("api", _apiKey)
        };

        var client = new RestClient(options);
        var request = new RestRequest($"/v3/{_domain}/messages", Method.Post);
        request.AlwaysMultipartFormData = true;
        request.AddParameter("from", $"YourApp <mailgun@{_domain}>");
        request.AddParameter("to", email);
        request.AddParameter("subject", subject);
        request.AddParameter("html", message);

        try
        {
            var response = await client.ExecuteAsync(request);

            if (response.IsSuccessful)
            {
                _logger.LogInformation("Email sent successfully to {Email} with subject: {Subject}", email, subject);
            }
            else
            {
                _logger.LogError("Failed to send email to {Email}. Status: {StatusCode}, Content: {Content}", 
                    email, response.StatusCode, response.Content);
                throw new Exception($"Failed to send email: {response.Content}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while sending email to {Email}", email);
            throw;
        }
    }
}