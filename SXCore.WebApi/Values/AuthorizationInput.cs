namespace SXCore.WebApi.Values
{
    public class AuthorizationInput
    {
        public string Scheme { get; set; }
        public string Value { get; set; }

        public AuthorizationInput(string schema, string value)
        {
            this.Scheme = schema;
            this.Value = value;
        }
    }
}
