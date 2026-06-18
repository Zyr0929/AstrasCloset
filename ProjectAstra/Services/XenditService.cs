using Xendit.net.Model.Invoice;
using Xendit.net.Enum;
using Xendit.net.Model.Invoice;
using Xendit.net.Struct;

namespace ProjectAstra.Services;

public class XenditService
{
    private readonly string _secretKey;

    public XenditService(IConfiguration config)
    {
        _secretKey = config["Xendit:SecretKey"] ?? "";
    }

    public async Task<string> CreateInvoice(
        string orderId,
        decimal amount,
        string email)
    {
        var invoiceClient = new InvoiceClient(
            _secretKey,
            null,
            null
        );

        var invoice = await invoiceClient.Create(new InvoiceParameter
        {
            ExternalId = orderId,
            Amount = (long)amount,
            PayerEmail = email,
            Description = $"Order {orderId}",

            PaymentMethods = new[]
    {
        InvoicePaymentChannelType.Gcash
    }
        });

        return invoice.InvoiceUrl;
    }
}