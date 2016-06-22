using System;

namespace SXCore.Common.Exceptions
{
    public class CustomException : ApplicationException
    {
        public virtual string Identifier { get { return "Error"; } }

        protected virtual string DefaultComment { get { return "Exception accured!"; } }

        public virtual string Comment { get; protected set; }

        public override string Message
        {
            get
            {
                var identifier = this.Identifier ?? "Error";
                var comment = String.IsNullOrWhiteSpace(this.Comment) ? this.DefaultComment : this.Comment;
                return $"{identifier}: {comment}";
            }
        }

        public CustomException() { }

        public CustomException(string comment)
        { this.Comment = comment; }
    }

    public class CustomNotFoundException : CustomException
    {
        public override string Identifier { get { return "NotFound"; } }

        protected override string DefaultComment { get { return "Object not found!"; } }

        public CustomNotFoundException(string comment = "")
            : base(comment)
        { }
    }

    public class CustomAuthenticationException : CustomException
    {
        public override string Identifier { get { return "Authentication"; } }

        protected override string DefaultComment { get { return "User is not authenticated!"; } }

        public CustomAuthenticationException(string comment = "")
            : base(comment)
        { }
    }

    public class CustomPermissionsException : CustomException
    {
        public override string Identifier { get { return "Authentication"; } }

        protected override string DefaultComment { get { return "User has no permissions to perform this action!"; } }

        public CustomPermissionsException(string comment = "")
            : base(comment)
        { }
    }

    public class CustomOperationException : CustomException
    {
        public override string Identifier { get { return "Authentication"; } }

        protected override string DefaultComment { get { return "Invalid operation!"; } }

        public CustomOperationException(string comment = "")
            : base(comment)
        { }
    }

    public class CustomSerializationException : CustomException
    {
        public override string Identifier { get { return "Serialization"; } }

        protected override string DefaultComment { get { return "Object can't be serialized!"; } }

        public CustomSerializationException(string comment = "")
            : base(comment)
        { }
    }

    public class CustomArgumentException : CustomException
    {
        public override string Identifier { get { return "Argument"; } }

        protected override string DefaultComment { get { return "Invalid argument!"; } }

        public CustomArgumentException(string comment = "")
            : base(comment)
        { }
    }
}
