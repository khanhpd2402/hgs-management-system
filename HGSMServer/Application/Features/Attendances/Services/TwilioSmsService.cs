using Application.Features.Attendances.DTOs;
using Application.Features.Attendances.Interfaces;
using Microsoft.Extensions.Options;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace Application.Features.Attendances.Services
{
    public class TwilioSmsService : ISmsService
    {
        private readonly TwilioSettings  _twilioSettings;

        public TwilioSmsService(IOptions<TwilioSettings> twilioSettings)
        {
            _twilioSettings = twilioSettings.Value;
        }

        public async Task SendSmsAsync(string toPhoneNumber, string message)
        {
            TwilioClient.Init(_twilioSettings.AccountSid, _twilioSettings.AuthToken);
            var messageOptions = new CreateMessageOptions(new PhoneNumber(toPhoneNumber))
            {
                From = new PhoneNumber(_twilioSettings.FromNumber),
                Body = message
            };
            await MessageResource.CreateAsync(messageOptions);
        }
    }
}
