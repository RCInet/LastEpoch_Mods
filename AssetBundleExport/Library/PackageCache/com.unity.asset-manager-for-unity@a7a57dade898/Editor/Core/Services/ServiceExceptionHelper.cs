using System;
using System.Net;
using Unity.Cloud.CommonEmbedded;

namespace Unity.AssetManager.Core.Editor
{
    internal class ServiceExceptionInfo
    {
        public string Detail;
        public string Message;
        public HttpStatusCode? StatusCode;

        public ServiceExceptionInfo(string detail, string message, HttpStatusCode? status)
        {
            Detail = detail;
            Message = message;
            StatusCode = status;
        }
    }

    internal static class ServiceExceptionHelper
    {
        public static ServiceExceptionInfo GetServiceExceptionInfo(Exception e)
        {
            if (e is ServiceException serviceException)
            {
                return new ServiceExceptionInfo(serviceException.Detail, serviceException.Message, serviceException.StatusCode);
            }

            return null;
        }
    }
}
