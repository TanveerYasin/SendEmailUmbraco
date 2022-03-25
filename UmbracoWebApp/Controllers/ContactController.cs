using System;
using System.Configuration;
using System.Net;
using System.Net.Mail;
using System.Web.Mvc;
using Umbraco.Core;
using Umbraco.Core.Services;
using Umbraco.Web.Mvc;
using UmbracoWebApp.Models;

namespace UmbracoWebApp.Controllers
{
    public class ContactController : SurfaceController
    {
        public const string partialviewpath = "~/Views/Partials/SharedLayout/";
        // GET: Contact
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult SubmitForm(ContactModel model)
        {
            if (ModelState.IsValid)
            {
                var parentNode = Services.ContentService.GetById(CurrentPage.Id);
                var parentUdi = new GuidUdi(parentNode.ContentType.ToString(), parentNode.Key);
                var message = Services.ContentService.
                    CreateContent(string.Format("{0} {1}", model.username, DateTime.Now.ToString()), parentUdi, "Contact");
                message.SetValue("username", model.username);
                message.SetValue("email", model.email);
                message.SetValue("message", model.message);
                Services.ContentService.SaveAndPublish(message);
                SendEmail(model);
                return RedirectToCurrentUmbracoPage();
            }
            return CurrentUmbracoPage();
        }
        //Turn On less secure app access in your gmail account entered in web.config before running project
        private void SendEmail(ContactModel model)
        {
            
            var fromemail = new MailAddress(ConfigurationManager.AppSettings["FromEmail"]);//Add FromEmail key value in web config
            var frompass = ConfigurationManager.AppSettings["FromPassword"]; //Add FromPassword key value in web config
            var toadress = new MailAddress(model.email);
            var subject = ConfigurationManager.AppSettings["MailSubject"] + model.username; //Add Add MailSubject key value in web config
            var body = model.message;

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromemail.Address, frompass)
            };
            var message = new MailMessage(fromemail, toadress)
            {
                Subject = subject,
                Body = body
            };
            smtp.Send(message);
        }
    }
}