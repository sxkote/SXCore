using SXCore.Common.Exceptions;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;

namespace SXCore.Common.Services
{
    public enum ImagerResizeType
    {
        /// <summary>
        /// Не изменять размеры изображения
        /// </summary>
        None,


        /// <summary>
        /// Новое изображение должно быть "вписано" в указанный размер без искажения с заполнением лишнего пространства цветом
        /// (скорее всего: оно будет менее указанного размера, а лишнее пространство будет заполнено цветом)
        /// </summary>
        Fit,


        /// <summary>
        /// Новое изображение должно "входить" без искажения в указанный размер, с текущими пропорции 
        /// (скорее всего: оно будет менее указанного размера, а пропорции будут сохранены)
        /// </summary>
        Scale,


        /// <summary>
        /// Новое изображение должно быть "обрезано" без искажения в рамках указанного размера
        /// (скорее всего: оно будет обрезано под размер)
        /// </summary>
        Crop,


        /// <summary>
        /// Новое изображение должно быть "растянуто" в рамки указанного размер
        /// (скорее всего: оно будет растянуто или сжато)
        /// </summary>
        Fill
    };

    public class ImagerResizeParams
    {
        #region Parametres
        public ImagerResizeType Type { get; private set; }

        public Size Size { get; private set; }
        public Color Background { get; private set; }

        public int Width { get { return ((this.Size == null) ? 0 : this.Size.Width); } }
        public int Height { get { return ((this.Size == null) ? 0 : this.Size.Height); } }
        #endregion

        #region Constructors
        public ImagerResizeParams()
        {
            this.Type = ImagerResizeType.None;
            this.Size = new Size(0, 0);
            this.Background = Color.Empty;
        }

        public ImagerResizeParams(ImagerResizeType type, Size size, Color background)
            : this()
        {
            this.Type = type;
            this.Size = size;
            this.Background = background;
        }

        public ImagerResizeParams(string type, Size size, Color background)
            : this(GetType(type), size, background)
        { }

        public ImagerResizeParams(string type, Size size, string background)
            : this(GetType(type), size, GetColor(background))
        { }
        #endregion

        #region Statics
        static public ImagerResizeType GetType(string type)
        {
            string val = type.Trim().ToLower();

            if (val == "fit")
                return ImagerResizeType.Fit;
            if (val == "scale")
                return ImagerResizeType.Scale;
            if (val == "fill")
                return ImagerResizeType.Fill;
            if (val == "crop")
                return ImagerResizeType.Crop;

            return ImagerResizeType.None;
        }

        static public Color GetColor(string color)
        {
            if (String.IsNullOrEmpty(color) || color.Trim().ToLower() == "empty")
                return Color.Empty;
            return Color.FromName(color);
        }
        #endregion
    }

    public class Imager
    {
        #region Constants
        public const string DefaultMimeType = "image/png";
        #endregion

        #region Variables
        protected Image _image = null;
        protected PropertyItem[] _properties = null;
        protected ImageCodecInfo _codec;
        #endregion

        #region Properties
        public Image Image
        {
            get { return _image; }
            private set { _image = value; }
        }

        protected ImageCodecInfo Codec
        { get { return _codec; } }

        public ImageFormat ImageFormat
        { get { return new ImageFormat(this.Codec.FormatID); } }

        public string MimeType
        { get { return this.Codec.MimeType; } }
        #endregion

        #region Constructors
        public Imager(Image image)
        {
            if (image == null)
                throw new CustomArgumentException("Imager can't be created with empty image object!");

            _image = image;
            _properties = image.PropertyItems;
            _codec = ImageCodecInfo.GetImageEncoders().FirstOrDefault(c => c.FormatID.ToString().Equals(image.RawFormat.Guid.ToString(), StringComparison.InvariantCultureIgnoreCase));
        }
        #endregion

