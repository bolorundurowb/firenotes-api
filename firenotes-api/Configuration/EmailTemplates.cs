using System.IO;
using System.Reflection;
using HandlebarsDotNet;

namespace firenotes_api.Configuration
{
    public class EmailTemplates
    {
        public static string GetForgotPasswordEmail(string resetLink)
        {
            string templateName = "ForgotPassword";

            string handlebars;
            
            using (var stream = GetTemplate(templateName))
            {
                if (stream == null)
                {
                    handlebars = string.Empty;
                }
                else
                {
                    using (var streamReader = new StreamReader(stream))
                    {
                        handlebars = streamReader.ReadToEnd();
                    }
                }
            }

            var template = Handlebars.Compile(handlebars);
            return template(new
            {
                ResetLink = resetLink
            });
        }

        private static Stream GetTemplate(string templateName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var fileNamespace = "firenotes_api.Configuration.EmailTemplates" + templateName + ".hbs";
            return assembly.GetManifestResourceStream(fileNamespace);
        }
    }
}