using SXCore.Common.Interfaces;
using SXCore.Common.Services;
using SXCore.Common.Values;
using System;

namespace SXCore.Common.Entities
{
    public class Avatar : Entity, ICoded
    {
        public const string Folder = "avatar";
        public const string FileName = "avatar.jpg";
        public const string Extension = ".jpg";
        public const string MimeType = "image/jpeg";
        public const int MaxSize = 100;
        public const int Quality = 90;

        public string Code { get; private set; }
        public string Value { get; private set; }

        private Avatar()
        {
            this.Code = Guid.NewGuid().ToString();
            this.Value = "";
        }

        public Avatar(string code, string value = "")
        {
            this.Code = code;
            this.Value = value ?? "";
        }

        public void ChangeValue(string value)
        {
            this.Value = value ?? "";
        }

        public override string ToString()
        {
            return this.Code ?? "";
        }

        public string GetPath()
        {
            return GetAvatarPath(this.Code);
        }

        static public implicit operator string(Avatar avatar)
        { return avatar == null ? null : avatar.Code; }

        static public Avatar CreateAvatar(string value = "")
        {
            return new Avatar() { Value = value ?? "" };
        }

        static public FileData MakeAvatarFile(byte[] data, int maxSize = Avatar.MaxSize, int quality = Avatar.Quality, string avatarFileName = Avatar.FileName, string mimeType = Avatar.MimeType)
        {
            if (data == null || data.Length <= 0)
                return null;

            byte[] avatarData = Imager.Create(data)
                                    .Modify(ImagerResizeType.Crop, maxSize)
                                    .Save(quality, mimeType);

            return new FileData(avatarFileName, avatarData);
        }

        static public string GetAvatarPath(string code)
        {
            return $"{Avatar.Folder}/{code}{Avatar.Extension}";
        }
    }
}
