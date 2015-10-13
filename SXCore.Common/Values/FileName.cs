using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SXCore.Common.Values
{
    public class FileName
    {
        public enum FileType { File, Image, Video, Audio, PDF, Excel, Word, PowerPoint, Text, ZIP };

        private string _name;

        public string Name
        {
            get { return _name; }
            private set { _name = value; }
        }

        private FileName(){ }

        public FileName(string name)
        { _name = name; }

        public string Extension
        {
            get
            {
                if (this.Name == "")
                    return "";

                return (new System.IO.FileInfo(this.Name)).Extension;
            }
        }

        public string MimeType
        {
            get { return System.Web.MimeMapping.GetMimeMapping(this.Name); }
        }

        public FileType Type
        {
            get { return DefineType(this.Extension); }
        }

        public override string ToString()
        {
            return this.Name;
        }

        public override int GetHashCode()
        {
            return this.Name.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return this.Name.Equals(obj);
        }

        static public FileType DefineType(string extension)
        {
            string ext = extension.TrimStart('.').Trim().ToLower();
            switch (ext)
            {
                case "jpeg":
                case "jpg":
                case "bmp":
                case "png":
                case "gif":
                case "tiff":
                    return FileType.Image;

                case "avi":
                case "mpeg":
                case "mp4":
                case "wmv":
                case "mov":
                    return FileType.Video;

                case "mp3":
                case "wav":
                case "wma":
                    return FileType.Audio;

                case "pdf":
                    return FileType.PDF;

                case "doc":
                case "docx":
                case "dotx":
                    return FileType.Word;

                case "xls":
                case "xlsx":
                    return FileType.Excel;

                case "ppt":
                case "pptx":
                    return FileType.PowerPoint;

                case "txt":
                    return FileType.Text;

                case "zip":
                case "7z":
                case "rar":
                    return FileType.ZIP;

                default:
                    return FileType.File;
            }

        }

        static public implicit operator FileName(string filename)
        { return new FileName(filename); }

        static public implicit operator string(FileName filename)
        { return filename.Name; }
    }
}
