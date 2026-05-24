using EventTickets.Application.DTOs;
using System.ComponentModel.DataAnnotations;

namespace EventTickets.Application.Validators
{
    public static class ReserveTicketRequestValidator
    {
        public static void Validate(ReserveTicketRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.HolderName))
                throw new ValidationException("holderName is required.");
        }
    }
}
