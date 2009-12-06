using System;
using System.Text;
using System.Web;
using Boo.Lang.Interpreter;
using MiniMVC;

namespace BooWebConsole {
    public class IndexController : Controller {
        public override IResult Execute(HttpContext context) {
            var prg = context.Request["prg"];
            var viewModel = new ViewModel();
            if (string.IsNullOrEmpty(prg))
                return new ViewResult(viewModel, ViewName);
            var interpreter = new InteractiveInterpreter();
            foreach (var a in AppDomain.CurrentDomain.GetAssemblies())
                interpreter.References.Add(a);
            var output = new StringBuilder();
            interpreter.Print = o => output.Append(o);
            var ctx = interpreter.Eval(prg);
            viewModel.Output = output.ToString();
            return new ViewResult(viewModel, ViewName);
        }
    }
}