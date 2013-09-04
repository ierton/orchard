using System;
using System.IO;
using System.Linq;
using CloudConstruct.SecureFileField.Providers;
using CloudConstruct.SecureFileField.Settings;
using Orchard;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.ContentManagement.Handlers;
using Orchard.Localization;
using Orchard.Utility.Extensions;

namespace CloudConstruct.SecureFileField.Drivers {

    public class SecureFileFieldDriver : ContentFieldDriver<Fields.SecureFileField>
    {
        private readonly IWorkContextAccessor _workContextAccessor;

        public SecureFileFieldDriver(IWorkContextAccessor workContextAccessor)
        {
            T = NullLocalizer.Instance;
            _workContextAccessor = workContextAccessor;

        }

        public Localizer T { get; set; }

        private static string GetPrefix(Fields.SecureFileField field, ContentPart part)
        {
            return part.PartDefinition.Name + "." + field.Name;
        }

        private static string GetDifferentiator(Fields.SecureFileField field, ContentPart part)
        {
            return field.Name;
        }

        protected override DriverResult Display(ContentPart part, Fields.SecureFileField field, string displayType, dynamic shapeHelper) {
            var appPath = this._workContextAccessor.GetContext().HttpContext.Request.ApplicationPath;
            field.SecureUrl = appPath+"/CloudConstruct.SecureFileField/SecureFileField/GetSecureFile/" + part.Id + "?fieldName=" + field.Name;
            
            //does the user want to use shared access sigs
            var settings = field.PartFieldDefinition.Settings.GetModel<SecureFileFieldSettings>();

            if (settings.SharedAccessExpirationMinutes > 0)
            {
            }

            return ContentShape("Fields_SecureFile", GetDifferentiator(field, part),
                () => shapeHelper.Fields_SecureFile());
        }

        protected override DriverResult Editor(ContentPart part, Fields.SecureFileField field, dynamic shapeHelper)
        {
            return ContentShape("Fields_SecureFile_Edit", GetDifferentiator(field, part),
                () => shapeHelper.EditorTemplate(TemplateName: "Fields/SecureFile.Edit", Model: field, Prefix: GetPrefix(field, part)));
        }
        
        protected override DriverResult Editor(ContentPart part, Fields.SecureFileField field, IUpdateModel updater, dynamic shapeHelper) {
            
            WorkContext wc = _workContextAccessor.GetContext();
            var file = wc.HttpContext.Request.Files["FileField-" + field.Name];

            // if the model could not be bound, don't try to validate its properties
            if (updater.TryUpdateModel(field, GetPrefix(field, part), null, null)) {
                var settings = field.PartFieldDefinition.Settings.GetModel<SecureFileFieldSettings>();

                var extensions = String.IsNullOrWhiteSpace(settings.AllowedExtensions)
                        ? new string[0]
                        : settings.AllowedExtensions.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);

                try {
                    if (file != null && file.ContentLength > 0) {
                        string fname = Path.GetFileName(file.FileName);
                        field.Url = fname;
                        IStorageProvider provider;

                        provider = new SecureFileStorageProvider(settings.SecureDirectoryName);

                        int length = (int) file.ContentLength;
                        byte[] buffer = new byte[length];
                        using (Stream stream = file.InputStream) {
                            stream.Read(buffer, 0, length);
                        }

                        provider.Insert(fname, buffer, file.ContentType, length, true);
                    }

                }
                catch (Exception) {

                    throw;
                }

                if (extensions.Any() && field.Url != null && !extensions.Any(x => field.Url.EndsWith(x, StringComparison.OrdinalIgnoreCase))) {
                    updater.AddModelError("Url", T("The field {0} must have one of these extensions: {1}", field.Name.CamelFriendly(), settings.AllowedExtensions));
                }

                if (settings.Required && String.IsNullOrWhiteSpace(field.Url)) {
                    updater.AddModelError("Url", T("The field {0} is mandatory", field.Name.CamelFriendly()));
                }

            }

            return Editor(part, field, shapeHelper);
        }

        protected override void Importing(ContentPart part, Fields.SecureFileField field, ImportContentContext context)
        {
            context.ImportAttribute(field.FieldDefinition.Name + "." + field.Name, "Url", value => field.Url = value);
            context.ImportAttribute(field.FieldDefinition.Name + "." + field.Name, "AlternateText", value => field.AlternateText = value);
            context.ImportAttribute(field.FieldDefinition.Name + "." + field.Name, "Class", value => field.Class = value);
            context.ImportAttribute(field.FieldDefinition.Name + "." + field.Name, "Style", value => field.Style = value);
            context.ImportAttribute(field.FieldDefinition.Name + "." + field.Name, "Alignment", value => field.Alignment = value);
            context.ImportAttribute(field.FieldDefinition.Name + "." + field.Name, "Width", value => field.Width = Int32.Parse(value));
            context.ImportAttribute(field.FieldDefinition.Name + "." + field.Name, "Height", value => field.Height = Int32.Parse(value));
        }

        protected override void Exporting(ContentPart part, Fields.SecureFileField field, ExportContentContext context)
        {
            context.Element(field.FieldDefinition.Name + "." + field.Name).SetAttributeValue("Url", field.Url);
            context.Element(field.FieldDefinition.Name + "." + field.Name).SetAttributeValue("AlternateText", field.AlternateText);
            context.Element(field.FieldDefinition.Name + "." + field.Name).SetAttributeValue("Class", field.Class);
            context.Element(field.FieldDefinition.Name + "." + field.Name).SetAttributeValue("Style", field.Style);
            context.Element(field.FieldDefinition.Name + "." + field.Name).SetAttributeValue("Alignment", field.Alignment);
            context.Element(field.FieldDefinition.Name + "." + field.Name).SetAttributeValue("Width", field.Width);
            context.Element(field.FieldDefinition.Name + "." + field.Name).SetAttributeValue("Height", field.Height);
        }

        protected override void Describe(DescribeMembersContext context) {
            context
                .Member(null, typeof(string), T("Url"), T("The url of the media."));
        }
    }
}