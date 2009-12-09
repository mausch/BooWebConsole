#region license
// Copyright (c) 2009 Mauricio Scheffer
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//      http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using System;
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