namespace iERP.Application.Abstractions.Metadata;

/// <summary>
/// Canonical UI example for Sales Enquiry (matches UI developer sample).
/// </summary>
public static class SalesEnquiryExamplePayload
{
    public static object Document { get; } = new
    {
        enquiryCode = "TEA-ENQ-00056",
        customer = "Globex",
        contactPerson = "Alice Johnson",
        opportunity = "S1-OPP-00063",
        email = "alice.johnson@globex.com",
        phone = "+91 9876543210",
        altPhone = "+91 9876543211",
        probability = 75,
        title = "ERP Implementation Enquiry",
        expectedClose = "2026-09-15",
        status = "Open",
        winLossReason = "",
        projectedTotal = 125000,
        forecastType = "Expected",
        weightedTotal = 93750,
        range = "100000-150000",
        subsidiary = "i-ERP India",
        className = "Priority",
        location = "Bengaluru",
        department = "Sales",
        salesRep = "John Smith",
        lastSalesActivity = "2026-09-05",
        currency = "INR - RUPEE",
        exchangeRate = 1,
        payment = "Payment due within 30 days of invoice",
        prices = "Prices valid for 30 days from quotation date",
        delivery = "Delivery within two weeks of order confirmation",
        createInitialFollowUp = true,
        followUpDate = "2026-09-06",
        nextFollowUpDate = "2026-09-12",
        followUpStatus = "Pending",
        activityType = "Call",
        remarks = "Customer requested ERP implementation proposal.",
        items = new[]
        {
            new
            {
                category = "Software",
                item = "ERP License",
                description = "ERP core module license",
                quantity = 10,
                uom = "Nos",
                priceLevel = "Standard",
                rate = 10000,
                discount = 5,
                amount = 95000,
                taxCode = "GST18",
                grossAmount = 112100,
                className = "Priority",
                countryOfOrigin = "India",
                hsCode = "85238020"
            }
        }
    };
}
