using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http.Routing;

namespace WebAPI.Factories.Links
{
    public abstract class LinkFactory
    {
        private readonly UrlHelper _urlHelper;
        private readonly string _controllerName;
        private const string DefaultApi = "base";

        protected LinkFactory(HttpRequestMessage request, Type controllerType)
        {
            _urlHelper = new UrlHelper(request);
            _controllerName = GetControllerName(controllerType);
        }

        private string GetControllerName(Type controllerType)
        {
            var name = controllerType.Name;
            return name.Substring(0, name.Length - "controller".Length).ToLower();
        }

        protected Uri GetLink<TController>(object id, string action,QueryString query ,string route = DefaultApi)
        {
            var dic = new Dictionary<string,object>();
            dic["controller"]=GetControllerName(typeof(TController));
            dic["id"]=id;
            dic["action"]=action;
            if(query!=null)dic[query.name]=query.value;
            
            return GetUri(dic, route);
        }

        protected Uri GetUri(Dictionary<string,object> routeValues, string route = DefaultApi)
        {
            return new Uri(_urlHelper.Link(route, routeValues));
        }
        public Uri Self(int id, string route = DefaultApi)
        {
            var dic = new Dictionary<string, object>();
            dic["controller"] = _controllerName;
            dic["id"] = id;
            return GetUri(dic, route) ;
        }
        public Uri Self(string id, string route = DefaultApi)
        {
            var dic = new Dictionary<string, object>();
            dic["controller"] = _controllerName;
            dic["id"] = id;
            return GetUri(dic, route);
        }

        public class Rels
        {
            public const string Self = "self";
        }

       
    }
    public abstract class LinkFactory<TController> : LinkFactory
    {
        public LinkFactory(HttpRequestMessage request) : base(request, typeof(TController)) { }
    }

    public class QueryString
    {
        public string name;
        public object value;

    }
}
