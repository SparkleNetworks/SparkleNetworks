
namespace Sparkle.Services.Networks
{
    using Sparkle.Common;

    public enum ResumeRequestType
    {
        Undefined = 0, // Tous / Peu importe
        FixedTermContract = 1, // CDD
        PermanentContract = 2, // CDI
        Pro = 3, // Contrat pro
        Training = 4, // Stage
    }
}
