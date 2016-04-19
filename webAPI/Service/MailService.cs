using jajs.MongoData.Interface;
using jajs.MongoData.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace jajs.API.Service
{
    public class MailService
    {
        private readonly IEmailNotificationRepository EmailRepository;

        public MailService(IEmailNotificationRepository emailRepository)
        {
            this.EmailRepository = emailRepository;
        }

        public void Send(string to, string templatePath, string username, string code)
        {
            MailMessage mail = new MailMessage();
            SmtpClient client = new SmtpClient();
            client.Port = 25;
            client.EnableSsl = true;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Host = "email-smtp.us-east-1.amazonaws.com";

            //mail.To.Add(new MailAddress(to));
            mail.To.Add(new MailAddress(to));
            mail.Bcc.Add(new MailAddress("parksmartlynz@gmail.com"));
            mail.From = new MailAddress("francis@parksmartly.co.nz");
            mail.Subject = "Welcome to parkSmartly";
            mail.IsBodyHtml = true;
            var param = new string[] { username, code };
            mail.Body = GetMessage(param, templatePath);
            client.UseDefaultCredentials = true;
            //client.Credentials = new NetworkCredential("AKIAIROLINP45YCSEZXQ", "Agz+P17KklLc9e8RlH3/dWWyZjp5StsYzOOeYQoZZsG/");
            client.Credentials = new NetworkCredential("AKIAJ7S5AA5HSXUFBGIQ", "AtkmIfjLl+KUkyIdhNGKPFCSfjrKjwz03v6RRyQGk4qH");
            var userToken = "something";
            client.Send(mail);
        }

        public void NewVendorRegistrationNotification(string email, string address)
        {
            MailMessage mail = new MailMessage();
            SmtpClient client = new SmtpClient();
            client.Port = 25;
            client.EnableSsl = true;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            //client.UseDefaultCredentials = false;
            client.Host = "email-smtp.us-east-1.amazonaws.com";

            //mail.To.Add(new MailAddress(to));
            mail.To.Add("francis.yanga@gmail.com");
            mail.To.Add("mguadalupe@gmail.com");
            //mail.To.Add("hello@parko.co.nz");
            mail.From = new MailAddress("francis.yanga@gmail.com");
            mail.Subject = "Parko - New vendor registration";
            mail.IsBodyHtml = true;
            var param = new string[] { email, address};
            mail.Body = GetMessage(param, System.Web.Hosting.HostingEnvironment.MapPath("~/EmailTemplates/NewVendorRegistration.html"));
            //client.UseDefaultCredentials = true;
            client.Credentials = new NetworkCredential("AKIAIYETBGWCLSE6K4QQ", "ApQQr4uByzNo5xfvftiVZbTUUfTwtirxJdB9jWKBIE+J");
            var userToken = "something";
            //client.Send(mail);
            client.SendMailAsync(mail);
        }

        public async Task<bool> VendorRegistrationNotification(ParkingSpace parkingSpace)
        {
            var param = new string[] { parkingSpace.Email, parkingSpace.Address };
            var emailToAdminBody = GetMessage(param, System.Web.Hosting.HostingEnvironment.MapPath("~/EmailTemplates/NewVendorRegistration.html"));
            var emailToAdminReceiver = new List<string>();
            emailToAdminReceiver.Add("francis.yanga@gmail.com");
            emailToAdminReceiver.Add("mguadalupe@gmail.com");
            emailToAdminReceiver.Add("welcome@parko.co.nz");
            var emailToAdmin = new EmailNotification
            {
                From = "welcome@parko.co.nz",
                To = emailToAdminReceiver,
                Subject = "Parko - new vendor registration",
                Message = emailToAdminBody,
                IsHtml = true,
                Status = 1
            };
            await EmailRepository.CreateSync(emailToAdmin);
            return true;
        }
        public async Task<bool> VendorRegistrationSendToVendor(ParkingSpace parkingSpace)
        {
            var param = new string[] { parkingSpace.Email, parkingSpace.Address };
            var emailToAdminBody = GetMessage(param, System.Web.Hosting.HostingEnvironment.MapPath("~/EmailTemplates/VendorRegistration.html"));
            var emailToAdminReceiver = new List<string>();
            emailToAdminReceiver.Add(parkingSpace.Email);
            var bcc = new List<string>();
            bcc.Add("welcome@parko.co.nz");
            var emailToAdmin = new EmailNotification
            {
                From = "welcome@parko.co.nz",
                To = emailToAdminReceiver,
                Subject = "Welcome to Parko!",
                Bcc = bcc,
                Message = emailToAdminBody,
                IsHtml = true,
                Status = 1
            };
            await EmailRepository.CreateSync(emailToAdmin);
            return true;
        }


        private string GetMessage(string[] parameters, string templatePath)
        {
            if (File.Exists(templatePath))
            {
                var template = File.ReadAllText(templatePath);
                for (var i = 0; i < parameters.Length; i++)
                {
                    template = template.Replace("{" + i + "}", parameters[i]);
                }
                return template;
            }
            return "";
        }
    }
}
