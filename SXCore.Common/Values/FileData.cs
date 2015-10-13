namespace SXCore.Common.Values
{
    public class FileData
    {
        private FileName _filename;
        private byte[] _data;

        public FileName FileName
        { get { return _filename; } }

        public byte[] Data
        { get { return _data; } }

        public string Hash
        { get { return _data == null ? "" : GetMD5(_data); } }

        public long Size
        { get { return _data == null ? 0 : _data.Length; } }

        public FileData(FileName filename, byte[] data = null)
        {
            _filename = filename;
            _data = data;
        }

        static public string GetMD5(byte[] data)
        {
            byte[] hash;

            using (var md5 = System.Security.Cryptography.MD5.Create())
                hash = md5.ComputeHash(data);

            var sBuilder = new System.Text.StringBuilder();
            for (int i = 0; i < hash.Length; i++)
                sBuilder.Append(hash[i].ToString("x2"));

            return sBuilder.ToString();
        }

    }
}
