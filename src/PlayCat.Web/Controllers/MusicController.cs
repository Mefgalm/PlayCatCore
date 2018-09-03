using System;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using PlayCat.Music;

namespace PlayCat.Web.Controllers
{
    [Route("api/[controller]")]
    public class MusicController : BaseController
    {
        private readonly IFileResolver _fileResolver;

        public MusicController(IFileResolver fileResolver)
        {
            _fileResolver = fileResolver;
        }

        [HttpGet("song/{filename}/storageType/fileSytem")]
        public FileStreamResult Song(string filename)
        {
            //var result = _authService.CheckToken(AccessToken);

            string contentType = "audio/mpeg";

            string audioFolderPath = _fileResolver.GetAudioFolderPath(StorageType.FileSystem);
            
            byte[] song = System.IO.File.ReadAllBytes(Path.Combine(audioFolderPath, filename));
            long fSize = song.Length;
            long startbyte = 0;
            long endbyte = fSize - 1;
            int statusCode = 200;

            var rangeHeader = Request.Headers["Range"];
            if (Request.Headers.ContainsKey("Range"))
            {
                //Get the actual byte range from the range header string, and set the starting byte.
                string[] range = rangeHeader.ToString().Split(new char[] { '=', '-' });
                startbyte = Convert.ToInt64(range[1]);
                if (range.Length > 2 && range[2] != "") endbyte = Convert.ToInt64(range[2]);
                //If the start byte is not equal to zero, that means the user is requesting partial content.
                if (startbyte != 0 || endbyte != fSize - 1 || range.Length > 2 && range[2] == "")
                { statusCode = 206; }//Set the status code of the response to 206 (Partial Content) and add a content range header.                                    
            }
            long desSize = endbyte - startbyte + 1;
            //Headers
            Response.StatusCode = statusCode;

            Response.ContentType = contentType;
            Response.Headers.Add("Content-Accept", Response.ContentType);
            Response.Headers.Add("Content-Length", desSize.ToString());
            Response.Headers.Add("Content-Range", string.Format("bytes {0}-{1}/{2}", startbyte, endbyte, fSize));
            //Data

            var stream = new MemoryStream(song, (int)startbyte, (int)desSize);

            return new FileStreamResult(stream, Response.ContentType);
        }
    }
}
