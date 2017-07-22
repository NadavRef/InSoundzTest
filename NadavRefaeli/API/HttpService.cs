using System;
using System.IO;
using System.Net;
using System.Threading;
using Newtonsoft.Json;

namespace Homework1.API
{
    public class HttpService
    {
        private readonly HttpListener _httpListener;
        private readonly string _serverUrl;
        private bool _stopRequested;

        private HttpService(int portToListen)
        {
            _httpListener = new HttpListener();
            _serverUrl = $"{Constants.UrlPrefix}{portToListen}/";
            _httpListener.Prefixes.Add(_serverUrl);
        }

        public static HttpService CreateNew(Configuration configuration)
        {
            var service = new HttpService(configuration.Port);
            return service;
        }

        public void StartService()
        {
            _httpListener.Start();
            while (!_stopRequested)
            {
                try
                {
                    var context = _httpListener.GetContext();
                    ThreadPool.QueueUserWorkItem(r => HandleRequest(context));
                }
                catch (Exception)
                {
                    // Send unicorns to space 
                }
            }
        }

        private void HandleRequest(HttpListenerContext context)
        {
            var command = context.Request.RawUrl;
            switch (command)
            {
                case Constants.InitalizeWebCommand: HandleInitalizeWebCommand(context);   break;
                case Constants.GenerateMovieCommand: HandleGenerateMovieCommand(context); break;
                default:
                    if (command.Contains(Constants.GetMovieCommand))
                    {
                        HandleGetMovieCommand(context);
                        return;
                    }
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;  
                    context.Response.StatusDescription = "Command Does Not Exists";
                    break;
            }
        }

        private void HandleGetMovieCommand(HttpListenerContext context)
        {
            var requestedPath = GetPathFromRequest(context.Request);
            var moviePath = string.Concat(Hollywood.Output, requestedPath);
            var response = context.Response;
            using (var fs = File.OpenRead(moviePath))
            {
                var filename = Path.GetFileName(moviePath);
                response.ContentLength64 = fs.Length;
                response.SendChunked = false;
                response.AddHeader("Access-Control-Allow-Origin", "*");
                response.ContentType = System.Net.Mime.MediaTypeNames.Application.Octet;
                response.AddHeader("Content-disposition", "attachment; filename=" + filename);

                var buffer = new byte[64 * 1024];
                using (var bw = new BinaryWriter(response.OutputStream))
                {
                    int read;
                    while ((read = fs.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        bw.Write(buffer, 0, read);
                        bw.Flush();
                    }

                    bw.Close();
                }

                response.StatusCode = (int)HttpStatusCode.OK;
                response.StatusDescription = "OK";
                response.OutputStream.Close();
            }
        }

        private string GetPathFromRequest(HttpListenerRequest contextRequest)
        {
            var query = contextRequest.RawUrl;
            return query.Remove(0, query.LastIndexOf("/", StringComparison.Ordinal) + 1);
        }

        private static void HandleInitalizeWebCommand(HttpListenerContext context)
        {
            var initalizationData = WebInitalizationData.GetData();
            var jsonData = JsonConvert.SerializeObject(initalizationData);
            var bytes = System.Text.Encoding.UTF8.GetBytes(jsonData);

            context.Response.ContentType = Constants.DefaultContentType;
            context.Response.ContentLength64 = bytes.Length;
            context.Response.StatusCode = (int)HttpStatusCode.OK;
            context.Response.AddHeader("Access-Control-Allow-Origin", "*");
            context.Response.OutputStream.Write(bytes, 0, bytes.Length);
            context.Response.OutputStream.Close();
        }

        public void HandleGenerateMovieCommand(HttpListenerContext context)
        {
            var enteringRequest = context.Request;
            if (!enteringRequest.HasEntityBody)
            {
                // Send unicorns to space 
                return;
            }

            string bodyAsString;
            using (var bodyStream = enteringRequest.InputStream)
            {
                using (var reader = new StreamReader(bodyStream, enteringRequest.ContentEncoding))
                {
                    bodyAsString = reader.ReadToEnd();
                }
            }

            if (string.IsNullOrWhiteSpace(bodyAsString))
            {
                // Send unicorns to space 
                return;
            }

            try
            {
                var makeMovieRequest = JsonConvert.DeserializeObject<MakeMovieRequest>(bodyAsString);
                if (!makeMovieRequest.IsValid)
                {
                    // Send unicorns to space 
                    return;
                }

                var moviePath = Hollywood.CreateNewMovie(makeMovieRequest);
                CreateMovieResponse(context, moviePath);
            }
            catch (Exception e)
            {
                // Send unicorns to space 
            }
        }

        private void CreateMovieResponse(HttpListenerContext context, string movieName)
        {
            var url = _serverUrl + Constants.GetMovieCommand +"/" + movieName;
            var json = JsonConvert.SerializeObject(url);
            var bytes = System.Text.Encoding.UTF8.GetBytes(json);

            context.Response.ContentType = Constants.DefaultContentType;
            context.Response.ContentLength64 = bytes.Length;
            context.Response.StatusCode = (int)HttpStatusCode.OK;
            context.Response.AddHeader("Access-Control-Allow-Origin", "*");
            context.Response.OutputStream.Write(bytes, 0, bytes.Length);
            context.Response.OutputStream.Close();
        }

        public void StopService()
        {
            _httpListener.Stop();
            _stopRequested = true;
        }
    }
}
