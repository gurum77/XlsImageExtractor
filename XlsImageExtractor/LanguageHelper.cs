using NGettext;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace XlsImageExtractor
{
    static public class LanguageHelper
    {
        static private Dictionary<string, ICatalog> loadedCataloges = new Dictionary<string, ICatalog>();

        // 마지막으로 load된 catalog
        static private ICatalog lastCatalog = null;
        static public string defaultLanguage = "en-US";
        static public string currentLanguage = "en-US";

        public static string Tr(string text)
        {
            if (lastCatalog == null)
                return text;

            return lastCatalog.GetString(text);
        }

        // ko-KR, en-US
        static public void Load(string code)
        {
            currentLanguage = code;
            if (code == defaultLanguage)
            {
                lastCatalog = null;
                return;
            }

            if (loadedCataloges.ContainsKey(code))
            {
                lastCatalog = loadedCataloges[code];
                return;
            }

            var i18nPath = Path.Combine(GetExePath(), "i18n");
            ICatalog catalog = new Catalog("XlsImageExtractor", i18nPath, new CultureInfo(code));
            loadedCataloges.Add(code, catalog);
            lastCatalog = catalog;
        }

        // exe path
        static public string GetExePath()
        {
            return AppDomain.CurrentDomain.BaseDirectory;
        }


    }
}
