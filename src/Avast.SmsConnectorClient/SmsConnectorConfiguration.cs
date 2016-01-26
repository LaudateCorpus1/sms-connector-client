using System;
using System.Security.Cryptography.X509Certificates;

namespace Avast.SmsConnectorClient
{
    /// <summary>
    /// Configuration for <see cref="SmsConnector"/> instances.
    /// </summary>
    public class SmsConnectorConfiguration
    {
        /// <summary>
        /// Initializes new instance of <see cref="SmsConnectorConfiguration"/>.
        /// </summary>
        /// <param name="applicationId">The Business Application Id.</param>
        /// <param name="certificate">Client certificate.</param>
        /// <exception cref="NullReferenceException">If <paramref name="applicationId"/> or <paramref name="certificate"/> is <c>null</c>.</exception>
        public SmsConnectorConfiguration(string applicationId, X509Certificate certificate)
        {
            if (applicationId == null) throw new ArgumentNullException(nameof(applicationId));
            if (applicationId == null) throw new ArgumentNullException(nameof(applicationId));

            ApplicationId = applicationId;
            Certificate = certificate;
        }

        /// <summary>
        /// The Business Application Id.
        /// </summary>
        public string ApplicationId { get; }

        /// <summary>
        /// Client certificate.
        /// </summary>
        public X509Certificate Certificate { get; }
    }
}