using SXCore.Common.Exceptions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;

namespace SXCore.Common.Services
{
    #region Enums
    public enum ImageProperties : int
    {
        PropertyTagGpsVer = 0x0000,
        PropertyTagGpsLatitudeRef = 0x0001,
        PropertyTagGpsLatitude = 0x0002,
        PropertyTagGpsLongitudeRef = 0x0003,
        PropertyTagGpsLongitude = 0x0004,
        PropertyTagGpsAltitudeRef = 0x0005,
        PropertyTagGpsAltitude = 0x0006,
        PropertyTagGpsGpsTime = 0x0007,
        PropertyTagGpsGpsSatellites = 0x0008,
        PropertyTagGpsGpsStatus = 0x0009,
        PropertyTagGpsGpsMeasureMode = 0x000A,
        PropertyTagGpsGpsDop = 0x000B,
        PropertyTagGpsSpeedRef = 0x000C,
        PropertyTagGpsSpeed = 0x000D,
        PropertyTagGpsTrackRef = 0x000E,
        PropertyTagGpsTrack = 0x000F,
        PropertyTagGpsImgDirRef = 0x0010,
        PropertyTagGpsImgDir = 0x0011,
        PropertyTagGpsMapDatum = 0x0012,
        PropertyTagGpsDestLatRef = 0x0013,
        PropertyTagGpsDestLat = 0x0014,
        PropertyTagGpsDestLongRef = 0x0015,
        PropertyTagGpsDestLong = 0x0016,
        PropertyTagGpsDestBearRef = 0x0017,
        PropertyTagGpsDestBear = 0x0018,
        PropertyTagGpsDestDistRef = 0x0019,
        PropertyTagGpsDestDist = 0x001A,
        PropertyTagNewSubfileType = 0x00FE,
        PropertyTagSubfileType = 0x00FF,
        PropertyTagImageWidth = 0x0100,
        PropertyTagImageHeight = 0x0101,
        PropertyTagBitsPerSample = 0x0102,
        PropertyTagCompression = 0x0103,
        PropertyTagPhotometricInterp = 0x0106,
        PropertyTagThreshHolding = 0x0107,
        PropertyTagCellWidth = 0x0108,
        PropertyTagCellHeight = 0x0109,
        PropertyTagFillOrder = 0x010A,
        PropertyTagDocumentName = 0x010D,
        PropertyTagImageDescription = 0x010E,
        PropertyTagEquipMake = 0x010F,
        PropertyTagEquipModel = 0x0110,
        PropertyTagStripOffsets = 0x0111,
        PropertyTagOrientation = 0x0112,
        PropertyTagSamplesPerPixel = 0x0115,
        PropertyTagRowsPerStrip = 0x0116,
        PropertyTagStripBytesCount = 0x0117,
        PropertyTagMinSampleValue = 0x0118,
        PropertyTagMaxSampleValue = 0x0119,
        PropertyTagXResolution = 0x011A,
        PropertyTagYResolution = 0x011B,
        PropertyTagPlanarConfig = 0x011C,
        PropertyTagPageName = 0x011D,
        PropertyTagXPosition = 0x011E,
        PropertyTagYPosition = 0x011F,
        PropertyTagFreeOffset = 0x0120,
        PropertyTagFreeByteCounts = 0x0121,
        PropertyTagGrayResponseUnit = 0x0122,
        PropertyTagGrayResponseCurve = 0x0123,
        PropertyTagT4Option = 0x0124,
        PropertyTagT6Option = 0x0125,
        PropertyTagResolutionUnit = 0x0128,
        PropertyTagPageNumber = 0x0129,
        PropertyTagTransferFunction = 0x012D,
        PropertyTagSoftwareUsed = 0x0131,
        PropertyTagDateTime = 0x0132,
        PropertyTagArtist = 0x013B,
        PropertyTagHostComputer = 0x013C,
        PropertyTagPredictor = 0x013D,
        PropertyTagWhitePoint = 0x013E,
        PropertyTagPrimaryChromaticities = 0x013F,
        PropertyTagColorMap = 0x0140,
        PropertyTagHalftoneHints = 0x0141,
        PropertyTagTileWidth = 0x0142,
        PropertyTagTileLength = 0x0143,
        PropertyTagTileOffset = 0x0144,
        PropertyTagTileByteCounts = 0x0145,
        PropertyTagInkSet = 0x014C,
        PropertyTagInkNames = 0x014D,
        PropertyTagNumberOfInks = 0x014E,
        PropertyTagDotRange = 0x0150,
        PropertyTagTargetPrinter = 0x0151,
        PropertyTagExtraSamples = 0x0152,
        PropertyTagSampleFormat = 0x0153,
        PropertyTagSMinSampleValue = 0x0154,
        PropertyTagSMaxSampleValue = 0x0155,
        PropertyTagTransferRange = 0x0156,
        PropertyTagJPEGProc = 0x0200,
        PropertyTagJPEGInterFormat = 0x0201,
        PropertyTagJPEGInterLength = 0x0202,
        PropertyTagJPEGRestartInterval = 0x0203,
        PropertyTagJPEGLosslessPredictors = 0x0205,
        PropertyTagJPEGPointTransforms = 0x0206,
        PropertyTagJPEGQTables = 0x0207,
        PropertyTagJPEGDCTables = 0x0208,
        PropertyTagJPEGACTables = 0x0209,
        PropertyTagYCbCrCoefficients = 0x0211,
        PropertyTagYCbCrSubsampling = 0x0212,
        PropertyTagYCbCrPositioning = 0x0213,
        PropertyTagREFBlackWhite = 0x0214,
        PropertyTagGamma = 0x0301,
        PropertyTagICCProfileDescriptor = 0x0302,
        PropertyTagSRGBRenderingIntent = 0x0303,
        PropertyTagImageTitle = 0x0320,
        PropertyTagResolutionXUnit = 0x5001,
        PropertyTagResolutionYUnit = 0x5002,
        PropertyTagResolutionXLengthUnit = 0x5003,
        PropertyTagResolutionYLengthUnit = 0x5004,
        PropertyTagPrintFlags = 0x5005,
        PropertyTagPrintFlagsVersion = 0x5006,
        PropertyTagPrintFlagsCrop = 0x5007,
        PropertyTagPrintFlagsBleedWidth = 0x5008,
        PropertyTagPrintFlagsBleedWidthScale = 0x5009,
        PropertyTagHalftoneLPI = 0x500A,
        PropertyTagHalftoneLPIUnit = 0x500B,
        PropertyTagHalftoneDegree = 0x500C,
        PropertyTagHalftoneShape = 0x500D,
        PropertyTagHalftoneMisc = 0x500E,
        PropertyTagHalftoneScreen = 0x500F,
        PropertyTagJPEGQuality = 0x5010,
        PropertyTagGridSize = 0x5011,
        PropertyTagThumbnailFormat = 0x5012,
        PropertyTagThumbnailWidth = 0x5013,
        PropertyTagThumbnailHeight = 0x5014,
        PropertyTagThumbnailColorDepth = 0x5015,
        PropertyTagThumbnailPlanes = 0x5016,
        PropertyTagThumbnailRawBytes = 0x5017,
        PropertyTagThumbnailSize = 0x5018,
        PropertyTagThumbnailCompressedSize = 0x5019,
        PropertyTagColorTransferFunction = 0x501A,
        PropertyTagThumbnailData = 0x501B,
        PropertyTagThumbnailImageWidth = 0x5020,
        PropertyTagThumbnailImageHeight = 0x5021,
        PropertyTagThumbnailBitsPerSample = 0x5022,
        PropertyTagThumbnailCompression = 0x5023,
        PropertyTagThumbnailPhotometricInterp = 0x5024,
        PropertyTagThumbnailImageDescription = 0x5025,
        PropertyTagThumbnailEquipMake = 0x5026,
        PropertyTagThumbnailEquipModel = 0x5027,
        PropertyTagThumbnailStripOffsets = 0x5028,
        PropertyTagThumbnailOrientation = 0x5029,
        PropertyTagThumbnailSamplesPerPixel = 0x502A,
        PropertyTagThumbnailRowsPerStrip = 0x502B,
        PropertyTagThumbnailStripBytesCount = 0x502C,
        PropertyTagThumbnailResolutionX = 0x502D,
        PropertyTagThumbnailResolutionY = 0x502E,
        PropertyTagThumbnailPlanarConfig = 0x502F,
        PropertyTagThumbnailResolutionUnit = 0x5030,
        PropertyTagThumbnailTransferFunction = 0x5031,
        PropertyTagThumbnailSoftwareUsed = 0x5032,
        PropertyTagThumbnailDateTime = 0x5033,
        PropertyTagThumbnailArtist = 0x5034,
        PropertyTagThumbnailWhitePoint = 0x5035,
        PropertyTagThumbnailPrimaryChromaticities = 0x5036,
        PropertyTagThumbnailYCbCrCoefficients = 0x5037,
        PropertyTagThumbnailYCbCrSubsampling = 0x5038,
        PropertyTagThumbnailYCbCrPositioning = 0x5039,
        PropertyTagThumbnailRefBlackWhite = 0x503A,
        PropertyTagThumbnailCopyRight = 0x503B,
        PropertyTagLuminanceTable = 0x5090,
        PropertyTagChrominanceTable = 0x5091,
        PropertyTagFrameDelay = 0x5100,
        PropertyTagLoopCount = 0x5101,
        PropertyTagGlobalPalette = 0x5102,
        PropertyTagIndexBackground = 0x5103,
        PropertyTagIndexTransparent = 0x5104,
        PropertyTagPixelUnit = 0x5110,
        PropertyTagPixelPerUnitX = 0x5111,
        PropertyTagPixelPerUnitY = 0x5112,
        PropertyTagPaletteHistogram = 0x5113,
        PropertyTagCopyright = 0x8298,
        PropertyTagExifExposureTime = 0x829A,
        PropertyTagExifFNumber = 0x829D,
        PropertyTagExifIFD = 0x8769,
        PropertyTagICCProfile = 0x8773,
        PropertyTagExifExposureProg = 0x8822,
        PropertyTagExifSpectralSense = 0x8824,
        PropertyTagGpsIFD = 0x8825,
        PropertyTagExifISOSpeed = 0x8827,
        PropertyTagExifOECF = 0x8828,
        PropertyTagExifVer = 0x9000,
        PropertyTagExifDTOrig = 0x9003,
        PropertyTagExifDTDigitized = 0x9004,
        PropertyTagExifCompConfig = 0x9101,
        PropertyTagExifCompBPP = 0x9102,
        PropertyTagExifShutterSpeed = 0x9201,
        PropertyTagExifAperture = 0x9202,
        PropertyTagExifBrightness = 0x9203,
        PropertyTagExifExposureBias = 0x9204,
        PropertyTagExifMaxAperture = 0x9205,
        PropertyTagExifSubjectDist = 0x9206,
        PropertyTagExifMeteringMode = 0x9207,
        PropertyTagExifLightSource = 0x9208,
        PropertyTagExifFlash = 0x9209,
        PropertyTagExifFocalLength = 0x920A,
        PropertyTagExifMakerNote = 0x927C,
        PropertyTagExifUserComment = 0x9286,
        PropertyTagExifDTSubsec = 0x9290,
        PropertyTagExifDTOrigSS = 0x9291,
        PropertyTagExifDTDigSS = 0x9292,
        PropertyTagExifFPXVer = 0xA000,
        PropertyTagExifColorSpace = 0xA001,
        PropertyTagExifPixXDim = 0xA002,
        PropertyTagExifPixYDim = 0xA003,
        PropertyTagExifRelatedWav = 0xA004,
        PropertyTagExifInterop = 0xA005,
        PropertyTagExifFlashEnergy = 0xA20B,
        PropertyTagExifSpatialFR = 0xA20C,
        PropertyTagExifFocalXRes = 0xA20E,
        PropertyTagExifFocalYRes = 0xA20F,
        PropertyTagExifFocalResUnit = 0xA210,
        PropertyTagExifSubjectLoc = 0xA214,
        PropertyTagExifExposureIndex = 0xA215,
        PropertyTagExifSensingMethod = 0xA217,
        PropertyTagExifFileSource = 0xA300,
        PropertyTagExifSceneType = 0xA301,
        PropertyTagExifCfaPattern = 0xA302
    }

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
    #endregion

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