        #region Functions
        public Imager Modify(ImagerResizeParams param)
        {
            //размеры текущего изображения
            int image_width = this.Image.Width;
            int image_height = this.Image.Height;

            //определяем процент сжатия, чтобы уложиться в новый размер
            float percent_width = ((float)param.Size.Width / (float)image_width);
            float percent_height = ((float)param.Size.Height / (float)image_height);

            float percent = Math.Min(percent_height, percent_width);

            //если нужно обрезать, то изображение должно быть больше указанных размеров
            if (param.Type == ImagerResizeType.Crop)
                percent = Math.Max(percent_height, percent_width);

            //если менярь размеры не нужно, то процент === 1
            if (param.Type == ImagerResizeType.None)
                percent = 1.0f;

            //окончательные размеры сжатого изображения
            int redraw_width = (int)(image_width * percent);
            int redraw_height = (int)(image_height * percent);

            //если нужно заполнить указанный размер с растяжением изображения 
            if (param.Type == ImagerResizeType.Fill)
            {
                redraw_width = param.Size.Width;
                redraw_height = param.Size.Height;
            }

            int canvas_width = param.Size.Width;
            int canvas_height = param.Size.Height;

            //если мы просто меняем размеры изображения пропорционально 
            // или не меняем размер изображения совсем
            // - холст должен совпадать с размерами измененного изображения
            if (param.Type == ImagerResizeType.None || param.Type == ImagerResizeType.Scale)
            {
                canvas_width = redraw_width;
                canvas_height = redraw_height;
            }

            //результирующий холст - новое изображение в новых размерах
            Bitmap result = new Bitmap(canvas_width, canvas_height);

            //как-то не правильно получилось - задаю CROP в квадрат - а возвращает он мне прямоугольник!!!
            ////если цвет заполнения не задан, то незачем слушаться новых размеров - просто нужно отмасштабировать
            //if (param.Type == ImageResizeType.None || param.Background == Color.Empty)
            //    result = new Bitmap(redraw_width, redraw_height);

            using (Graphics graphics = Graphics.FromImage(result))
            {
                if (param.Background != Color.Empty)
                    using (Brush br = new SolidBrush(param.Background))
                        graphics.FillRectangle(br, 0, 0, result.Width, result.Height);

                graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

                graphics.DrawImage(this.Image, (result.Width / 2) - (redraw_width / 2), (result.Height / 2) - (redraw_height / 2), redraw_width, redraw_height);
            }

            this.Image = result;

            return this;
        }

        public Imager Modify(ImagerResizeType type, Size size)
        {
            var param = new ImagerResizeParams(type, size, Color.Empty);
            return this.Modify(param);
        }

        public Imager Modify(ImagerResizeType type, int maxSize)
        {
            var param = new ImagerResizeParams(type, new Size(maxSize, maxSize), Color.Empty);
            return this.Modify(param);
        }

        public Imager Resize(int max_size)
        {
            var param = new ImagerResizeParams(ImagerResizeType.Scale, new Size(max_size, max_size), Color.Empty);
            return this.Modify(param);
        }

        protected byte[] Save(Action<Stream> writeToStream)
        {
            byte[] data = null;

            using (var ms = new MemoryStream())
            {
                writeToStream(ms);
                ms.Position = 0;
                data = ms.ToArray();
                ms.Close();
            }

            return data;
        }

        public byte[] Save(long quality, string mimeType = null)
        {
            var encoderMimeType = String.IsNullOrWhiteSpace(mimeType) ? this.MimeType : mimeType;

            var encoder = ImageCodecInfo.GetImageEncoders().FirstOrDefault(c => c.MimeType.Equals(encoderMimeType, StringComparison.InvariantCultureIgnoreCase));
            if (encoder == null)
                return null;

            // Encoder parameters for image (quality)
            var encoderParametres = new EncoderParameters(1);
            encoderParametres.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality);

            return this.Save(stream => this.Image.Save(stream, encoder, encoderParametres));
        }

        public byte[] Save(ImageFormat format)
        {
            return this.Save(stream => this.Image.Save(stream, format));
        }

        public byte[] Save()
        {
            return this.Save(this.ImageFormat);
        }

        public void Save(string filename, ImageFormat format = null)
        {
            this.Image.Save(filename, format == null ? this.ImageFormat : format);
        }

        //public Imager Load(byte[] data)
        //{
        //    this.Image = null;

        //    if (data != null)
        //    {
        //        using (MemoryStream ms = new MemoryStream(data))
        //        {
        //            ms.Position = 0;
        //            this.Image = Image.FromStream(ms);
        //            this._properties = this.Image.PropertyItems;
        //            ms.Close();
        //        }
        //    }

        //    return this;
        //}

