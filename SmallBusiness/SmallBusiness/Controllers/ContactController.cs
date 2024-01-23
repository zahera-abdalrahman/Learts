using Microsoft.AspNetCore.Mvc;
using MimeKit;
using SmallBusiness.ViewModels;

namespace SmallBusiness.Controllers
{
    public class ContactController : Controller
    {
        public IActionResult Contact()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SendMessage(ContactFormViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Process the form data
                SendEmailAsync(model);

                // Display a success message
                ViewBag.SuccessMessage = "Your message has been sent successfully!";
                return View("Contact");
            }

            // If ModelState is not valid, return to the contact form with validation errors
            return View("Contact", model);
        }

        private async Task SendEmailAsync(ContactFormViewModel model)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(model.Name,model.Email));
            message.To.Add(new MailboxAddress("Admin", "zaheraalakash15@gmail.com"));
            message.Subject = "New Contact Form Submission";

            var bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = $@"
        <p>You have received a new contact form submission:</p>
        <p><strong>Name:</strong> {model.Name}</p>
        <p><strong>Email:</strong> {model.Email}</p>
        <p><strong>Message:</strong> {model.Message}</p>";

            message.Body = bodyBuilder.ToMessageBody();



            var smtpServer = "smtp.gmail.com";
            var smtpPort = 587;
            var useSsl = false;
            var smtpUsername = "learts2024@gmail.com";
            var smtpPassword = "luuqmyopbrmchuez";
            using (var client = new MailKit.Net.Smtp.SmtpClient())
            {
                await client.ConnectAsync(smtpServer, smtpPort, useSsl);
                await client.AuthenticateAsync(smtpUsername, smtpPassword);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
        }



    }
}
