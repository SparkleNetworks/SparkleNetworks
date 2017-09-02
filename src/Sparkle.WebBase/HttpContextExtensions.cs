
namespace Sparkle.WebBase
{
    using Sparkle.EmailTemplates;
    using Sparkle.Infrastructure;
    using Sparkle.Infrastructure.Data;
    using Sparkle.Services.Internals;
    using Sparkle.Services.Main.Networks;
    using Sparkle.Services.Networks;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Diagnostics;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Text;
    using System.Web;
    using System.Web.SessionState;
    using HttpRequestContext = System.Web.Http.Controllers.HttpRequestContext;
    using HttpRequestMessage = System.Net.Http.HttpRequestMessage;

    /// <summary>
    /// Extension methods for the HttpContext.
    /// </summary>
    public static class HttpContextExtensions
    {
        private static Func<HttpContext, SessionStateBehavior> getSessionStateBehaviorMethod;
        private static FieldInfo getHttpContextMethod;

        /// <summary>
        /// Gets the DOMAIN LAYER INSTANCE associated with the current request.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="autoCreate">if true, will create a new instance if none exists</param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public static IServiceFactory GetNetworkServices(this HttpContext context, bool autoCreate = true)
        {
            return Get<IServiceFactory>(
                context.Items,
                () => context.GetSparkleApp().GetNewServiceFactory(new AspnetServiceCache(context.Cache)),
                autoCreate);
        }

        /// <summary>
        /// Gets the DOMAIN LAYER INSTANCE associated with the current request.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="autoCreate">if true, will create a new instance if none exists</param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public static IServiceFactory GetNetworkServices(this HttpContextBase context, bool autoCreate = true)
        {
            return Get<IServiceFactory>(
                context.Items,
                () => context.GetSparkleApp().GetNewServiceFactory(new AspnetServiceCache(context.Cache)),
                autoCreate);
        }

        /// <summary>
        /// Gets the DOMAIN LAYER INSTANCE associated with the current request.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="autoCreate">if true, will create a new instance if none exists</param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public static IServiceFactory GetNetworkServices(this HttpRequestMessage context, bool autoCreate = true)
        {
            var services = Get<IServiceFactory>(context.Properties, null, false);

            if (services == null)
            {
                AspnetServiceCache cache = null;
                if (context.Properties.ContainsKey("MS_HttpContext"))
                {
                    var httpContext = (System.Web.HttpContextWrapper)context.Properties["MS_HttpContext"];
                    if (httpContext != null)
                    {
                        cache = new AspnetServiceCache(httpContext.Cache);
                        services = httpContext.GetNetworkServices(autoCreate);
                        Set<IServiceFactory>(context.Properties, services);
                    }
                }
            }

            return services;
        }

        /// <summary>
        /// Destroys the DOMAIN LAYER INSTANCE associated with the current request.
        /// </summary>
        public static void ClearNetworkServices(this HttpContext context, bool autoCreate = true)
        {
            IServiceFactory oldValue;
            Reset<IServiceFactory>(context.Items, out oldValue, null);
        }

        /// <summary>
        /// Destroys the DOMAIN LAYER INSTANCE associated with the current request.
        /// </summary>
        public static void ClearNetworkServices(this HttpContextBase context, bool autoCreate = true)
        {
            IServiceFactory oldValue;
            Reset<IServiceFactory>(context.Items, out oldValue, null);
        }

