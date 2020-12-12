using System.IO;
using System.Reflection;
using HandlebarsDotNet;

namespace firenotes_api.Configuration
{
    public static class EmailTemplates
    {
        internal static string GetForgotPasswordEmail(string resetLink)
        {
            var templateName = "ForgotPassword";
            var handlebars = GetTemplate(templateName);
            var template = Handlebars.Compile(handlebars);
            return template(new
            {
                ResetLink = resetLink
            });
        }

        internal static string GetWelcomeEmail()
        {
            var templateName = "Welcome";
            var handlebars = GetTemplate(templateName);
            var template = Handlebars.Compile(handlebars);
            return template(new {});
        }

        internal static string GetArchivedAccountEmail()
        {
            var templateName = "ArchivedAccount";
            var handlebars = GetTemplate(templateName);
            var template = Handlebars.Compile(handlebars);
            return template(new {});
        }

        internal static string GetResetPasswordEmail(string firstname)
        {
            var templateName = "ResetPassword";
            var handlebars = GetTemplate(templateName);
            var template = Handlebars.Compile(handlebars);
            return template(new
            {
                FirstName = firstname
            });
        }

        private static string GetTemplate(string templateName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var fileNamespace = "firenotes_api.Configuration.EmailTemplates." + templateName + ".hbs";
            using (var stream = assembly.GetManifestResourceStream(fileNamespace))
            {
                if (stream == null)
                {
                    return string.Empty;
                }
                using (var streamReader = new StreamReader(stream))
                {
                    return streamReader.ReadToEnd();
                }
            }
        }
    }
}