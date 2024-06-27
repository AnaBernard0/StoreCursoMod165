using System.Net.Mail;
using System.Net;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace StoreCursoMod165.Services
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            SmtpClient smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Credentials = new NetworkCredential("testecatarinabernardo@gmail.com", "vtdbfdrtcxhbdpgi"),
                Port = 587,
                EnableSsl = true
            };

            //enviar conteudo do email
            MailMessage mailMessage = new MailMessage()
            {
                From = new MailAddress("testecatarinabernardo@gmail.com", "Loja Catarina Bernardo"),
                Subject = subject,
                Body = htmlMessage,
                IsBodyHtml = true
            };

            mailMessage.To.Add(email);

            mailMessage.Bcc.Add("testecatarinabernardo@gmail.com"); //para saber o que é enviado

            smtpClient.Send(mailMessage);//enviar a mensagem      

            return Task.CompletedTask;//tarefa realizada
        }
    }
}
