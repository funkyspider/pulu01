namespace Savant.Pulse.Utility.Client.PULU01.Models;

public record DonationRecord(
    string DonationNumber,
    string ProductCode,
    string HoldCode,
    DateTime? HoldDateTime = null)
{
    public string GetKey() => $"{DonationNumber}|{ProductCode}|{HoldCode}";
    
    public bool IsValid()
    {
        return !string.IsNullOrWhiteSpace(DonationNumber) &&
               !string.IsNullOrWhiteSpace(ProductCode) && (ProductCode.Length == 4 || ProductCode.ToUpper() == 'ALL') &&
               !string.IsNullOrWhiteSpace(HoldCode) && HoldCode.Length > 1;
    }

    /// <summary>
    /// Parses HDATE (YYYYMMDD) and HTIME (HHMMSSzzz) into a single DateTime
    /// </summary>
    public static DateTime? ParseHoldDateTime(string hdate, string htime)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(hdate) || string.IsNullOrWhiteSpace(htime))
                return null;

            hdate = hdate.Trim();
            htime = htime.Trim();

            // Parse HDATE (YYYYMMDD)
            if (hdate.Length != 8 || !int.TryParse(hdate, out _))
                return null;

            var year = int.Parse(hdate.Substring(0, 4));
            var month = int.Parse(hdate.Substring(4, 2));
            var day = int.Parse(hdate.Substring(6, 2));

            // Parse HTIME (HHMMSSzzz) - taking first 6 characters for HHMMSS
            if (htime.Length < 6)
                return null;

            var hour = int.Parse(htime.Substring(0, 2));
            var minute = int.Parse(htime.Substring(2, 2));
            var second = int.Parse(htime.Substring(4, 2));

            // Handle milliseconds if present (remaining digits)
            var millisecond = 0;
            if (htime.Length > 6)
            {
                var msString = htime.Substring(6);
                if (msString.Length >= 3)
                {
                    millisecond = int.Parse(msString.Substring(0, 3));
                }
                else if (msString.Length > 0)
                {
                    // Pad with zeros if less than 3 digits
                    millisecond = int.Parse(msString.PadRight(3, '0'));
                }
            }

            return new DateTime(year, month, day, hour, minute, second, millisecond);
        }
        catch
        {
            return null;
        }
    }
}