    public class ImageInfo
    {
        #region Constants
        public const int PropIDTitlePrimary = 270;
        public const int PropIDTitleSecondary = 40091;
        public const int PropIDComment = 40092;
        public const int PropIDDate = 36867;
        #endregion

        #region Properties
        public List<PropertyItem> PropertyItems { get; private set; }

        public string Title
        {
            get
            {
                try
                {
                    var result = this.GetPropertyValue(PropIDTitlePrimary, Encoding.UTF8);

                    if (String.IsNullOrWhiteSpace(result))
                        result = this.GetPropertyValue(PropIDTitleSecondary, Encoding.Unicode);

                    if (String.IsNullOrWhiteSpace(result))
                        result = this.GetPropertyValue((int)ImageProperties.PropertyTagImageTitle, Encoding.UTF8);

                    return result ?? "";
                }
                catch { return ""; }
            }
            set
            {
                try
                {
                    this.SetPropertyValue(PropIDTitlePrimary, value ?? "", Encoding.UTF8);
                    this.SetPropertyValue(PropIDTitleSecondary, value ?? "", Encoding.Unicode);
                    //this.SetPropertyValue((int)ImageProperties.PropertyTagImageTitle, value ?? "", Encoding.UTF8);
                }
                catch { }
            }
        }

        public string Comment
        {
            get
            {
                try { return this.GetPropertyValue(PropIDComment, Encoding.Unicode); }
                catch { return ""; }
            }
            set
            {
                try { this.SetPropertyValue(PropIDComment, value ?? "", Encoding.Unicode); }
                catch { }
            }
        }