        /// <summary>
        /// Gets the DOMAIN LAYER BOOTSTRAPPER associated with the current PROCESS.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="autoCreate">if true, will create a new instance if none exists</param>
        /// <returns></returns>
        public static SparkleNetworksApplication GetSparkleApp(this HttpContext context, bool autoCreate = true)
        {
            var key = typeof(SparkleNetworksApplication).Name;
            if (context.Application[key] is SparkleNetworksApplication)
            {
                return (SparkleNetworksApplication)context.Application[key];
            }
            else if (autoCreate)
            {
                var value = GetSparkleApp(context.Request.Url.Host);
                context.Application[key] = value;
                return value;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the DOMAIN LAYER BOOTSTRAPPER associated with the current PROCESS.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="autoCreate">if true, will create a new instance if none exists</param>
        /// <returns></returns>
        public static SparkleNetworksApplication GetSparkleApp(this HttpContextBase context, bool autoCreate = true)
        {
            var key = typeof(SparkleNetworksApplication).Name;
            if (context.Application[key] is SparkleNetworksApplication)
            {
                return (SparkleNetworksApplication)context.Application[key];
            }
            else if (autoCreate)
            {
                var value = GetSparkleApp(context.Request.Url.Host);
                context.Application[key] = value;
                return value;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Gets a DOMAIN LAYER BOOTSTRAPPER associated with a specified domain name.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static SparkleNetworksApplication GetSparkleApp(string domainName)
        {
            return SparkleNetworksApplication.WebCreate(domainName, () => new DefaultEmailTemplateProvider());
        }

        /// <summary>
        /// Destroys the DOMAIN LAYER BOOTSTRAPPER associated with the current PROCESS.
        /// </summary>
        public static void ClearSparkleApp(this HttpContext context)
        {
            SparkleNetworksApplication item;
            Reset<SparkleNetworksApplication>(context.Items, out item, null);
            if (item != null)
            {
                item.Dispose();
            }
        }

        /// <summary>
        /// Destroys the DOMAIN LAYER BOOTSTRAPPER associated with the current PROCESS.
        /// </summary>
        public static void ClearSparkleApp(this HttpContextBase context)
        {
            SparkleNetworksApplication item;
            Reset<SparkleNetworksApplication>(context.Items, out item, null);
            if (item != null)
            {
                item.Dispose();
            }
        }

        /// <summary>
        /// Gets the <see cref="SessionStateBehavior"/> in the specified http context.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static SessionStateBehavior GetSessionStateBehavior(this HttpContextBase context)
        {
            if (context is HttpContextWrapper)
            {
                var http = (HttpContextWrapper)context;
                if (getHttpContextMethod == null)
                {
                    // HACK: we should not use reflection against the framework private stuff
                    // but the framework does not expose what we need...
                    getHttpContextMethod = context.GetType().GetField("_context", BindingFlags.Instance | BindingFlags.NonPublic);
                }

                var httpContext = (HttpContext)getHttpContextMethod.GetValue(context);

                if (httpContext != null)
                {
                    return GetSessionStateBehavior(httpContext);
                }
            }
            
            ////throw new ArgumentException("The value must be of a HttpContextWrapper", "context");
            return SessionStateBehavior.Default;
        }

        /// <summary>
        /// Gets the <see cref="SessionStateBehavior"/> in the specified http context.
        /// </summary>
        public static SessionStateBehavior GetSessionStateBehavior(this HttpContext context)
        {
            if (context == null)
                throw new ArgumentNullException("value");

            if (getSessionStateBehaviorMethod == null)
            {
                // HACK: we should not use reflection against the framework private stuff
                // but the framework does not expose what we need...
                var contextType = context.GetType();
                var prop = contextType.GetProperty("SessionStateBehavior", BindingFlags.Instance | BindingFlags.NonPublic);
                getSessionStateBehaviorMethod = prop.GetGetMethod(true).CreateDelegate(typeof(Func<HttpContext, SessionStateBehavior>)) as Func<HttpContext, SessionStateBehavior>;
            }

            var value = getSessionStateBehaviorMethod(context);
            return value;
        }
        
        /// <summary>
        /// Gets a value from the HttpContext.Items collection. Allows creating the value if it does not exists.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dictionary"></param>
        /// <param name="factory"></param>
        /// <param name="autoCreate"></param>
        /// <returns></returns>
        [DebuggerStepThrough]
        private static T Get<T>(IDictionary dictionary, Func<T> factory, bool autoCreate)
            where T : class
        {
            var key = typeof(T).Name;
            return dictionary.GetValue<T>(key, factory, autoCreate);
        }

        /// <summary>
        /// Gets a value from the HttpContext.Items collection. Allows creating the value if it does not exists.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dictionary"></param>
        /// <param name="factory"></param>
        /// <param name="autoCreate"></param>
        /// <returns></returns>
        [DebuggerStepThrough]
        private static T Get<T>(IDictionary<string, object> dictionary, Func<T> factory, bool autoCreate)
            where T : class
        {
            var key = typeof(T).Name;
            if (dictionary.ContainsKey(key))
            {
                return (T)dictionary[key];
            }
            else if (autoCreate)
            {
                var value = factory();
                dictionary[key] = value;
                return value;
            }
            else
            {
                return default(T);
            }
        }

        [DebuggerStepThrough]
        private static void Set<T>(IDictionary<string, object> dictionary, T value)
            where T : class
        {
            var key = typeof(T).Name;
            dictionary[key] = value;
        }

        private static void Reset<T>(IDictionary dictionary, out T previousValue, T newValue)
            where T : class
        {
            var key = typeof(T).Name;
            if (dictionary.Contains(key))
            {
                previousValue = (T)dictionary[key];
            }
            else
            {
                previousValue = null;
            }

            dictionary[key] = newValue;
        }
    }
}
