using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Wine_Shop_Inventory_Application.DAL;
using Wine_Shop_Inventory_Application.Encryption;
using Wine_Shop_Inventory_Application.Models;

namespace Wine_Shop_Inventory_Application.Controllers
{
    public class AccountController : Controller
    {
        #region Variable
        DBWorker worker = new DBWorker();
        SecurityEncryption SE = new SecurityEncryption();
        #endregion

        #region Login
        // GET: Account
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(UserViewModel model)
        {
            if (ModelState.IsValid)
            {
                UserViewModel user = VerifyLogin(model.Email, model.Password);
                if (user != null)
                {
                    if (user.IsAvtive)
                    {
                        #region Modle Serialization
                        CurrentUser serializeModel = new CurrentUser();
                        serializeModel.UserId = user.UserId;
                        serializeModel.Email = user.Email;
                        serializeModel.Fullname = user.FullName;
                        serializeModel.RoleId = user.RoleId;
                        #endregion

                        Session["UserId"] = user.UserId;
                        Session["Email"] = user.Email;
                        Session["Fullname"] = user.FullName;
                        Session["RoleId"] = user.RoleId;
                        Session.Timeout = 1440;
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Your account is inactive. Please contact to administrator.");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "User Name or Password is Invalid.");
                }
                return View(model);
            }
            return View(model);
        }

        public UserViewModel VerifyLogin(string Email, string Password)
        {
            UserViewModel model = new UserViewModel();
            if (Email != "" && Password != "")
            {
                var user = worker.UserEntity.Get(x => x.Email == Email && x.Password == Password).FirstOrDefault();
                if (user != null)
                {
                    model.Email = user.Email;
                    model.FullName = user.FullName;
                    model.UserId = user.UserId;
                    model.RoleId = user.RoleId;
                    model.IsAvtive = user.IsAvtive;
                }
            }
            return model;
        }
        #endregion

        #region Forgot Password
        [HttpGet]
        public ActionResult ForgotPassword()
        {
            return PartialView("_ForgotPassword");
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult ForgotPassword(ForgotEmailModel model)
        {
            if (ModelState.IsValid)
            {
                string baseUrl = Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/');
                var user = worker.UserEntity.Get(s => s.Email == model.Email).FirstOrDefault();
                if (user != null)
                {
                    string token = SE.Encrypt(user.UserId.ToString());
                    MvcHtmlString Link1;

                    SendVerificationLinkEmail(user.Email, token, "ResetPassword");

                    DateTime expDate = DateTime.Now.AddHours(4);
                    user.PasswordResetToken = token;
                    user.PasswordResetTokenExpiry = expDate;
                    worker.UserEntity.Update(user);
                    worker.Save();
                    ViewBag.Message = "Your Password Reset Link has been sent";
                }
                else
                {
                    ModelState.AddModelError("", "E-mail is not valid");
                }
            }
            return PartialView("_ForgotPassword");
        }

        [NonAction]
        public void SendVerificationLinkEmail(string emailID, string activationCode, string emailFor)
        {
            var verifyUrl = "/Account/" + emailFor + "/" + activationCode;
            var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verifyUrl);

            var fromEmail = new MailAddress("sachin.sharma0583@gmail.com", "Sales Inventory Project");
            var toEmail = new MailAddress(emailID);
            var fromEmailPassword = "Saibaba@10583"; // Replace with actual password

            string subject = "";
            string body = "";
            if (emailFor == "ResetPassword")
            {
                subject = "Reset Password";
                body = "Hi,<br/><br/>We got request for reset your account password. Please click on the below link to reset your password" +
                    "<br/><br/><a href=" + link + ">Reset Password link</a>";
            }

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromEmail.Address, fromEmailPassword)
            };

            using (var message = new MailMessage(fromEmail, toEmail)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            })
                smtp.Send(message);
        }
        #endregion

        #region Reset Password
        [HttpGet]
        public ActionResult ResetPassword(string id)
        {
            ResetPasswordModel model = new ResetPasswordModel();
            model.PasswordResetToken = id;
            return PartialView("_ResetPassword", model);
        }

        [HttpPost]
        public ActionResult ResetPassword(ResetPasswordModel model)
        {
            if (model.PasswordResetToken != null && model.PasswordResetToken != "")
            {
                int Id = Convert.ToInt32(SE.Decrypt(model.PasswordResetToken));
                if (ModelState.IsValid)
                {
                    var user = worker.UserEntity.GetByID(Id);
                    if (user != null)
                    {
                        if (user.PasswordResetToken == model.PasswordResetToken && user.PasswordResetTokenExpiry >= DateTime.Now)
                        {
                            user.Password = model.NewPassword;
                            user.IsAvtive = true;
                            worker.UserEntity.Update(user);
                            worker.Save();
                            return RedirectToAction("Login", "Account");
                        }
                        else
                        {
                            ModelState.AddModelError("", "Reset Token Expired!");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Invalid Token!");
                    }
                }
            }
            return View(model);
        }
        #endregion

        #region Logout
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Logout()
        {
            Session.Abandon();
            Session.Clear();
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Account");
        }
        #endregion
    }
}