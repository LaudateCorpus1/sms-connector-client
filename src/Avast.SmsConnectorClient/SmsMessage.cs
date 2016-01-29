using System;

namespace Avast.SmsConnectorClient
{
    /// <summary>
    /// SMS message.
    /// </summary>
    public class SmsMessage
    {
        /// <summary>
        /// Maximum message length (for multipart messages).
        /// </summary>
        public static readonly int MaximumMessageLength = 900;

        /// <summary>
        /// Initializes new instance of <see cref="SmsMessage"/>.
        /// </summary>
        /// <param name="phoneNumber">Phone number.</param>
        /// <param name="messageText">Message text. Must not be longer than the value of <see cref="MaximumMessageLength"/>.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="phoneNumber"/> or <paramref name="messageText"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">If <paramref name="messageText"/> does not meet the specified constraints.</exception>
        public SmsMessage(PhoneNumber phoneNumber, string messageText)
        {
            if (phoneNumber == null) throw new ArgumentNullException(nameof(phoneNumber));
            if (messageText == null) throw new ArgumentNullException(nameof(messageText));

            if (messageText.Length > 900) throw new ArgumentException("Text is too long. Maximum length is 900 characters.", nameof(messageText));

            PhoneNumber = phoneNumber;
            Text = messageText;
        }

        /// <summary>
        /// Destination phone number.
        /// </summary>
        public PhoneNumber PhoneNumber { get; }

        /// <summary>
        /// Message text.
        /// </summary>
        public string Text { get; }

        /// <summary>
        /// Whether the message will be sent as multiplart.
        /// </summary>
        public bool Multipart => Text.Length > 160;

        public override string ToString()
        {
            return $"{{ PhoneNumber: {PhoneNumber}, Text: '{Text}' }}";
        }
    }
}