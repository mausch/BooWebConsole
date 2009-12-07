using System;
using System.Linq;
using System.Web;
using Boo.Lang.Interpreter;
using MiniMVC;

namespace BooWebConsole {
    public class SuggestController: Controller {
        public override IResult Execute(HttpContext context) {
            var q = context.Request["q"];
            string response = GetSuggestions(q);
            return new RawResult(response) {
                ContentType = "application/json"
            };
        }

        public string GetSuggestions(string q) {
            var interpreter = new InteractiveInterpreter();
            foreach (var a in AppDomain.CurrentDomain.GetAssemblies())
                interpreter.References.Add(a);
            var suggestions = new string[0];
            if (!string.IsNullOrEmpty(q)) {
                suggestions = interpreter.SuggestCodeCompletion(q)
                    .Select(c => c.Name)
                    .Distinct()
                    .OrderBy(x => x)
                    .Select(c => string.Format("'{0}'", c))
                    .ToArray();
            }
            return string.Format("[{0}]", string.Join(",", suggestions));
        }
    }
}