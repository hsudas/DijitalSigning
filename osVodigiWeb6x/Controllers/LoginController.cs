using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Text;
using System.Configuration;
using osVodigiWeb6x.Models;

namespace osVodigiWeb6x.Controllers
{
    public class LoginController : Controller
    {
        ILoginRepository repository;

        public LoginController()
            : this(new EntityLoginRepository())
        { }

        public LoginController(ILoginRepository paramrepository)
        {
            repository = paramrepository;
        }


        //
        // GET: /Login/

        public ActionResult Validate()
        {
            try
            {
                Session.Abandon();

                // Redirect to https is required
                if (ConfigurationManager.AppSettings["RequireHTTPS"] == "true")
                {
                    if (Request.Url.AbsoluteUri.StartsWith("http://"))
                        Response.Redirect(Request.Url.AbsoluteUri.Replace("http://", "https://"));
                }

                // Display the free links if appropriate
                ViewData["FreeLinks"] = "";
                if (ConfigurationManager.AppSettings["ShowFreeLinks"] == "true")
                    ViewData["FreeLinks"] = BuildFreeLinks();

                // Display the system messages, if any
                ViewData["SystemMessages"] = BuildSystemMessages();

                ViewData["Username"] = String.Empty;
                ViewData["Password"] = String.Empty;
                ViewData["ValidationMessage"] = String.Empty;
                ViewData["LoginInfo"] = "Please log in.";

                return View();
            }
            catch (Exception ex)
            {
                Helpers.SetupApplicationError("Login", "Validate", ex.Message);
                return RedirectToAction("Index", "ApplicationError");
            }
        }

        //
        // POST: /Login/

        [HttpPost]
        public ActionResult Validate(FormCollection collection)
        {
            try
            {
                // Validate the login
                User user = repository.ValidateLogin(Request.Form["txtUsername"].ToString(), Request.Form["txtPassword"].ToString());

                ViewData["FreeLinks"] = "";
                if (ConfigurationManager.AppSettings["ShowFreeLinks"] == "true")
                    ViewData["FreeLinks"] = BuildFreeLinks();

                // Display the system messages, if any
                ViewData["SystemMessages"] = BuildSystemMessages();

                if (user == null)
                {
                    ViewData["Username"] = Request.Form["txtUsername"].ToString();
                    ViewData["Password"] = String.Empty;
                    ViewData["ValidationMessage"] = "Invalid Login. Please try again.";
                    ViewData["LoginInfo"] = "Please log in.";

                    return View();
                }
                else
                {
                    Session["User"] = user;
                    Session["UserAccountID"] = user.AccountID;

                    IAccountRepository acctrep = new EntityAccountRepository();
                    Account account = acctrep.GetAccount(user.AccountID);
                    Session["UserAccountName"] = account.AccountName;

                    // Make sure the Account Folders exist
                    string serverpath = Server.MapPath("~/UploadedFiles");
                    if (!serverpath.EndsWith(@"\"))
                        serverpath += @"\";
                    System.IO.Directory.CreateDirectory(serverpath + user.AccountID.ToString() + @"\Images");
                    System.IO.Directory.CreateDirectory(serverpath + user.AccountID.ToString() + @"\Videos");
                    System.IO.Directory.CreateDirectory(serverpath + user.AccountID.ToString() + @"\Music");

                    serverpath = Server.MapPath("~/Media");
                    if (!serverpath.EndsWith(@"\"))
                        serverpath += @"\";
                    System.IO.Directory.CreateDirectory(serverpath + user.AccountID.ToString() + @"\Images");
                    System.IO.Directory.CreateDirectory(serverpath + user.AccountID.ToString() + @"\Videos");
                    System.IO.Directory.CreateDirectory(serverpath + user.AccountID.ToString() + @"\Music");

                    // Create example data for the account (if appropriate)
                    IPlayerGroupRepository pgrep = new EntityPlayerGroupRepository();
                    IEnumerable<PlayerGroup> groups = pgrep.GetAllPlayerGroups(account.AccountID);
                    if (groups == null || groups.Count() == 0)
                        acctrep.CreateExampleData(account.AccountID);

                    // Log the login
                    ILoginLogRepository llrep = new EntityLoginLogRepository();
                    LoginLog loginlog = new LoginLog();
                    loginlog.AccountID = user.AccountID;
                    loginlog.UserID = user.UserID;
                    loginlog.Username = user.Username;
                    loginlog.LoginDateTime = DateTime.Now.ToUniversalTime();
                    llrep.CreateLoginLog(loginlog);

                    return RedirectToAction("Index", "PlayerGroup");

                }
            }
            catch (Exception ex)
            {
                Helpers.SetupApplicationError("Login", "Validate POST", ex.Message);
                return RedirectToAction("Index", "ApplicationError");
            }
        }

        private string BuildFreeLinks()
        {
            try
            {
                StringBuilder links = new StringBuilder();

                links.Append("<span id=\"freelink\"><a href=\"http://www.vodigi.com/VodigiAccountSignUp.aspx\" target=\"_blank\">New to Vodigi? Click to Request a New Account.</a></span>");
                links.Append("<br />");
                links.Append("<span id=\"freelink\"><a href=\"http://free.vodigi.com/osVodigiPlayerSetup52.msi\" target=\"_blank\">Download the Vodigi Player Installer</a></span>");
                links.Append("<br />");
                links.Append("<span id=\"freelink\"><a href=\"http://www.vodigi.com/VodigiPlayerDeviceConfiguration.pdf\" target=\"_blank\">View the Vodigi Player Configuration Guide</a></span>");
                links.Append("<br />");
                links.Append("<span id=\"freelink\"><a href=\"http://www.youtube.com/user/VodigiDigitalSignage?ob=0&feature=results_main\" target=\"_blank\">Video İncele to Help You Get Started</a></span>");

                return links.ToString();
            }
            catch { return String.Empty; }

        }

        private string BuildSystemMessages()
        {
            try
            {
                StringBuilder msgstext = new StringBuilder();
                msgstext.Append("");

                ISystemMessageRepository msgrep = new EntitySystemMessageRepository();
                IEnumerable<SystemMessage> msgs = msgrep.GetSystemMessagesByDate(DateTime.Today);
                if (msgs != null && msgs.Count() > 0)
                {
                    msgstext.Append("<b>System Messages:</b><br>");
                    msgstext.Append("<ul>");
                    foreach (SystemMessage msg in msgs)
                    {
                        msgstext.Append("<li>");
                        msgstext.Append("<b>" + msg.SystemMessageTitle + ": " + "</b>" + msg.SystemMessageBody);
                        msgstext.Append("</li>");
                    }
                    msgstext.Append("</ul>");
                }

                return msgstext.ToString();
            }
            catch { return String.Empty; }

        }
    }
}
