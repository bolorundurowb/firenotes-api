﻿using System.IO;
using System.Reflection;
using HandlebarsDotNet;

namespace firenotes_api.Configuration
{
    public class EmailTemplates
    {
        public static string GetForgotPasswordEmail(string resetLink)
        {
            string templateName = "ForgotPassword";
            string handlebars = GetTemplate(templateName);
            var template = Handlebars.Compile(handlebars);
            return template(new
            {
                ResetLink = resetLink
            });
        }

        private static string GetTemplate(string templateName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var fileNamespace = "firenotes_api.Configuration.EmailTemplates" + templateName + ".hbs";
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