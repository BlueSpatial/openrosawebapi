
using System;
using System.Net;
using System.Net.Http;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ODKnew
{
    public class BasicAuthenticationAttribute : AuthorizationFilterAttribute
    {
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            IEnumerable<string> panda;
            string kemerdekaan;    
            try {
                panda= actionContext.Request.Headers.GetValues("X-Openrosa-Version");
                kemerdekaan = panda.First();
            } catch (Exception e) {
                kemerdekaan = null;
            }
            if (actionContext.Request.Headers.Authorization == null && kemerdekaan == null)
            {
                actionContext.Response = actionContext.Request
                    .CreateResponse(HttpStatusCode.Unauthorized);
            }
            else if (actionContext.Request.Headers.Authorization != null && kemerdekaan != null)
            {
                string authenticationToken = actionContext.Request.Headers
                                            .Authorization.Parameter;
                string decodedAuthenticationToken = Encoding.UTF8.GetString(
                    Convert.FromBase64String(authenticationToken));
                string[] usernamePasswordArray = decodedAuthenticationToken.Split(':');
                string username = usernamePasswordArray[0];
                string password = usernamePasswordArray[1];

                if (Login(username, password))
                {
                    HttpContext.Current.Response.AppendHeader("X-Openrosa-Version","1.0");
                    HttpContext.Current.Response.AppendHeader("Content-Type", "text/xml; charset=UTF-8");
                    
                   
                }
                else
                {

                    actionContext.Response = actionContext.Request
                        .CreateResponse(HttpStatusCode.Unauthorized);
                }
            }
            else {
                HttpContext.Current.Response.AppendHeader("Www-Authenticate", "Basic realm=ODKnew");
                actionContext.Response = actionContext.Request
                .CreateResponse(HttpStatusCode.Unauthorized);
            }
        }
        public static bool Login(string username, string password)
        {
            if (username=="panda"&& password=="kecil") {
                return true;
            } else {
                return false;
            }
        }



    }
}