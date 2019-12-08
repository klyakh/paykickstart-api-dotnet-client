public class PayKickstartIPNController : Controller
{
    public ActionResult IPN()
    {
        if(!IsValidIPN(Request.Form))
        {
            throw new Exception("Invalid IPN")
        }

        //your code here
    }


    /// <summary>
    /// Related information:
    /// https://support.paykickstart.com/api/#instant-payment-notification-ipn
    /// https://github.com/ArsalanDotMe/paykickstart-validator
    /// https://www.php.net/manual/ru/language.types.array.php
    /// https://www.php.net/manual/ru/function.ksort.php
    /// </summary>
    /// <returns></returns>
    private bool IsValidIPN(NameValueCollection inputFields)
    {
        var verificationCode = inputFields["verification_code"];

        var fields = new List<KeyValuePair<string, string>>();
        foreach (var fieldName in inputFields.AllKeys)
        {
            if (fieldName == "verification_code"
                || string.IsNullOrWhiteSpace(fieldName)
                || string.IsNullOrWhiteSpace(inputFields[fieldName])
                || inputFields[fieldName] == "0")
                continue;

            fields.Add(new KeyValuePair<string, string>(fields.Count.ToString(), inputFields[fieldName]));
        }

        fields = fields.OrderBy(f => f.Key).ToList();
        var allFieldValues = string.Join("|", fields.Select(f => f.Value));

        string hash = null;
        using (var hmacsha1 = new HMACSHA1(Encoding.ASCII.GetBytes(AppSettings.PayKickstartSecretKey)))
        {
            byte[] allFieldValuesBytes = Encoding.ASCII.GetBytes(allFieldValues);
            byte[] hashBytes = hmacsha1.ComputeHash(allFieldValuesBytes);

            hash = BitConverter.ToString(hashBytes).Replace("-", string.Empty).ToLower();
        }

        return hash == verificationCode;
    }
}