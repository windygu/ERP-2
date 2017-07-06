using CustomMembershipEF.Infrastructure;
using CustomMembershipEF.Interfaces;
using CustomMembershipEF.Services;
using ERP.BLL.Consts;
using ERP.BLL.ERP.AdminUser;
using ERP.Models.AdminUser;
using ERP.Models.Common;
using ERP.Models.CustomEnums;
using ERP.Tools;
using ERP.Tools.Logs;
using ERP.Tools.Validate;
using ERP.WebNew.Attributes;
using ERP.WebNew.Service;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;

namespace ERP.WebNew.Controllers
{
    [UserTrackerLog]
    public class AccountController : Controller
    {
        public IMembershipService MembershipService { get; set; }

        private UserServices _userServices = new UserServices();

        protected override void Initialize(RequestContext requestContext)
        {
            if (MembershipService == null) { MembershipService = new AccountMembershipService(); }

            base.Initialize(requestContext);
        }

        public ActionResult LogOn()
        {
            string redirectURL = Request.QueryString["ReturnUrl"];
            if (!string.IsNullOrEmpty(redirectURL) && Url.IsLocalUrl(redirectURL))
            {
                if (HttpContext.User.Identity.IsAuthenticated)
                {
                    return Redirect(redirectURL);
                }
                else
                {
                    ViewBag.ReturnURL = redirectURL;
                }
            }
            else
            {
                if (HttpContext.User.Identity.IsAuthenticated)
                {
                    return RedirectToAction("Index", "Home");
                }
            }

            return View(new VMLogOnModel() { UserName = "", Password = "" });
        }

        //
        // POST: /Account/Create
        [HttpPost]
        public ActionResult LogOn(VMLogOnModel model, string returnUrl)
        {
            // TODO: 开发阶段不验证
            // 先验证验证码
            //if ((string)HttpContext.Session[AdminConsts.SESSION_NAME_VALIDATE_CODE] != model.ValidateCode)
            //{
            //    ModelState.AddModelError("", "验证码错误");
            //}
            //else
            {
                if (ModelState.IsValid)
                {
                    AdminUserStatus status = _userServices.GetUserStatus(model.UserName);

                    if (status == AdminUserStatus.Locked)
                    {
                        ModelState.AddModelError("", "您的账号目前已被冻结，如有疑问请联系管理员！");
                        return View(model);
                    }
                    else if (status == AdminUserStatus.NotFind)
                    {
                        ModelState.AddModelError("", "用户名不存在！");
                        return View(model);
                    }

                    if (MembershipService.ValidateUser(model.UserName, model.Password))
                    {
                        LogHelper.WriteLog(string.Format("用户登录成功：User:{0}", model.UserName));

                        SetupFormsAuthTicket(model.UserName, model.RememberMe);

                        if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                            && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                        {
                            return Redirect(returnUrl);
                        }

                        if (status == AdminUserStatus.NotActived)
                        {
                            return RedirectToAction("ActiveAccount", "Account");
                        }
                        else
                        {
                            return RedirectToAction("Index", "Home");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "密码错误！");
                    }

                }
            }
            return View(model);
        }

        public ActionResult ActiveAccount()
        {
            VMChangePassword user = new VMChangePassword();
            return View(user);
        }

        [HttpPost]
        public ActionResult ActiveAccount(VMChangePassword user)
        {
            bool changeResult = MembershipService.ChangePassword(CurrentUserServices.Me.UserName, user.NewPassword, user.NewPassword);
            return new CustomJsonResult(changeResult);
        }

        public ActionResult ChangePwd()
        {
            VMChangePassword user = new VMChangePassword();
            return View(user);
        }

        [HttpPost]
        public ActionResult ChangePwd(VMChangePassword user)
        {
            bool oldPwdCorrect = _userServices.CompareOldPwd(CurrentUserServices.Me.UserID, CustomMembershipProvider.GetMd5Hash(user.OldPassword));
            if (oldPwdCorrect)
            {
                bool changeResult = MembershipService.ChangePassword(CurrentUserServices.Me.UserName, user.NewPassword, user.NewPassword);
                return new CustomJsonResult(changeResult ? 0 : -1);
            }
            else
            {
                return new CustomJsonResult(1);
            }
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            HttpContext.Session.Clear();
            return RedirectToAction("Logon", "Account");
        }

        private void SetupFormsAuthTicket(string loginName, bool persistanceFlag)
        {
            UserServices userService = new UserServices();
            VMERPUser currentUser = userService.GetCurrentUserInfos(loginName);

            string userData = JsonConvert.SerializeObject(currentUser);

            var authTicket = new FormsAuthenticationTicket(1, //version
                                                        loginName, // user name
                                                        DateTime.Now,             //creation
                                                        DateTime.Now.Add(FormsAuthentication.Timeout), //Expiration
                                                        persistanceFlag, //Persistent
                                                        userData);

            var encTicket = FormsAuthentication.Encrypt(authTicket);
            Response.Cookies.Add(new HttpCookie(FormsAuthentication.FormsCookieName, encTicket));
        }

        public ActionResult GetValidateCode()
        {
            string code = ValidateImgHelper.GenerateRandomCode(4);
            HttpContext.Session[AdminConsts.SESSION_NAME_VALIDATE_CODE] = code;
            byte[] bytes = ValidateImgHelper.GenerateValidateImg(code);
            return File(bytes, @"image/jpeg");
        }

        public ActionResult NoAuthPop()
        {
            return View();
        }

        public ActionResult NoAuth()
        {
            return View();
        }

        public ActionResult PageNotFind()
        {
            return View();
        }

        public ActionResult FileNotFind()
        {
            return View();
        }
        

        [Authorize]
        public ActionResult Logs()
        {
            return View(new VMLogs() { Type = HttpContext.Request["Type"], Date = string.IsNullOrEmpty(HttpContext.Request["Date"]) ? DateTime.Now.AddMonths(-1).ToString("yyyy-MM-dd") : HttpContext.Request["Date"], Keyword = HttpContext.Request["Keyword"] });
        }

        [Authorize]
        public ActionResult GetLogs(VMLogs log)
        {
            int currentPage = ERP.Tools.Utils.StrToInt(HttpContext.Request[EasyuiPagenationConsts.CURRENT_PAGE], 1);
            int pageSize = ERP.Tools.Utils.StrToInt(HttpContext.Request[EasyuiPagenationConsts.PAGE_SIZE], 1);
            int totalRows = 0;
            var logs = new MongoDBHelper().GetLogs(log.Type ?? "LogError", log.Date ?? DateTime.Now.AddMonths(-1).ToString("yyyy-MM-dd"), log.Keyword, currentPage, pageSize, out totalRows);
            return new CustomJsonResult(new VMEasyuiPagenationResult() { Total = totalRows, Rows = logs });
        }
    }

    public class VMMailTest
    {
        public string Host { get; set; }

        public int Port { get; set; }

        public string From { get; set; }

        public string To { get; set; }

        public string CC { get; set; }

        public string BCC { get; set; }

        public string Subject { get; set; }

        public string BodyContent { get; set; }
    }
}