        public DateTime? Date
        {
            get
            {
                try
                {
                    string value = this.GetPropertyValue(PropIDDate, Encoding.UTF8).Trim();

                    if (String.IsNullOrWhiteSpace(value))
                        value = this.GetPropertyValue((int)ImageProperties.PropertyTagDateTime, Encoding.UTF8);

                    if (String.IsNullOrWhiteSpace(value))
                        return null;

                    var formats = new string[] { "yyyy:MM:dd HH:mm:ss", "yyyy:MM:dd HH:mm", "yyyy:MM:dd" };

                    DateTime result;
                    if (DateTime.TryParseExact(value, formats, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out result))
                        return result;

                    return null;
                }
                catch { return null; }
            }
            set
            {
                try
                {
                    var dateString = value == null ? "" : value.Value.ToString("yyyy:MM:dd HH:mm:ss");
                    this.SetPropertyValue(PropIDDate, dateString, Encoding.UTF8);
                    //this.SetPropertyValue((int)ImageProperties.PropertyTagDateTime, dateString, Encoding.UTF8);
                }
                catch { }
            }
        }
        #endregion

        #region Constructors
        public ImageInfo(IEnumerable<PropertyItem> propertyItems)
        {
            if (propertyItems == null)
                throw new CustomArgumentException("Can't create ImageInfo from empty PropertyItems!");

            this.PropertyItems = propertyItems.ToList();
        }
        #endregion

