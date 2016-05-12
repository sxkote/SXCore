namespace SXCore.Common.Values
{
    public class Message
    {
        protected string _subject;
        protected string _text;

        public string Subject { get { return _subject; } }
        public string Text { get { return _text; } }

        public Message(string subject, string text = "")
        {
            _subject = subject;
            _text = text;
        }

        public Message FillTemplate(ParamValueCollection collection)
        {
            var subject = collection.Replace(this.Subject);
            var text = collection.Replace(this.Text);

            return new Message(subject, text);
        }
    }
}
