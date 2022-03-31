using System;
using System.Runtime.Serialization;

namespace Litium.Accelerator.Services
{
    /// <summary>
    /// The exception supports to display more information on Error Page.
    /// </summary>
    public class ErrorPageMoreInfoException : Exception
    {
        /// <summary>
		/// Initializes a new instance of the <see cref="ErrorPageMoreInfoException"/> class.
		/// </summary>
        public ErrorPageMoreInfoException()
        {
        }

        /// <summary>
		/// Initializes a new instance of the <see cref="ErrorPageMoreInfoException"/> class.
		/// </summary>
        /// <param name="message">The exception message.</param>
        public ErrorPageMoreInfoException(string message) : base(message)
        {
        }

        /// <summary>
		/// Initializes a new instance of the <see cref="ErrorPageMoreInfoException"/> class.
		/// </summary>
        /// <param name="message">The exception message.</param>
		/// <param name="innerException">The inner exception.</param>
        public ErrorPageMoreInfoException(string message, Exception innerException) : base(message, innerException)
        {
        }


        /// <summary>
		/// Initializes a new instance of the <see cref="ChangePasswordException"/> class.
		/// </summary>
		/// <param name="info">The object that holds the serialized object data.</param>
		/// <param name="context">The contextual information about the source or destination.</param>
        protected ErrorPageMoreInfoException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