        #region Functions
        public bool HasProperty(int id)
        { return this.PropertyItems.Any(p => p.Id == id); }

        public PropertyItem GetProperty(int id)
        {
            return this.PropertyItems.FirstOrDefault(p => p.Id == id);
        }

        public string GetPropertyValue(int id, Encoding encoding = null)
        {
            try
            {
                var property = this.GetProperty(id);
                if (property == null || property.Value == null || property.Len <= 0)
                    return "";

                if (encoding != null)
                    return encoding.GetString(property.Value, 0, property.Len).Trim(new char[] { '\0' });
                else if (property.Type == 1)
                    return Encoding.Unicode.GetString(property.Value, 0, property.Len).Trim(new char[] { '\0' });
                else if (property.Type == 2)
                    return Encoding.UTF8.GetString(property.Value, 0, property.Len).Trim(new char[] { '\0' });

                return "";
            }
            catch { return ""; }
        }

        public void SetPropertyValue(int id, string value, Encoding encoding = null)
        {
            PropertyItem property = this.GetProperty(id);

            if (property == null)
            {
                property = ImageInfo.CreatePropertyItem(id, (encoding == null || encoding == Encoding.Unicode) ? (short)1 : (short)2);
                if (property != null)
                    this.PropertyItems.Add(property);
            }

            if (property != null)
                this.SetPropertyValue(property, value, encoding);
        }

        protected void SetPropertyValue(PropertyItem property, string value, Encoding encoding = null)
        {
            if (property == null)
                return;

            byte[] data = null;
            if (encoding != null)
                data = encoding.GetBytes(value.Trim(new char[] { '\0' }) + '\0');
            else if (property.Type == 1)
                data = Encoding.Unicode.GetBytes(value.Trim(new char[] { '\0' }) + '\0');
            else if (property.Type == 2)
                data = Encoding.UTF8.GetBytes(value.Trim(new char[] { '\0' }) + '\0');

            if (data != null)
            {
                property.Value = data;
                property.Len = data.Length;
            }
        }

        public void RemoveProperty(int id)
        {
            var prop = this.GetProperty(id);
            if (prop != null)
                this.PropertyItems.Remove(prop);
        }

        public void Clear()
        {
            this.PropertyItems.Clear();
        }
        #endregion

