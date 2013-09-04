using System.Collections.Generic;
using Orchard.ContentManagement;
using Orchard.ContentManagement.MetaData;
using Orchard.ContentManagement.MetaData.Builders;
using Orchard.ContentManagement.MetaData.Models;
using Orchard.ContentManagement.ViewModels;

namespace CloudConstruct.SecureFileField.Settings
{
    public class SecureFileFieldEditorEvents : ContentDefinitionEditorEventsBase
    {

        public override IEnumerable<TemplateViewModel> PartFieldEditor(ContentPartFieldDefinition definition) {
            if (definition.FieldDefinition.Name == "SecureFileField")
            {
                var model = definition.Settings.GetModel<SecureFileFieldSettings>();
                yield return DefinitionTemplate(model);
            }
        }

        public override IEnumerable<TemplateViewModel> PartFieldEditorUpdate(ContentPartFieldDefinitionBuilder builder, IUpdateModel updateModel) {
            if (builder.FieldType != "SecureFileField")
            {
                yield break;
            }

            var model = new SecureFileFieldSettings();
            if (updateModel.TryUpdateModel(model, "SecureFileFieldSettings", null, null))
            {
                builder.WithSetting("SecureFileFieldSettings.Hint", model.Hint);
                builder.WithSetting("SecureFileFieldSettings.SecureDirectoryName", model.SecureDirectoryName);
                builder.WithSetting("SecureFileFieldSettings.SecureBlobAccountName", model.SecureBlobAccountName);
                builder.WithSetting("SecureFileFieldSettings.SecureBlobEndpoint", model.SecureBlobEndpoint);
                builder.WithSetting("SecureFileFieldSettings.SecureSharedKey", model.SecureSharedKey);
                builder.WithSetting("SecureFileFieldSettings.SharedAccessExpirationMinutes", model.SharedAccessExpirationMinutes.ToString());
                builder.WithSetting("SecureFileFieldSettings.AllowedExtensions", model.AllowedExtensions);
                builder.WithSetting("SecureFileFieldSettings.Required", model.Required.ToString());
                builder.WithSetting("SecureFileFieldSettings.Custom1", model.Custom1);
                builder.WithSetting("SecureFileFieldSettings.Custom2", model.Custom2);
                builder.WithSetting("SecureFileFieldSettings.Custom3", model.Custom3);
            }

            yield return DefinitionTemplate(model);
        }
    }
}