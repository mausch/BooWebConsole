﻿using System;
using System.IO;
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
            var interpreter = new InteractiveInterpreter {
                Ducky = true,
            };
            foreach (var a in AppDomain.CurrentDomain.GetAssemblies())
                interpreter.References.Add(a);
            var output = new StringBuilder();
            var defaultOut = Console.Out;
            var newOut = new StringWriter(output);
            try {
                Console.SetOut(newOut);
                //interpreter.Print = o => output.Append(o); // doesn't work with the print macro
                interpreter.SetValue("context", context);
                interpreter.Declare("context", typeof(object));
                try {
                    var compilerContext = interpreter.Eval(ctx.Prg);
                    ctx.Errors = compilerContext.Errors.ToString(true);
                } catch (Exception e) {
                    ctx.Errors = e.ToString();
                }
                ctx.Output = output.ToString();
                return new ViewResult(ctx, ViewName);
            } finally {
                Console.SetOut(defaultOut);
                newOut.Dispose();
            }
        }
    }
}