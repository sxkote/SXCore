using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public Message FillTemplate(ValuesCollection collection)
        {
            var subject = collection.Replace(this.Subject);
            var text = collection.Replace(this.Text);

            return new Message(subject, text);
        }
    }
}
