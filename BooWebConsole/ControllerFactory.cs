using System;
using System.Linq;
using System.Web;

namespace BooWebConsole {
    public class ControllerFactory : IHttpHandlerFactory {
        public IHttpHandler GetHandler(HttpContext context, string requestType, string url, string pathTranslated) {
            var name = context.Request.RawUrl.Split('/').Last().Split('?')[0].Split('.')[0];
            name = name.ToUpperInvariant().First() + new string(name.ToLowerInvariant().Skip(1).ToArray());
            return (IHttpHandler) Activator.CreateInstance(Type.GetType(string.Format("BooWebConsole.{0}Controller", name)));
        }

        public void ReleaseHandler(IHttpHandler handler) {}
    }
}