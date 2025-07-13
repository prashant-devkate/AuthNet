namespace AuthNet.Helpers
{
    public static class EmailTemplateHelper
    {
        public static string LoadTemplate(string templatePath, Dictionary<string, string> placeholders)
        {
            if (!File.Exists(templatePath))
                throw new FileNotFoundException("Email template not found.", templatePath);

            var html = File.ReadAllText(templatePath);

            foreach (var pair in placeholders)
            {
                html = html.Replace($"{{{{{pair.Key}}}}}", pair.Value);
            }

            return html;
        }

    }
}
