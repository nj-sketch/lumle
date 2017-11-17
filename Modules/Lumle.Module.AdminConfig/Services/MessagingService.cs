using Lumle.Infrastructure;
using Lumle.Infrastructure.Utilities.Abstracts;
using System;
using System.Collections.Generic;
using System.Linq;
using Lumle.Infrastructure.Constants.LumleLog;
using NLog;
using System.Threading.Tasks;
using MimeKit;
using MailKit.Net.Smtp;
using Lumle.Data.Data.Abstracts;
using Lumle.Module.AdminConfig.Entities;

namespace Lumle.Module.AdminConfig.Services
{
    public class MessagingService : IMessagingService
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly IRepository<EmailTemplate> _emailTemplateRepository;
        private readonly IRepository<Credential> _credentialRepository;
        private readonly IEmailService _emailService;

        public MessagingService
        (
            IRepository<EmailTemplate> emailTemplateRepository,
            IRepository<Credential> credentialRepository,
            IEmailService emailService
        )
        {
            _emailTemplateRepository = emailTemplateRepository;
            _credentialRepository = credentialRepository;
            _emailService = emailService;
        }


        /// <summary>
        /// send confirmation email to new user while creating account
        /// </summary>
        /// <param name="to"></param>
        /// <param name="username"></param>
        /// <param name="url"></param>
        public async Task SendEmailConfirmationMailAsync(string to, string username, string url)
        {
            try
            {
                var emailTemplate = await _emailTemplateRepository.GetSingleAsync(x => x.Slug == "emailconfirmation");
                if (emailTemplate == null) throw new Exception();

                var emailServiceCredntials = _credentialRepository.GetAll(x => x.Slug == "smtpmailserver"
                                                                          || x.Slug == "smtpusername"
                                                                          || x.Slug == "smtpnoreplyemail"
                                                                          || x.Slug == "smtpuserpassword"
                                                                          || x.Slug == "loginurl"
                                                                          ).ToList();

                if (emailServiceCredntials == null) throw new Exception();

                emailTemplate.Body = emailTemplate.Body.Replace("%username%", username).Replace("%url%", url);

                var smtpOption = new SmtpOptions()
                {
                    Server = emailServiceCredntials.FirstOrDefault(x => x.Slug == "smtpmailserver")?.Value,
                    User = emailServiceCredntials.FirstOrDefault(x => x.Slug == "smtpusername")?.Value,
                    DefaultEmailFromAddress = emailServiceCredntials.FirstOrDefault(x => x.Slug == "smtpnoreplyemail")?.Value,
                    Password = emailServiceCredntials.FirstOrDefault(x => x.Slug == "smtpuserpassword")?.Value,
                    RequiresAuthentication = true
                };

                await _emailService.SendEmailAsync(smtpOption, to, smtpOption.DefaultEmailFromAddress, emailTemplate.Subject, null, emailTemplate.Body);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ErrorLog.LoginCredentialMailTemplate);
                throw;
            }
        }

        public async Task SendForgotPasswordMailAsync(string to, string username, string url)
        {

            try
            {
                var emailTemplate = await _emailTemplateRepository.GetSingleAsync(x => x.Slug == "forgotpassword");
                if (emailTemplate == null) return;

                emailTemplate.Body = emailTemplate.Body.Replace("%url%", url).Replace("%username%", username);

                var emailServiceCredentials = _credentialRepository.GetAll(x => x.Slug == "smtpmailserver"
                                                                            || x.Slug == "smtpusername"
                                                                            || x.Slug == "smtpnoreplyemail"
                                                                            || x.Slug == "smtpuserpassword"
                                                                            ).ToList();

                var smtpOption = new SmtpOptions()
                {
                    Server = emailServiceCredentials.FirstOrDefault(x => x.Slug == "smtpmailserver")?.Value,
                    User = emailServiceCredentials.FirstOrDefault(x => x.Slug == "smtpusername")?.Value,
                    DefaultEmailFromAddress = emailServiceCredentials.FirstOrDefault(x => x.Slug == "smtpnoreplyemail")?.Value,
                    Password = emailServiceCredentials.FirstOrDefault(x => x.Slug == "smtpuserpassword")?.Value,
                    RequiresAuthentication = true
                };

                await _emailService.SendEmailAsync(smtpOption, to, smtpOption.DefaultEmailFromAddress, emailTemplate.Subject, null, emailTemplate.Body);

            }
            catch (Exception ex)
            {
                Logger.Error(ex, ErrorLog.ForgetPasswordMailTemplate);
                throw;
            }
        }

        public async Task SendLoginCredentialMailAsync(string to, string username, string role, string email, string password)
        {
            try
            {
                var emailTemplate = await _emailTemplateRepository.GetSingleAsync(x => x.Slug == "logincredential");
                if (emailTemplate == null) throw new Exception();

                var emailServiceCredntials = _credentialRepository.GetAll(x => x.Slug == "smtpmailserver"
                                                                          || x.Slug == "smtpusername"
                                                                          || x.Slug == "smtpnoreplyemail"
                                                                          || x.Slug == "smtpuserpassword"
                                                                          || x.Slug == "loginurl"
                                                                          ).ToList();

                if (emailServiceCredntials == null) throw new Exception();

                emailTemplate.Body = emailTemplate.Body.Replace("%username%", username).Replace("%role%", role).Replace("%email%", email).Replace("%password%", password);
                emailTemplate.Body = emailTemplate.Body.Replace("%loginurl%", emailServiceCredntials.FirstOrDefault(x => x.Slug == "loginurl")?.Value);


                var smtpOption = new SmtpOptions()
                {
                    Server = emailServiceCredntials.FirstOrDefault(x => x.Slug == "smtpmailserver")?.Value,
                    User = emailServiceCredntials.FirstOrDefault(x => x.Slug == "smtpusername")?.Value,
                    DefaultEmailFromAddress = emailServiceCredntials.FirstOrDefault(x => x.Slug == "smtpnoreplyemail")?.Value,
                    Password = emailServiceCredntials.FirstOrDefault(x => x.Slug == "smtpuserpassword")?.Value,
                    RequiresAuthentication = true
                };

                await _emailService.SendEmailAsync(smtpOption, to, smtpOption.DefaultEmailFromAddress, emailTemplate.Subject, null, emailTemplate.Body);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ErrorLog.LoginCredentialMailTemplate);
                throw;
            }
        }

        public async Task SendMailAsync(string emailTemplateSlug, string to)
        {
            try
            {
                var emailTemplate = await _emailTemplateRepository.GetSingleAsync(x => x.Slug == emailTemplateSlug);
                if (emailTemplate == null) return;


                var emailServiceCredntials = _credentialRepository.GetAll(x => x.Slug == "smtpmailserver"
                                                                            || x.Slug == "smtpusername"
                                                                            || x.Slug == "smtpnoreplyemail"
                                                                            || x.Slug == "smtpuserpassword"
                                                                            ).ToList();

                var smtpOption = new SmtpOptions()
                {
                    Server = emailServiceCredntials.FirstOrDefault(x => x.Slug == "smtpmailserver")?.Value,
                    User = emailServiceCredntials.FirstOrDefault(x => x.Slug == "smtpusername")?.Value,
                    DefaultEmailFromAddress = emailServiceCredntials.FirstOrDefault(x => x.Slug == "smtpnoreplyemail")?.Value,
                    Password = emailServiceCredntials.FirstOrDefault(x => x.Slug == "smtpuserpassword")?.Value,
                    RequiresAuthentication = true
                };

                await _emailService.SendEmailAsync(smtpOption, to, smtpOption.DefaultEmailFromAddress, emailTemplate.Subject, null, emailTemplate.Body);

            }
            catch (Exception ex)
            {
                Logger.Error(ex, ErrorLog.MailError);
                throw;
            }

        }

        public async Task SendEmailWithBody(Dictionary<string, string> emailReceiverList, Dictionary<string, string> mailBodyDictionary)
        {
            try
            {
                var emailTemplate = await _emailTemplateRepository.GetSingleAsync(x => x.Slug == "systemhealthreport");
                if (emailTemplate == null) return;

                if (mailBodyDictionary == null) return;

                // Make email body
                var count = 1;
                foreach (var item in mailBodyDictionary)
                {
                    emailTemplate.Body = emailTemplate.Body.Replace("%serviceName" + count + "%", item.Key);
                    emailTemplate.Body = emailTemplate.Body.Replace("%serviceStatus" + count + "%", item.Value);
                    count++;
                }

                var emailServiceCredentials = _credentialRepository.GetAll(x => x.Slug == "smtpmailserver"
                                                                             || x.Slug == "smtpusername"
                                                                             || x.Slug == "smtpnoreplyemail"
                                                                             || x.Slug == "smtpuserpassword"
                ).ToList();

                var smtpOption = new SmtpOptions()
                {
                    Server = emailServiceCredentials.FirstOrDefault(x => x.Slug == "smtpmailserver")?.Value,
                    User = emailServiceCredentials.FirstOrDefault(x => x.Slug == "smtpusername")?.Value,
                    DefaultEmailFromAddress = emailServiceCredentials.FirstOrDefault(x => x.Slug == "smtpnoreplyemail")?.Value,
                    Password = emailServiceCredentials.FirstOrDefault(x => x.Slug == "smtpuserpassword")?.Value,
                    RequiresAuthentication = true
                };

                foreach (var emailReceiverItem in emailReceiverList)
                {
                    emailTemplate.Body = emailTemplate.Body.Replace("%username%", emailReceiverItem.Key);
                    await _emailService.SendEmailAsync(smtpOption, emailReceiverItem.Value, smtpOption.DefaultEmailFromAddress, emailTemplate.Subject, null, emailTemplate.Body);
                }


            }
            catch (Exception ex)
            {
                Logger.Error(ex, ErrorLog.MailError);
                throw;
            }
        }

        public void VerifySMTPStatus()
        {
            // Get Credentials from the email template
            var emailServiceCredntials = _credentialRepository.GetAll(
                                            x => x.Slug == "smtpmailserver" || x.Slug == "smtpusername" || x.Slug == "smtpnoreplyemail" || x.Slug == "smtpuserpassword").ToList();

            var smtpOption = new SmtpOptions()
            {
                Server = emailServiceCredntials.FirstOrDefault(x => x.Slug == "smtpmailserver")?.Value,
                User = emailServiceCredntials.FirstOrDefault(x => x.Slug == "smtpusername")?.Value,
                DefaultEmailFromAddress = emailServiceCredntials.FirstOrDefault(x => x.Slug == "smtpnoreplyemail")?.Value,
                Password = emailServiceCredntials.FirstOrDefault(x => x.Slug == "smtpuserpassword")?.Value,
                RequiresAuthentication = true
            };

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("supernarwen", "supernarwen@gmail.com"));
            message.To.Add(new MailboxAddress("Ekbana", "niraj@ekbana.com"));

            message.Subject = "Is this mail workin";

            var bodyBuilder = new BodyBuilder { HtmlBody = @"<b>Just ignore this mail</b>" };
            message.Body = bodyBuilder.ToMessageBody();

            using (var client = new SmtpClient())
            {
                // For demo-purposes, accept all SSL certificates (in case the server supports STARTTLS)
                client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                client.Connect(smtpOption.Server, 587, false);

                // Note: since we don't have an OAuth2 token, disable
                // the XOAUTH2 authentication mechanism.
                client.AuthenticationMechanisms.Remove("XOAUTH2");

                // Note: only needed if the SMTP server requires authentication
                client.Authenticate(smtpOption.User, smtpOption.Password);

                client.Send(message);
                client.Disconnect(true);
            }
        }
    }
}
