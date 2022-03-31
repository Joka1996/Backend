using System;

namespace Litium.Accelerator.Services
{
	/// <summary>
	/// Change password exception.
	/// </summary>
	public class ChangePasswordException : Exception
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ChangePasswordException"/> class.
		/// </summary>
		/// <param name="title">Exception title.</param>
		public ChangePasswordException(string title)
			: base(title)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ChangePasswordException"/> class.
		/// </summary>
		/// <param name="title">The title.</param>
		/// <param name="innerException">The inner exception.</param>
		public ChangePasswordException(string title, Exception innerException)
			: base(title, innerException)
		{
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="ChangePasswordException"/> class.
		/// </summary>
		/// <param name="info">The object that holds the serialized object data.</param>
		/// <param name="context">The contextual information about the source or destination.</param>
		protected ChangePasswordException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
			: base(info, context)
		{ }

        public ChangePasswordException() : base()
        {
        }
    }
}
