using System;
using System.Globalization;
using Litium.Accelerator.Constants;
using Litium.Accelerator.Mailing.Models;
using Litium.Websites;

namespace Litium.Accelerator.Mailing
{
    public class ForgotPasswordEmailDefinition : HtmlMailDefinition<ForgotPasswordEmailModel>
    {
        private readonly Page _page;

        public ForgotPasswordEmailDefinition(Page page, Guid channelSystemId, ForgotPasswordEmailModel model, string toEmail)
            : base(channelSystemId, model, toEmail)
        {
            _page = page;
        }

        protected override string RawBodyText => _page.Fields.GetValue<string>(LoginPageFieldNameConstants.ForgottenPasswordBody, CultureInfo.CurrentUICulture);
        protected override string RawSubjectText => _page.Fields.GetValue<string>(LoginPageFieldNameConstants.ForgottenPasswordSubject, CultureInfo.CurrentUICulture);
    }
}
