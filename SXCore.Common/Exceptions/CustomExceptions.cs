using System;

namespace SXCore.Common.Exceptions
{
    public class CustomException : ApplicationException
    {
        public virtual string Template { get; protected set; }

        public virtual object[] Arguments { get; set; }

        public virtual string Identifier { get; set; }

        public override string Message
        {
            get
            {
                return String.Format("{0}: {1}", this.Identifier, this.Arguments == null ? this.Template : String.Format(this.Template, this.Arguments));
            }
        }

        public CustomException()
        {
            this.Identifier = "Error";
            this.Template = "Произошла непредвиденная ошибка!";
            this.Arguments = null;
        }

        public CustomException(string message)
            : this()
        { this.Template = message; }

        public CustomException(string identifier, string template, params object[] args)
            : this()
        {
            this.Identifier = identifier;
            this.Template = template;
            this.Arguments = args;
        }

        //public CustomException(string template, params object[] args)
        //    : this("Error", template, args) { }
    }

    public class CustomNotFoundException : CustomException
    {
        public CustomNotFoundException(string title = "")
            : base("Not Found", "Запрошенный объект {0} не найден!", title)
        { }

        public CustomNotFoundException(string type, object key)
            : base("Not Found", "Запрошенный объект {0}:{1} не найден!", type, key)
        { }
    }

    public class CustomAuthenticationException : CustomException
    {
        public CustomAuthenticationException()
            : base("Authorization", "Ошибка авторизации")
        { }

        public CustomAuthenticationException(string login)
            : base("Authorization", "Пользователь {0} не авторизован! Проверьте логин и/или пароль!", login)
        { }
    }

    public class CustomPermissionsException : CustomException
    {
        public CustomPermissionsException()
            : base("Permissions", "Не достаточно прав для запрошенного действия!")
        { }
    }

    public class CustomOperationException : CustomException
    {
        public CustomOperationException(string operation, string reason = "")
            : base("Operation", "Операция \"{0}\" не может быть выполнена!", operation, reason)
        {
            if (!String.IsNullOrEmpty(reason))
                this.Template = "Операция \"{0}\" не может быть выполнена, так как {1}";
        }
    }

    public class CustomSerializationException : CustomException
    {
        public CustomSerializationException()
            : base("Serialization", "Ошибка сериализации объекта")
        { }

        public CustomSerializationException(string typeTitle)
            : base("Serialization", "Ошибка сериализации объекта {0}", typeTitle)
        { }
    }

    public class CustomArgumentException : CustomException
    {
        public CustomArgumentException(string param)
            : base("Argument", "Параметр {0} указан не верно!", param)
        { }

    }
}
