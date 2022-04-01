using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Wine_Shop_Inventory_Application.DAL;
using Wine_Shop_Inventory_Application.Models;

namespace Wine_Shop_Inventory_Application.Controllers
{
    public class BaseController : Controller
    {
        #region Variable
        private DBWorker worker = new DBWorker();
        protected CurrentUser currentUser { get; set; }
        #endregion

        protected override void Initialize(RequestContext requestContext)
        {
            base.Initialize(requestContext);
            if (Session["UserId"] != null && Session["UserId"].ToString() != string.Empty)
            {
                CurrentUser CU = new CurrentUser();
                CU.UserId = Convert.ToInt32(Session["UserId"].ToString());
                CU.RoleId = Session["RoleId"] != null ? Convert.ToInt32(Session["RoleId"].ToString()) : 0;
                CU.Fullname = Session["Fullname"].ToString();
                CU.Email = Session["Email"].ToString();
                currentUser = CU;
            }
            else
            {
                currentUser = null;
                RedirectToRoute("/Account/Login");
            }
        }
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            HttpSessionStateBase session = filterContext.HttpContext.Session;
            // If the browser session or authentication session has expired...

            if (session.IsNewSession || Session["UserId"] == null)
            {
                if (filterContext.HttpContext.Request.IsAjaxRequest())
                {
                    // For AJAX requests, return result as a simple string, 
                    // and inform calling JavaScript code that a user should be redirected.
                    JsonResult result = Json("SessionTimeout", "text/html");
                    filterContext.Result = result;
                }
                else
                {
                    // For round-trip requests,
                    filterContext.Result = new RedirectToRouteResult(
                        new RouteValueDictionary { { "Controller", "Account" }, { "Action", "Login" } });
                }
            }
            base.OnActionExecuting(filterContext);
        }

        #region Globle Exception handler
        protected override void OnException(ExceptionContext filterContext)
        {
            // fail if we can't do anything
            if (filterContext == null)
                return;

            #region log

            var ex = filterContext.Exception ??
                   new Exception("No further information exists.");

            string controllerName = (string)filterContext.RouteData.Values["controller"];
            string actionName = (string)filterContext.RouteData.Values["action"];

            //ErrorSaveInDataBase(ex, ex.Source);
            filterContext.ExceptionHandled = true;
            ViewData["cMsg"] = "Error: " + ex.Message;
            ViewData["cstracktrack"] = ex.StackTrace;
            ViewData["cData"] = ex.Data;
            ViewData["cInnerException"] = ex.InnerException;
            ViewData["cAction"] = "Action: " + actionName;
            ViewData["cController"] = "Controller: " + controllerName;
            //ViewBag.URL = path;
            filterContext.Result = PartialView("Error");
            filterContext.HttpContext.Response.Clear();
            #endregion
        }
        #endregion

        #region SaveError
        //public static void ErrorSaveInDataBase(Exception exc, string source)
        //{
        //    try
        //    {
        //        ErrorLogViewModel _ErrorLogViewModel = new ErrorLogViewModel();
        //        if (exc.InnerException != null)
        //        {
        //            //_ErrorLogViewModel.checkInner = "Inner";
        //            _ErrorLogViewModel.InnerExceptionType = exc.InnerException.GetType().ToString();
        //            _ErrorLogViewModel.InnerException = exc.InnerException.Message;
        //            _ErrorLogViewModel.Source = source;

        //            if (exc.InnerException.StackTrace != null)
        //            {
        //                _ErrorLogViewModel.InnerStackTrace = exc.InnerException.StackTrace;
        //            }
        //        }
        //        _ErrorLogViewModel.ExceptionType = exc.GetType().ToString();
        //        _ErrorLogViewModel.Exception = exc.Message;
        //        _ErrorLogViewModel.Source = source;
        //        _ErrorLogViewModel.StackTrace = exc.StackTrace;
        //    }
        //    catch (Exception ex)
        //    {
        //    }
        //}
        #endregion
    }
}