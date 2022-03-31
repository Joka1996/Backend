﻿using Litium.Accelerator.Constants;
using Litium.FieldFramework;
using Litium.Websites;
using System.Collections.Generic;

namespace Litium.Accelerator.Definitions.Pages
{
    internal class MyPagesPageTemplateSetup : FieldTemplateSetup
    {
        public override IEnumerable<FieldTemplate> GetTemplates()
        {
            var templates = new List<FieldTemplate>
            {
                new PageFieldTemplate(PageTemplateNameConstants.MyPages)
                {
                    IndexThePage = false,
                    TemplatePath = "",
                    FieldGroups = new []
                    {
                        new FieldTemplateFieldGroup()
                        {
                            Id = "General",
                            Collapsed = false,
                            Fields =
                            {
                                SystemFieldDefinitionConstants.Name,
                                SystemFieldDefinitionConstants.Url
                            }
                        },
                        new FieldTemplateFieldGroup()
                        {
                            Id = "Contents",
                            Collapsed = false,
                            Fields =
                            {
                               PageFieldNameConstants.Title,
                               PageFieldNameConstants.Introduction,
                               PageFieldNameConstants.Text,
                               PageFieldNameConstants.MayUserEditLogin,
                            }
                        },
                        new FieldTemplateFieldGroup()
                        {
                            Id = "Image",
                            Collapsed = false,
                            Fields =
                            {
                                PageFieldNameConstants.Image
                            }
                        },
                        new FieldTemplateFieldGroup()
                        {
                            Id = "LinkList",
                            Collapsed = false,
                            Fields =
                            {
                                PageFieldNameConstants.OrderHistoryLink
                            }
                        },
                    }
                },
            };
            return templates;
        }
    }
}
