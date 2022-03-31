using System;

namespace Litium.Accelerator.ValidationRules
{
    /// <summary>
    /// Raised when the cart context validation fails.
    /// </summary>
    public class CartContextValidationException : ApplicationException
    {
        /// <summary>
		/// Initializes a new instance of the <see cref="CartContextValidationException"/> class.
		/// </summary>
		public CartContextValidationException() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CartContextValidationException"/> class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        public CartContextValidationException(string message)
            : base(message)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CartContextValidationException"/> class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <param name="e">The inner exception.</param>
        public CartContextValidationException(string message, Exception e)
            : base(message, e)
        { }
    }
}
