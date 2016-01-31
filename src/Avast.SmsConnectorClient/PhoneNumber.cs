using System;
using System.Collections.Generic;
using System.Linq;

namespace Avast.SmsConnectorClient
{
    /// <summary>
    /// Represents a phone number.
    /// </summary>
    public class PhoneNumber
    {
        private static readonly IEnumerable<string> Prefixes = new[] { "+", "00" };
        private const string NormalizedPrefix = "+";

        /// <summary>
        /// Initializes a new instance of <see cref="PhoneNumber"/>.
        /// </summary>
        /// <param name="phoneNumber">Phone number. It must start with either '+' or '00' prefix, followed only by numeric characters.
        /// The phone number must include the country code. The number of digits (not including the '+' or '00' prefix) must not exceed 15.
        /// </param>
        /// <exception cref="ArgumentNullException">If <paramref name="phoneNumber"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">If <paramref name="phoneNumber"/> does not meet the specified validation constraints.</exception>
        public PhoneNumber(string phoneNumber)
        {
            if (phoneNumber == null) throw new ArgumentNullException(nameof(phoneNumber));

            FullPhoneNumber = phoneNumber;

            var matchingPrefix = false;

            foreach (var prefix in Prefixes.Where(p => p.StartsWith(p)))
            {
                matchingPrefix = true;

                var remainingCharacters = phoneNumber.Skip(prefix.Length).ToArray();
                if (remainingCharacters.Length == 0 || remainingCharacters.Length > 15 || remainingCharacters.Any(c => !char.IsNumber(c)))
                {
                    throw new ArgumentException(
                        $"After the leading {prefix}, the phone number must contain (at least some and at most 15) numeric characters.");
                }

                Prefix = prefix;
                PhoneNumberWithoutPrefix = new string(remainingCharacters);
                FullPhoneNumberWithNormalizedPrefix = NormalizedPrefix + PhoneNumberWithoutPrefix;
            }

            if (!matchingPrefix)
            {
                throw new ArgumentException(
                    $"Phone number must start with one of {string.Join(", ", Prefixes)}.", nameof(phoneNumber));
            }
        }

        /// <summary>
        /// Phone number prefix.
        /// </summary>
        public string Prefix { get; }

        /// <summary>
        /// Full phone number, including the prefix.
        /// </summary>
        public string FullPhoneNumber { get; }

        /// <summary>
        /// Phone number without prefix.
        /// </summary>
        public string PhoneNumberWithoutPrefix { get; }

        /// <summary>
        /// Phone number with the '+' prefix, regardless of the prefix originally passed to the constructor.
        /// </summary>
        public string FullPhoneNumberWithNormalizedPrefix { get; }
    }
}
