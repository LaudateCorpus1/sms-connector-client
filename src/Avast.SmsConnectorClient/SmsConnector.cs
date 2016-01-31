using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Avast.SmsConnectorClient
{
    /// <summary>
    /// SMS connector client.
    /// </summary>
    public class SmsConnector
    {
        private static readonly IEnumerable<string> ErrorResponseKeysToIncludeInExceptionDetails = new[] { "responseType", "responseCode", "responseDescription" };

        /// <summary>
        /// Initializes new instance of <see cref="SmsConnector"/>.
        /// </summary>
        /// <param name="smsConnectorConfiguration">Sms connector configuration.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="smsConnectorConfiguration"/> is <c>null</c>.</exception>
        public SmsConnector(SmsConnectorConfiguration smsConnectorConfiguration)
        {
            if (smsConnectorConfiguration == null) throw new ArgumentNullException(nameof(smsConnectorConfiguration));

            SmsConnectorConfiguration = smsConnectorConfiguration;
        }

        /// <summary>
        /// SMS connector configuration.
        /// </summary>
        public SmsConnectorConfiguration SmsConnectorConfiguration { get; }

        /// <summary>
        /// Sends an SMS message.
        /// </summary>
        /// <param name="message">SMS message.</param>
        /// <returns>Task.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="message"/> is <c>null</c>.</exception>
        public async Task SendSmsAsync(SmsMessage message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));

            var url = ComposeUrl(message);

#if NET45
            var messageHandler = new WebRequestHandler();
            messageHandler.ClientCertificates.Add(SmsConnectorConfiguration.Certificate);
#else
            var messageHandler = new WinHttpHandler();
            messageHandler.ClientCertificates.Add(SmsConnectorConfiguration.Certificate);
#endif

            using (var client = new HttpClient(messageHandler))
            {
                var response = await client.GetAsync(url).ConfigureAwait(false);

                await HandleErrorResponse(response).ConfigureAwait(false);
            }
        }

        private Uri ComposeUrl(SmsMessage message)
        {
            const string pattern =
                "https://smsconnector.cz.o2.com/smsconnector/getpost/GP?action=send&baID={0}&toNumber={1}&text={2}&intruder=FALSE&multipart={3}&deliveryReport=FALSE&validityPeriod=10000&priority=1";

            var baId = WebUtility.UrlEncode(SmsConnectorConfiguration.ApplicationId);

            var phoneNumber = WebUtility.UrlEncode(message.PhoneNumber.FullPhoneNumberWithNormalizedPrefix);
            var messageText = WebUtility.UrlEncode(message.Text);
            var multipart = message.Multipart.ToString();

            return new Uri(string.Format(pattern, baId, phoneNumber, messageText, multipart));
        }

        private static async Task HandleErrorResponse(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
            {
                return;
            }

            // In case of errors, the response should contain additional textual information
            // formatted as key=value pairs separated by the \n character.
            if (response.Content != null)
            {
                var mediaType = response.Content.Headers?.ContentType?.MediaType;
                if (string.Equals(mediaType, "text/plain"))
                {
                    var textResponseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    var detailsToInclude = textResponseBody?.Split('\n')
                        .Select(s => s.Trim())
                        .Where(s => s.Length > 0)
                        .Where(v => ErrorResponseKeysToIncludeInExceptionDetails.Any(key => v.StartsWith(key, StringComparison.OrdinalIgnoreCase)))
                        .ToList();

                    if (detailsToInclude?.Count > 0)
                    {
                        throw new Exception(
                            $"Received error response from SMS connector ({(int) response.StatusCode} {response.ReasonPhrase}). {string.Join("; ", detailsToInclude)}");
                    }
                }
            }

            response.EnsureSuccessStatusCode();
        }
    }
}
