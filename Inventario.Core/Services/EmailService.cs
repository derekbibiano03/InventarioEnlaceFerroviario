using Azure.Identity;
using Microsoft.Graph;
using Microsoft.Extensions.Configuration;

namespace Inventario.Core.Services
{
    public class EmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task EnviarCorreoNotificacionAsync(string correoDestino, string asunto, string cuerpoHtml)
        {
            var tenantId = _configuration["AzureAd:TenantId"];
            var clientId = _configuration["AzureAd:ClientId"];
            var clientSecret = _configuration["AzureAd:ClientSecret"];
            var remitente = _configuration["AzureAd:Remitente"];

            if (string.IsNullOrEmpty(tenantId) || string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret) || string.IsNullOrEmpty(remitente))
            {
                throw new InvalidOperationException("Faltan configuraciones de AzureAd en el archivo appsettings.json para el envío de correos.");
            }

            // Configuración de credenciales de Azure AD (Client Credentials Flow)
            var options = new TokenCredentialOptions
            {
                AuthorityHost = AzureAuthorityHosts.AzurePublicCloud
            };

            var clientSecretCredential = new ClientSecretCredential(tenantId, clientId, clientSecret, options);
            var graphClient = new GraphServiceClient(clientSecretCredential, new[] { "https://graph.microsoft.com/.default" });

            // Construcción del mensaje utilizando tipos totalmente calificados para evitar ambigüedades
            var requestBody = new Microsoft.Graph.Users.Item.SendMail.SendMailPostRequestBody
            {
                Message = new Microsoft.Graph.Models.Message
                {
                    Subject = asunto,
                    Body = new Microsoft.Graph.Models.ItemBody
                    {
                        ContentType = Microsoft.Graph.Models.BodyType.Html,
                        Content = cuerpoHtml
                    },
                    ToRecipients = new List<Microsoft.Graph.Models.Recipient>
                    {
                        new Microsoft.Graph.Models.Recipient
                        {
                            EmailAddress = new Microsoft.Graph.Models.EmailAddress
                            {
                                Address = correoDestino
                            }
                        }
                    }
                },
                SaveToSentItems = true
            };

            // Envío del correo mediante la cuenta del remitente configurada
            await graphClient.Users[remitente].SendMail.PostAsync(requestBody);
        }
    }
}