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
    public static class Program
    {

        public static void Main(string[] args)
        {
            UnityContainer container = new UnityContainer();
            container.AddNewExtension<Interception>();
            container.AddNewExtension<AspectRegistrationExtension>();
            container.RegisterType<IAction, NopAction>(
                new InterceptionBehavior<PolicyInjectionBehavior>(),
                new Interceptor<InterfaceInterceptor>());

            var action = container.Resolve<IAction>();
            action.Do("test" as object);
            action.Do2();
            action.Create(new User { Id = "123", UserName = "john.doe", Name = new UserName { FirstName = "John", LastName = "Doe" } });
            
            Console.WriteLine("Press any key to end..");
            Console.ReadKey(true);
        }
    }

    public class User
    {
        public string Id { get; set; }

        public string UserName { get; set; }

        public UserName Name { get; set; }
    }

    public class UserName
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }
    }

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

    public interface IAction
    {
        void Do(object target);

        void Do2();

        void Create(User user);
    }

    public class NopAction : IAction
    {
        public void Do(object target)
        {
            Console.WriteLine("Doing nothing.");
            // Do nothing
        }

        public void Do2()
        {
            Console.WriteLine("Doing nothing.");
            // Do nothing
        }

        public void Create(User user)
        {
            // Created user
            Console.WriteLine("Created the user.");
        }
    }

    public static class Configuration
    {
        public static string PostUrl
        {
            get { return ConfigurationManager.AppSettings["post-url"] ?? "http://requestb.in/ptyy7ipt"; }
        }
    }

    public class PrinterAspect : VexiereAspect
    {
        protected override bool OnBeforeExecution(IInputArguments arguments, out object returnValue)
        {
            returnValue  = null;
            Console.WriteLine("Before aspect.");
            // Console.WriteLine("Argument is a {0}.", arguments[0].GetType().Name);
            return false;
        }

        protected override bool OnAfterExecution(IInputArguments args, object output, out object newOutput)
        {
            newOutput = null;
            Console.WriteLine("After aspect.");
            return false;
        }

        protected override MethodSignature GetMatchingMethod()
        {
            var method = new MethodSignature
            {
                TargetType = typeof(IAction),
                Name = "Create"
            };
            method.Parameters.Add(typeof(User));
            return method;
        }
    }


}
