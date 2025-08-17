namespace Savant.Pulse.Utility.Client.PULU01.Models;

public interface IProcessingRecord
{
    string GetKey();
    bool IsValid();
}