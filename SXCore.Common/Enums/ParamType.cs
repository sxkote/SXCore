using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SXCore.Common.Enums
{
    public enum ParamType : long
    {
        /// <summary>
        /// Строка
        /// </summary>
        String = 1,

        /// <summary>
        /// Число
        /// </summary>
        Number = 2,

        /// <summary>
        /// Дата и время
        /// </summary>
        DateTime = 3,

        /// <summary>
        /// Флажок
        /// </summary>
        Boolean = 4,

        /// <summary>
        /// Файл
        /// </summary>
        File = 5
    }
}
