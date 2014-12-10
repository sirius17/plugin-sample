using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.InterceptionExtension;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Plugins.WebHooks
{
    public class WebHookFilter : VexiereAspect
    {

        protected override bool OnBeforeExecution(IInputArguments arguments, out object returnValue)
        {
            returnValue = null;
            return false;
        }

        protected override MethodSignature GetMatchingMethod()
        {
            var signature = new MethodSignature
            {
                TargetType = typeof(IAction),
                Name = "Create"
            };
            signature.Parameters.Add(typeof(User));
            return signature;
        }

        protected override bool OnAfterExecution(IInputArguments args, object output, out object newOutput)
        {
            newOutput = null;
            var user = args[0] as User;
            PostCreatedUser(user);
            return false;
        }

        private void PostCreatedUser(User user)
        {
            Console.WriteLine("Posting to web hook.");
            // Post async
            Task.Factory.StartNew(() =>
                {
                    var json = new JObject(
                        new JProperty("id", user.Id),
                        new JProperty("username", user.UserName),
                        new JProperty("firstName", user.Name.FirstName),
                        new JProperty("lastName", user.Name.LastName));
                    byte[] data = null;
                    using (var buffer = new MemoryStream())
                    {
                        using (var streamWriter = new StreamWriter(buffer, Encoding.UTF8))
                        {
                            using (var jsonWriter = new JsonTextWriter(streamWriter))
                            {
                                json.WriteTo(jsonWriter);
                                
                            }
                        }
                        data = buffer.ToArray();
                    }

                    System.Net.WebClient wc = new System.Net.WebClient();
                    wc.Headers[HttpRequestHeader.ContentType] = "application/json";
                    wc.UploadData(Configuration.PostUrl, "POST", data);
                });
        }
    }
}
