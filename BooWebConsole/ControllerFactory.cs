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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using Boo.Lang.Interpreter;
using BooWebConsole.Views;
using MiniMVC;

namespace BooWebConsole {
    public class ControllerFactory : HttpHandlerFactory {
        public override IHttpHandler GetHandler(HttpContextBase context) {
            var lastUrlSegment = context.Request.Url.Segments.Last().Split('.')[0];
            return routes.Where(k => k.Key == lastUrlSegment).Select(k => k.Value).FirstOrDefault();
        }

        public static KeyValuePair<K, V> KV<K, V>(K key, V value) {
            return new KeyValuePair<K, V>(key, value);
        }

        private readonly IEnumerable<KeyValuePair<string, IHttpHandler>> routes =
            new[] {
                KV("suggest", SuggestHandler),
                KV("index", IndexHandler),
                KV("static", StaticHandler),
            };

        public static readonly IHttpHandler SuggestHandler = new HttpHandlerWithReadOnlySession(Suggest);

        public static void Suggest(HttpContextBase context) {
            var q = context.Request["q"];
            string response = GetSuggestions(q);
            context.Raw(response, contentType: "application/json");
        }

        public static string GetSuggestions(string q) {
            var interpreter = NewInterpreter();
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

        public static AbstractInterpreter NewInterpreter() {
            var interpreter = new InteractiveInterpreter {
                Ducky = true,
            };
            foreach (var a in AppDomain.CurrentDomain.GetAssemblies())
                interpreter.References.Add(a);
            return interpreter;
        }

        public static AbstractInterpreter BuildInterpreter(HttpContextBase context) {
            var interpreter = NewInterpreter();
            //interpreter.Print = o => output.Append(o); // doesn't work with the print macro
            interpreter.SetValue("context", context);
            interpreter.Declare("context", typeof(object));
            return interpreter;
        }

        private static void IndexResult(HttpContextBase context, Context ctx) {
            context.XDocument(Views.Views.Index(ctx).MakeHTML5Doc());
        }

        public static readonly IHttpHandler IndexHandler = new HttpHandlerWithReadOnlySession(Index);

        public static void Index(HttpContextBase context) {
            var ctx = new Context {
                Prg = context.Request["prg"]
            };
            if (string.IsNullOrEmpty(ctx.Prg)) {
                IndexResult(context, ctx);
                return;
            }
            var output = new StringBuilder();
            var defaultOut = Console.Out;
            var newOut = new StringWriter(output);
            try {
                Console.SetOut(newOut);
                var interpreter = BuildInterpreter(context);
                try {
                    var compilerContext = interpreter.Eval(ctx.Prg);
                    ctx.Errors = compilerContext.Errors.ToString(true);
                } catch (Exception e) {
                    ctx.Errors = e.ToString();
                }
                ctx.Output = output.ToString();
                IndexResult(context, ctx);
            } finally {
                Console.SetOut(defaultOut);
                newOut.Dispose();
            }
        }

        public static readonly IHttpHandler StaticHandler = new HttpHandler(Static);

        public static void Static(HttpContextBase context) {
            var resource = context.Request.QueryString["r"];
            var assembly = typeof (ControllerFactory).Assembly;
            var fullResourceName = string.Format("{0}.Resources.{1}", assembly.FullName.Split(',')[0], resource);
            using (var resourceStream = assembly.GetManifestResourceStream(fullResourceName))
                Copy(resourceStream, context.Response.OutputStream);
        }

        public static void Copy(Stream source, Stream dest) {
            const int size = 32768;
            var buffer = new byte[size];
            var read = 0;
            while ((read = source.Read(buffer, 0, size)) > 0)
                dest.Write(buffer, 0, read);
        }

    }
}