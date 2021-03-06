﻿using System;
using System.Net;
using System.Runtime.InteropServices;

namespace PlayCat.Helpers
{
    public class Headers
    {
        public int ContentLenght { get; set; }
    }

    public static class HttpRequester
    {
        public static Headers GetHeaders(string url)
        {
            if (url == null)
                throw new ArgumentNullException(nameof(url));

            var webRequest = (HttpWebRequest) WebRequest.Create(url);
            var webResponse = (HttpWebResponse) webRequest.GetResponse();

            int contentLength = int.Parse(webResponse.Headers["Content-Length"]);

            webResponse.Close();

            return new Headers
            {
                ContentLenght = contentLength
            };
        }
    }
}