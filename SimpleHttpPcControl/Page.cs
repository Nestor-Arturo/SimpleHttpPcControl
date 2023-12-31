﻿using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace SimpleHttpPcControl
{
    /// <summary>
    /// Handle home page html.
    /// </summary>
    internal class Page
    {
        /// <summary>
        /// Returns the html for the home page.
        /// </summary>
        /// <returns></returns>
        static internal string GetIndexPageHtml()
        {
            var Result = new StringBuilder();
            Result.Append(
                TransformHtmlContentPart(
                    File.ReadAllText(Common.GetCurrentExecutionFolder("data", "Index-head.html"))));
            Result.Append(GetIndexPageActions());
            Result.Append(
                TransformHtmlContentPart(
                    File.ReadAllText(Common.GetCurrentExecutionFolder("data", "Index-foot.html"))));
            return Result.ToString();
        }

        /// <summary>
        /// Replaces other information in the html content.
        /// </summary>
        /// <param name="htmlTemplate"></param>
        /// <returns></returns>
        static StringBuilder TransformHtmlContentPart(string htmlTemplate)
        {
            var Result = new StringBuilder(htmlTemplate);
            var ExpTemplateReplacements = new Regex(@"{{server-name}}", RegexOptions.Singleline);
            var AllReplacementsMatches = ExpTemplateReplacements.Matches(htmlTemplate).Reverse();
            foreach (var Replace in AllReplacementsMatches)
            {
                Result.Remove(Replace.Index, Replace.Length);
                Result.Insert(Replace.Index, Common.Config.Server.Name);
            }
            return Result;
        }

        /// <summary>
        /// Build the html for the actions buttons.
        /// </summary>
        /// <returns></returns>
        static StringBuilder GetIndexPageActions()
        {
            var Result = new StringBuilder();
            var ActionHtmlTemplate = File.ReadAllText(Common.GetCurrentExecutionFolder("data", "Index-action.html"));
            var ExpTemplateReplacements = new Regex(@"{{action-(?<name>[^}]+)}}");
            var AllReplacementsMatches = ExpTemplateReplacements.Matches(ActionHtmlTemplate).Reverse();

            foreach (var Action in Common.Config.Actions)
            {
                var ActionItem = new StringBuilder(ActionHtmlTemplate);
                foreach (var Replace in AllReplacementsMatches)
                {
                    ActionItem.Remove(Replace.Index, Replace.Length);
                    ActionItem.Insert(Replace.Index, Action.GetPropertyValueByName(Replace.Groups["name"].Value));
                }
                Result.Append(ActionItem);
            }
            return Result;
        }
    }
}
