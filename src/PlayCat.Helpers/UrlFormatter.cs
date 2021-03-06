﻿using System;
using System.Text.RegularExpressions;

namespace PlayCat.Helpers
{
    public static class UrlFormatter
    {
        private const string ParemeterRegexp = @"&[-A-Za-z0-9_]+=[-A-Za-z0-9_\.]+";

        public static string RemoveParametersFromUrl(string url)
        {
            if (url == null)
                return null;

            var regexp = new Regex(ParemeterRegexp);

            return regexp.Replace(url, string.Empty);
        }

        public static string GetYoutubeVideoIdentifier(string url)
        {
            if (url == null)
                throw new ArgumentNullException(nameof(url));

            url = RemoveParametersFromUrl(url);

            int idIndex = url.LastIndexOf('=');

            if (idIndex < 0 && url.Contains("youtu.be"))
                idIndex = url.LastIndexOf('/');

            if (idIndex > 0)
                return url.Substring(idIndex + 1);

            throw new Exception("Wrong youtube url link format");
        }
    }
}
