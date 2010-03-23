// Copyright (c) 2009 Chris Pietschmann (http://pietschsoft.com)
// All rights reserved.
// Licensed under the Microsoft Public License (Ms-PL)
// http://opensource.org/licenses/ms-pl.html

using System.Web.Mvc;

namespace ScriptManager
{
    public static class ScriptManagerExtensions
    {
        private static readonly string simpleScriptManagerKey = "ScriptManager";

        public static ScriptManager ScriptManager(this HtmlHelper helper)
        {
            // Get ScriptManager from HttpContext.Items
            // This allows for a single ScriptManager to be created and used per HTTP request.
            var scriptmanager = helper.ViewContext.HttpContext.Items[simpleScriptManagerKey] as ScriptManager;
            if (scriptmanager == null)
            {
                // If ScriptManager hasn't been initialized yet, then initialize it.
                scriptmanager = new ScriptManager(helper);
                // Store it in HttpContext.Items for subsequent requests during this HTTP request.
                helper.ViewContext.HttpContext.Items[simpleScriptManagerKey] = scriptmanager;
            }
            // Return the ScriptManager
            return scriptmanager;
        }
    }
}