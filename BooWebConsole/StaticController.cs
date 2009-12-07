using System.Web;
using MiniMVC;

namespace BooWebConsole {
    public class StaticController : Controller {
        public override IResult Execute(HttpContext context) {
            var resource = context.Request.QueryString["r"];
            var fullResourceName = string.Format("{0}.Resources.{1}", GetType().Assembly.FullName.Split(',')[0], resource);
            var resourceStream = GetType().Assembly.GetManifestResourceStream(fullResourceName);
            const int size = 32768;
            var buffer = new byte[size];
            var read = 0;
            while ((read = resourceStream.Read(buffer, 0, size)) > 0)
                context.Response.OutputStream.Write(buffer, 0, read);
            return new EmptyResult();
        }

        public class EmptyResult : IResult {
            public void Execute(HttpContext context) {}
        }
    }
}