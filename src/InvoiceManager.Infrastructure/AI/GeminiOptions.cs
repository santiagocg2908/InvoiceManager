namespace InvoiceManager.Infrastructure.AI
{
    public sealed class GeminiOptions
    {
        public const string SectionName = "Gemini";
        public string ApiKey {  get; set; } = string.Empty;

        public string Model {  get; set; } = "gemini-2.5-flash-lite";
    }
}
