using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using YotubeLinkerCore;

namespace PlayCat.Music.Youtube
{
    public class YoutubeLinkerCore : IYotubeLinkerCore
    {
        private const string RateBypassFlag = "ratebypass";
        private const string SignatureQuery = "signature";

        private static bool IsVideoUnavailable(string pageSource)
        {
            const string unavailableContainer = "<div id=\"watch-player-unavailable\">";

            return pageSource.Contains(unavailableContainer);
        }

        private static async Task<JObject> LoadJsonAsync(string url)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    var pageSource = await httpClient.GetStringAsync(url);

                    if (IsVideoUnavailable(pageSource))
                    {
                        throw new VideoNotAvailableException();
                    }

                    var dataRegex = new Regex(@"ytplayer\.config\s*=\s*(\{.+?\});", RegexOptions.Multiline);

                    var extractedJson = dataRegex.Match(pageSource).Result("$1");

                    return JObject.Parse(extractedJson);
                }
            }
            catch
            {
                throw new Exception(url);
            }
        }

        private static string GetStreamMap(JObject json)
        {
            var streamMap = json["args"]["url_encoded_fmt_stream_map"];

            var streamMapString = streamMap?.ToString();

            if (streamMapString == null || streamMapString.Contains("been+removed"))
            {
                throw new VideoNotAvailableException("Video is removed or has an age restriction.");
            }

            return streamMapString;
        }

        private static string GetAdaptiveStreamMap(JObject json)
        {
            // bugfix: adaptive_fmts is missing in some videos, use url_encoded_fmt_stream_map instead
            var streamMap = json["args"]["adaptive_fmts"] ?? json["args"]["url_encoded_fmt_stream_map"];

            return streamMap.ToString();
        }

        private static IEnumerable<ExtractionInfo> ExtractDownloadUrls(JObject json)
        {
            var splitByUrls            = GetStreamMap(json).Split(',');
            var adaptiveFmtSplitByUrls = GetAdaptiveStreamMap(json).Split(',');
            splitByUrls = splitByUrls.Concat(adaptiveFmtSplitByUrls).ToArray();

            foreach (var s in splitByUrls)
            {
                var queries = new HttpHelper().ParseQueryString(s);

                string url;

                var requiresDecryption = false;

                if (queries.ContainsKey("s") || queries.ContainsKey("sig"))
                {
                    requiresDecryption = queries.ContainsKey("s");
                    var signature = queries.ContainsKey("s") ? queries["s"] : queries["sig"];

                    url = string.Format("{0}&{1}={2}", queries["url"], SignatureQuery, signature);

                    var fallbackHost = queries.ContainsKey("fallback_host")
                                           ? "&fallback_host=" + queries["fallback_host"]
                                           : String.Empty;

                    url += fallbackHost;
                }

                else
                {
                    url = queries["url"];
                }

                url = new HttpHelper().UrlDecode(url);
                url = new HttpHelper().UrlDecode(url);

                var parameters = new HttpHelper().ParseQueryString(url);
                if (!parameters.ContainsKey(RateBypassFlag))
                    url += string.Format("&{0}={1}", RateBypassFlag, "yes");

                yield return new ExtractionInfo {RequiresDecryption = requiresDecryption, Uri = new Uri(url)};
            }
        }

        private static bool TryNormalizeYoutubeUrl(string url, out string normalizedUrl)
        {
            url = url.Trim();

            url = url.Replace("youtu.be/", "youtube.com/watch?v=");
            url = url.Replace("www.youtube", "youtube");
            url = url.Replace("youtube.com/embed/", "youtube.com/watch?v=");

            if (url.Contains("/v/"))
            {
                url = "http://youtube.com" + new Uri(url).AbsolutePath.Replace("/v/", "/watch?v=");
            }

            url = url.Replace("/watch#", "/watch?");

            var query = new HttpHelper().ParseQueryString(url);

            if (!query.TryGetValue("v", out var v))
            {
                normalizedUrl = null;
                return false;
            }

            normalizedUrl = "http://youtube.com/watch?v=" + v;

            return true;
        }

        private static string GetHtml5PlayerVersion(JObject json)
        {
            var regex = new Regex(@"player-(.+?).js");

            var js = json["assets"]["js"].ToString();

            return regex.Match(js).Result("$1");
        }

        private static string GetVideoTitle(JObject json)
        {
            var title = json["args"]["title"];

            return title == null ? string.Empty : title.ToString();
        }

        private static IEnumerable<VideoInfo> GetVideoInfos(IEnumerable<ExtractionInfo> extractionInfos,
                                                            string videoTitle)
        {
            var downLoadInfos = new List<VideoInfo>();

            foreach (var extractionInfo in extractionInfos)
            {
                var itag = new HttpHelper().ParseQueryString(extractionInfo.Uri.Query)["itag"];

                var formatCode = int.Parse(itag);

                var info = VideoInfo.Defaults.SingleOrDefault(videoInfo => videoInfo.FormatCode == formatCode);

                if (info != null)
                {
                    info = new VideoInfo(info)
                    {
                        DownloadUrl        = extractionInfo.Uri.ToString(),
                        Title              = videoTitle,
                        RequiresDecryption = extractionInfo.RequiresDecryption
                    };
                }

                else
                {
                    info = new VideoInfo(formatCode)
                    {
                        DownloadUrl = extractionInfo.Uri.ToString()
                    };
                }

                downLoadInfos.Add(info);
            }

            return downLoadInfos;
        }

        public async Task<IEnumerable<VideoInfo>> GetDownloadUrlsAsync(string videoUrl)
        {
            if (videoUrl == null)
                throw new ArgumentNullException(nameof(videoUrl));

            var isYoutubeUrl = TryNormalizeYoutubeUrl(videoUrl, out videoUrl);

            if (!isYoutubeUrl)
            {
                throw new ArgumentException("URL is not a valid youtube URL!");
            }

            try
            {
                var json = await LoadJsonAsync(videoUrl);

                var jsFileUrl = GetJsFileUrl(json);

                var videoTitle = GetVideoTitle(json);

                var downloadUrls = ExtractDownloadUrls(json);

                IEnumerable<VideoInfo> infos = GetVideoInfos(downloadUrls, videoTitle).ToList();

                //var htmlPlayerVersion = GetHtml5PlayerVersion(json);

                foreach (var info in infos)
                {
                    //info.HtmlPlayerVersion = htmlPlayerVersion;

                    if (info.RequiresDecryption)
                    {
                        await DecryptDownloadUrlAsync(info, jsFileUrl);
                    }
                }

                return infos;
            }

            catch (Exception ex)
            {
                if (ex is WebException || ex is VideoNotAvailableException)
                {
                    throw;
                }

                ThrowYoutubeParseException(ex, videoUrl);
            }

            return null; // Will never happen, but the compiler requires it
        }

        private static string GetJsFileUrl(JObject jObject)
        {
            return jObject["assets"]["js"].ToString();
        }

        private static void ThrowYoutubeParseException(Exception innerException, string videoUrl)
        {
            throw new YoutubeParseException("Could not parse the Youtube page for URL " + innerException.Message,
                innerException);
        }

        private static async Task DecryptDownloadUrlAsync(VideoInfo videoInfo, string jsFileUrl)
        {
            var queries = new HttpHelper().ParseQueryString(videoInfo.DownloadUrl);

            if (!queries.ContainsKey(SignatureQuery)) return;

            var encryptedSignature = queries[SignatureQuery];

            string decrypted;

            try
            {
                decrypted = await DecipherWithVersionAsync(encryptedSignature, jsFileUrl);
            }

            catch (Exception ex)
            {
                throw new YoutubeParseException("Could not decipher signature", ex);
            }

            videoInfo.DownloadUrl =
                new HttpHelper().ReplaceQueryStringParameter(videoInfo.DownloadUrl, SignatureQuery, decrypted);
            videoInfo.RequiresDecryption = false;
        }

        private static async Task<string> DecipherWithVersionAsync(string cipher, string jsFileUrl)
        {
            //const string jsUrl = "https://s.ytimg.com/yts/jsbin/player-vfl-Sv0Xf/ru_RU/base.js";
            var js = await new HttpHelper().DownloadStringAsync("https://s.ytimg.com" + jsFileUrl);

            //Find "C" in this: var A = B.sig||C (B.s)
            const string
                functNamePattern = @"\""signature"",\s?([a-zA-Z0-9\$]+)\("; //Regex Formed To Find Word or DollarSign

            var funcName = Regex.Match(js, functNamePattern).Groups[1].Value;

            if (funcName.Contains("$"))
            {
                funcName = "\\" + funcName; //Due To Dollar Sign Introduction, Need To Escape
            }

            var funcPattern = @"(?!h\.)" + @funcName + @"=function\(\w+\)\{.*?\}";         //Escape funcName string
            var funcBody    = Regex.Match(js, funcPattern, RegexOptions.Singleline).Value; //Entire sig function
            var lines       = funcBody.Split(';');                                         //Each line in sig function

            string idReverse = "", idSlice = "", idCharSwap = ""; //Hold name for each cipher method
            string functionIdentifier;
            var    operations = "";

            foreach (var line in lines.Skip(1).Take(lines.Length - 2)
            ) //Matches the funcBody with each cipher method. Only runs till all three are defined.
            {
                if (!string.IsNullOrEmpty(idReverse) && !string.IsNullOrEmpty(idSlice) &&
                    !string.IsNullOrEmpty(idCharSwap))
                {
                    break; //Break loop if all three cipher methods are defined
                }

                functionIdentifier = GetFunctionFromLine(line);
                var reReverse =
                    string.Format(@"{0}:\bfunction\b\(\w+\)", functionIdentifier); //Regex for reverse (one parameter)
                var reSlice =
                    string.Format(@"{0}:\bfunction\b\([a],b\).(\breturn\b)?.?\w+\.",
                        functionIdentifier); //Regex for slice (return or not)
                var reSwap =
                    string.Format(@"{0}:\bfunction\b\(\w+\,\w\).\bvar\b.\bc=a\b",
                        functionIdentifier); //Regex for the char swap.

                if (Regex.Match(js, reReverse).Success)
                {
                    idReverse =
                        functionIdentifier; //If def matched the regex for reverse then the current function is a defined as the reverse
                }

                if (Regex.Match(js, reSlice).Success)
                {
                    idSlice =
                        functionIdentifier; //If def matched the regex for slice then the current function is defined as the slice.
                }

                if (Regex.Match(js, reSwap).Success)
                {
                    idCharSwap =
                        functionIdentifier; //If def matched the regex for charSwap then the current function is defined as swap.
                }
            }

            foreach (var line in lines.Skip(1).Take(lines.Length - 2))
            {
                Match m;
                functionIdentifier = GetFunctionFromLine(line);

                if ((m = Regex.Match(line, @"\(\w+,(?<index>\d+)\)")).Success && functionIdentifier == idCharSwap)
                {
                    operations += "w" + m.Groups["index"].Value + " "; //operation is a swap (w)
                }

                if ((m = Regex.Match(line, @"\(\w+,(?<index>\d+)\)")).Success && functionIdentifier == idSlice)
                {
                    operations += "s" + m.Groups["index"].Value + " "; //operation is a slice
                }

                if (functionIdentifier == idReverse) //No regex required for reverse (reverse method has no parameters)
                {
                    operations += "r "; //operation is a reverse
                }
            }

            operations = operations.Trim();

            return DecipherWithOperations(cipher, operations);
        }

        private static string ApplyOperation(string cipher, string op)
        {
            switch (op[0])
            {
                case 'r':
                    return new string(cipher.ToCharArray().Reverse().ToArray());

                case 'w':
                {
                    var index = GetOpIndex(op);
                    return SwapFirstChar(cipher, index);
                }

                case 's':
                {
                    var index = GetOpIndex(op);
                    return cipher.Substring(index);
                }

                default:
                    throw new NotImplementedException("Couldn't find cipher operation.");
            }
        }

        private static string DecipherWithOperations(string cipher, string operations)
        {
            return operations.Split(new[] {" "}, StringSplitOptions.RemoveEmptyEntries)
                             .Aggregate(cipher, ApplyOperation);
        }

        private static string GetFunctionFromLine(string currentLine)
        {
            var matchFunctionReg = new Regex(@"\w+\.(?<functionID>\w+)\("); //lc.ac(b,c) want the ac part.
            var rgMatch          = matchFunctionReg.Match(currentLine);
            var matchedFunction  = rgMatch.Groups["functionID"].Value;
            return matchedFunction; //return 'ac'
        }

        private static int GetOpIndex(string op)
        {
            var parsed = new Regex(@".(\d+)").Match(op).Result("$1");
            var index  = int.Parse(parsed);

            return index;
        }

        private static string SwapFirstChar(string cipher, int index)
        {
            var builder = new StringBuilder(cipher)
            {
                [0]     = cipher[index],
                [index] = cipher[0]
            };

            return builder.ToString();
        }
    }
}