        #region Statics
        //static public PropertyItem CreatePropertyItemFromDummy(int id, short type)
        //{
        //    var data = Convert.FromBase64String("/9j/4AAQSkZJRgABAQAAAQABAAD/2wCEAAYEBQYFBAYGBQYHBwYIChAKCgkJChQODwwQFxQYGBcUFhYaHSUfGhsjHBYWICwgIyYnKSopGR8tMC0oMCUoKSgBBwcHCggKEwoKEygaFhooKCgoKCgoKCgoKCgoKCgoKCgoKCgoKCgoKCgoKCgoKCgoKCgoKCgoKCgoKCgoKCgoKP/AABEIAAEAAQMBEQACEQEDEQH/xAGiAAABBQEBAQEBAQAAAAAAAAAAAQIDBAUGBwgJCgsQAAIBAwMCBAMFBQQEAAABfQECAwAEEQUSITFBBhNRYQcicRQygZGhCCNCscEVUtHwJDNicoIJChYXGBkaJSYnKCkqNDU2Nzg5OkNERUZHSElKU1RVVldYWVpjZGVmZ2hpanN0dXZ3eHl6g4SFhoeIiYqSk5SVlpeYmZqio6Slpqeoqaqys7S1tre4ubrCw8TFxsfIycrS09TV1tfY2drh4uPk5ebn6Onq8fLz9PX29/j5+gEAAwEBAQEBAQEBAQAAAAAAAAECAwQFBgcICQoLEQACAQIEBAMEBwUEBAABAncAAQIDEQQFITEGEkFRB2FxEyIygQgUQpGhscEJIzNS8BVictEKFiQ04SXxFxgZGiYnKCkqNTY3ODk6Q0RFRkdISUpTVFVWV1hZWmNkZWZnaGlqc3R1dnd4eXqCg4SFhoeIiYqSk5SVlpeYmZqio6Slpqeoqaqys7S1tre4ubrCw8TFxsfIycrS09TV1tfY2dri4+Tl5ufo6ery8/T19vf4+fr/2gAMAwEAAhEDEQA/APlq7uZ7y6muryaWe5mdpJZZXLPI7HJZieSSSSSaAP8A/9k=");
        //    var bitmap = new Bitmap(new MemoryStream(data));

        //    var property = bitmap.PropertyItems.FirstOrDefault(p => p.Id == id);
        //    if (property != null)
        //        return property;

        //    property = bitmap.PropertyItems.FirstOrDefault();
        //    if (property != null)
        //    {
        //        property.Id = id;
        //        property.Type = type;
        //    }

        //    return property;
        //}

        static public PropertyItem CreatePropertyItem(int id, short type)
        {
            try
            {
                var property = (PropertyItem)FormatterServices.GetUninitializedObject(typeof(PropertyItem));
                if (property != null)
                {
                    property.Id = id;
                    property.Type = type;
                }
                return property;
            }
            catch { return null; }
        }
        #endregion
    }

    public class Imager
    {
        #region Constants
        public const string DefaultMimeType = "image/png";
        #endregion

        #region Variables
        protected Bitmap _image = null;
        protected ImageInfo _info = null;
        protected ImageCodecInfo _codec;
        #endregion

        #region Properties
        public Bitmap Image
        {
            get { return _image; }
            private set { _image = value; }
        }

        public ImageInfo Info
        {
            get { return _info; }
            private set { _info = value; }
        }

        protected ImageCodecInfo Codec
        { get { return _codec; } }

        public ImageFormat ImageFormat
        { get { return new ImageFormat(this.Codec.FormatID); } }

        public string MimeType
        { get { return this.Codec.MimeType; } }

        public int Width { get { return this.Image.Width; } }

        public int Height { get { return this.Image.Height; } }
        #endregion

        #region Constructors
        public Imager(Image image)
        {
            if (image == null)
                throw new CustomArgumentException("Imager can't be created with empty image object!");

            _image = new Bitmap(image);
            _info = new ImageInfo(image.PropertyItems);
            _codec = ImageCodecInfo.GetImageEncoders().FirstOrDefault(c => c.FormatID.ToString().Equals(image.RawFormat.Guid.ToString(), StringComparison.InvariantCultureIgnoreCase));
        }
        #endregion

