using System;
using System.Text;
using System.Web;
using Boo.Lang.Interpreter;
using MiniMVC;

namespace BooWebConsole {
    public class IndexController : Controller {
        public override IResult Execute(HttpContext context) {
            var ctx = new Context {
                Prg = context.Request["prg"]
            };
            if (string.IsNullOrEmpty(ctx.Prg))
                return new ViewResult(ctx, ViewName);
            var interpreter = new InteractiveInterpreter();
            foreach (var a in AppDomain.CurrentDomain.GetAssemblies())
                interpreter.References.Add(a);
            var output = new StringBuilder();
            interpreter.Print = o => output.Append(o);
            var compilerContext = interpreter.Eval(ctx.Prg);
            ctx.Errors = compilerContext.Errors.ToString(true);
            ctx.Output = output.ToString();
            return new ViewResult(ctx, ViewName);
        }
    }
}