        public PropertyItem GetProperty(int id)
        {
            try
            {
                if (this.Image == null)
                    return null;

                PropertyItem property = null;

                if (this.Image.PropertyItems != null && this.Image.PropertyItems.Length > 0)
                    property = this.Image.GetPropertyItem(id);
                else if (this._properties != null && this._properties.Length > 0)
                    property = this._properties.FirstOrDefault(p => p.Id == id);

                return property;
            }
            catch { return null; }
        }

        public string GetProperty(int id, Encoding encoding)
        {
            try
            {
                var property = this.GetProperty(id);
                if (property == null || property.Value == null || property.Len <= 0)
                    return "";

                return encoding.GetString(property.Value, 0, property.Len - ((encoding.IsSingleByte) ? 1 : 2));
            }
            catch { return ""; }
        }

        //public Image RoundCorners(int radius, Color background_color)
        //{
        //    radius *= 2;
        //    Bitmap result = new Bitmap(this.Image.Width, this.Image.Height);

        //    using (Graphics g = Graphics.FromImage(result))
        //    {
        //        g.Clear(background_color);
        //        g.SmoothingMode = SmoothingMode.AntiAlias;

        //        using (Brush brush = new TextureBrush(this.Image))
        //        using (GraphicsPath gp = new GraphicsPath())
        //        {
        //            gp.AddArc(0, 0, radius, radius, 180, 90);
        //            gp.AddArc(0 + result.Width - radius, 0, radius, radius, 270, 90);
        //            gp.AddArc(0 + result.Width - radius, 0 + result.Height - radius, radius, radius, 0, 90);
        //            gp.AddArc(0, 0 + result.Height - radius, radius, radius, 90, 90);
        //            g.FillPath(brush, gp);

        //            this.Image = result;
        //        }
        //    }

        //    return this.Image;
        //}

        public Imager Pad(Size size, Color background)
        {
            var param = new ImagerResizeParams(ImagerResizeType.Fit, size, background);

            return this.Modify(param);
        }


        public Imager Pad(Color background)
        {
            int max_size = Math.Max(this.Image.Height, this.Image.Width);

            return this.Pad(new Size(max_size, max_size), background);
        }

        public Imager Rotate(RotateFlipType type)
        {
            this.Image.RotateFlip(type);

            return this;
        }

        public Imager Rotate(bool clockwise)
        {
            if (clockwise)
                this.Image.RotateFlip(RotateFlipType.Rotate90FlipNone);
            else
                this.Image.RotateFlip(RotateFlipType.Rotate270FlipNone);

            return this;
        }

        public Imager RotateCorrection()
        {
            var property = this.GetProperty(0x112);
            if (property == null || property.Value == null || property.Value.Length <= 0)
                return this;

            RotateFlipType rotate = RotateFlipType.RotateNoneFlipNone;
            if (property.Value[0] == 6)
                rotate = RotateFlipType.Rotate90FlipNone;
            if (property.Value[0] == 3)
                rotate = RotateFlipType.Rotate180FlipNone;
            if (property.Value[0] == 8)
                rotate = RotateFlipType.Rotate270FlipNone;

            this.Rotate(rotate);

            property.Value = new byte[] { 0x1 };

            this.Image.SetPropertyItem(property);

            return this;
        }
        #endregion

        #region Statics
        static public Imager Create(Image img)
        {
            if (img == null)
                throw new CustomArgumentException("Image can't be empty");

            return new Imager(img);
        }

        static public Imager Create(byte[] data)
        {
            if (data == null)
                throw new CustomArgumentException("Imager can't be created with empty data");

            using (var ms = new MemoryStream(data))
                return new Imager(Image.FromStream(ms));
        }

        static public Imager Create(string filename)
        {
            if (!File.Exists(filename))
                throw new CustomArgumentException($"File {filename} not found to create Imager");

            return new Imager(Image.FromFile(filename));
        }

        //static public ImageCodecInfo GetImageEncoder(string mimeType)
        //{
        //    return ImageCodecInfo.GetImageEncoders().FirstOrDefault(c => c.MimeType.Equals(mimeType, StringComparison.InvariantCultureIgnoreCase));
        //}

        static public byte[] ResizeImage(byte[] image, int maxSize, int quality = 90)
        {
            if (image == null || image.Length <= 0)
                return null;

            return Imager.Create(image).Resize(maxSize).Save(quality);
        }
        #endregion
    }
}