        #region Functions
        public Imager Modify(ImagerResizeParams param)
        {
            //размеры текущего изображения
            int image_width = this.Width;
            int image_height = this.Height;

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

        public Imager Modify(ImagerResizeType type, int maxWidth, int maxHeight)
        {
            var param = new ImagerResizeParams(type, new Size(maxWidth, maxHeight), Color.Empty);
            return this.Modify(param);
        }

        public Imager Modify(ImagerResizeType type, int maxSize)
        {
            return this.Modify(type, maxSize, maxSize);
        }

        public Imager Resize(int maxSize)
        {
            return this.Resize(maxSize, maxSize);
        }

        public Imager Resize(int maxWidth, int maxHeight)
        {
            var param = new ImagerResizeParams(ImagerResizeType.Scale, new Size(maxWidth, maxHeight), Color.Empty);
            return this.Modify(param);
        }

        protected void SaveInfoToImage()
        {
            if (this.Info != null)
            {
                this.Info.PropertyItems
                    .ForEach(p => this.Image.SetPropertyItem(p));

                this.Image.PropertyIdList
                    .Where(id => !this.Info.HasProperty(id)).ToList()
                    .ForEach(id => this.Image.RemovePropertyItem(id));
            }
        }

        protected byte[] Save(Action<Stream> writeToStream)
        {
            this.SaveInfoToImage();

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
        { return this.Save(stream => this.Image.Save(stream, format)); }

        public byte[] Save()
        { return this.Save(this.ImageFormat); }

        public void Save(string filename, ImageFormat format = null)
        {
            this.SaveInfoToImage();

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

        //public PropertyItem GetProperty(int id)
        //{
        //    try
        //    {
        //        if (this.Image == null)
        //            return null;

        //        PropertyItem property = null;

        //        if (this.Image.PropertyItems != null && this.Image.PropertyItems.Length > 0)
        //            property = this.Image.GetPropertyItem(id);
        //        else if (this._properties != null && this._properties.Length > 0)
        //            property = this._properties.FirstOrDefault(p => p.Id == id);

        //        return property;
        //    }
        //    catch { return null; }
        //}

        //public string GetProperty(int id, Encoding encoding)
        //{
        //    try
        //    {
        //        var property = this.GetProperty(id);
        //        if (property == null || property.Value == null || property.Len <= 0)
        //            return "";

        //        //return encoding.GetString(property.Value, 0, property.Len - ((encoding.IsSingleByte) ? 1 : 2);
        //        return encoding.GetString(property.Value, 0, property.Len).Trim(new char[] { '\0' });
        //    }
        //    catch { return ""; }
        //}

        //public void SetProperty(int id, string value, Encoding encoding)
        //{
        //    try
        //    {
        //        PropertyItem property = this.GetProperty(id);
        //        if (property == null)
        //            return;

        //        property.Value = encoding.GetBytes(value);

        //        this.Image.SetPropertyItem(property);
        //    }
        //    catch { }
        //}

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
            int max_size = Math.Max(this.Height, this.Width);

            return this.Pad(new Size(max_size, max_size), background);
        }

        public Imager Rotate(RotateFlipType type)
        {
            this.Image.RotateFlip(type);

            this.Info.RemoveProperty((int)ImageProperties.PropertyTagOrientation);

            return this;
        }

        public Imager Rotate(bool clockwise)
        {
            if (clockwise)
                this.Rotate(RotateFlipType.Rotate90FlipNone);
            else
                this.Rotate(RotateFlipType.Rotate270FlipNone);

            return this;
        }

        public Imager RotateCorrection()
        {
            var property = this.Info.GetProperty((int)ImageProperties.PropertyTagOrientation);
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

            //property.Value = new byte[] { 0x1 };

            //this.Image.SetPropertyItem(property);

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
                return new Imager(System.Drawing.Image.FromStream(ms));
        }

        static public Imager Create(string filename)
        {
            if (!File.Exists(filename))
                throw new CustomArgumentException($"File {filename} not found to create Imager");

            return new Imager(System.Drawing.Image.FromFile(filename));
        }

        static public Imager CreateFromDataUrl(string dataUrl)
        {
            if (String.IsNullOrWhiteSpace(dataUrl))
                throw new CustomArgumentException("Invalid DataUrl specified: input is empty!");

            string dataUrlPattern = "data:(?<mime>.*);base64,(?<data>.*)";
            Match match = Regex.Match(dataUrl, dataUrlPattern);
            if (!match.Success)
                throw new CustomArgumentException($"Invalid DataUrl specified: input doesn't match the pattern '{dataUrlPattern}'");

            var data64 = match.Groups["data"]?.Value;
            if (String.IsNullOrWhiteSpace(data64))
                throw new CustomArgumentException($"Invalid DataUrl specified: no base64 data found!");

            using (var ms = new MemoryStream(Convert.FromBase64String(data64)))
                return new Imager(System.Drawing.Image.FromStream(ms));
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
