﻿using System;
using Server.Core;

namespace FileServer.Core
{
    public class InlineTextDocService : IHttpServiceProcessor
    {
        public bool CanProcessRequest(string request,
            ServerProperties serverProperties)
        {
            var readers = (Readers) serverProperties
                .ServiceSpecificObjectsWrapper;
            var requestItem = CleanRequest(request);
            return serverProperties.CurrentDir != null &&
                   readers.FileProcess
                       .Exists(serverProperties.CurrentDir
                               + requestItem)
                   && requestItem.EndsWith(".txt")
                   && request.Contains("GET /");
        }

        public IHttpResponse ProcessRequest(string request,
            IHttpResponse httpResponse,
            ServerProperties serverProperties)
        {
            var readers = (Readers) serverProperties
                .ServiceSpecificObjectsWrapper;
            var requestItem = CleanRequest(request);
            httpResponse.HttpStatusCode = "200 OK";
            httpResponse.CacheControl = "no-cache";
            httpResponse.FilePath = serverProperties.CurrentDir
                                    + requestItem;
            httpResponse.Filename = requestItem.Remove(0,
                requestItem.LastIndexOf('/') + 1);
            httpResponse.ContentType = "text/plain";
            httpResponse.ContentDisposition = "inline";
            httpResponse.ContentLength = readers
                .FileProcess.FileSize(httpResponse.FilePath);
            return httpResponse;
        }

        private string CleanRequest(string request)
        {
            if (request.Contains("HTTP/1.1"))
                return request.Substring(request
                    .IndexOf("GET /", StringComparison.Ordinal) + 5,
                    request.IndexOf(" HTTP/1.1",
                        StringComparison.Ordinal) - 5)
                    .Replace("%20", " ");
            return request.Substring(request
                .IndexOf("GET /", StringComparison.Ordinal) + 5,
                request.IndexOf(" HTTP/1.0",
                    StringComparison.Ordinal) - 5)
                .Replace("%20", " ");
        }
    }
}