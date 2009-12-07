using System;
using System.Linq;
using System.Web;
using Boo.Lang.Interpreter;
using MiniMVC;

namespace BooWebConsole {
    public class SuggestController: Controller {
        public override IResult Execute(HttpContext context) {
            var interpreter = new InteractiveInterpreter();
            foreach (var a in AppDomain.CurrentDomain.GetAssemblies())
                interpreter.References.Add(a);
            var suggestions = interpreter.SuggestCodeCompletion(context.Request["code"])
                .Select(c => c.Name)
                .Distinct()
                .OrderBy(x => x)
                .Select(c => string.Format("'{0}'", c))
                .ToArray();
            return new RawResult(string.Format("[{0}]", string.Join(",", suggestions))) {
                ContentType = "application/json"
            };
        }
    }
}