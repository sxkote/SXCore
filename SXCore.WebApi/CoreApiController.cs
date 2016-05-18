using SXCore.Common.Contracts;
using SXCore.Common.Values;
using SXCore.WebApi.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace SXCore.WebApi
{
    [ApiException]
    public class CoreApiController : ApiController
    {
        //protected ILog _log { get; private set; }

        //protected CustomUserSession GetCurrentSession()
        //{ return this.SessionAs<CustomUserSession>(); }

        public ICacheProvider CacheProvider { get; set; }

        public ApiPrincipal CurrentUser
        { get { return ApiPrincipal.Current; } }

        public long CurrentSubscriptionID
        {
            get
            {
                var user = this.CurrentUser;
                return user == null ? 0 : user.SubscriptionID;
            }
        }

        public long CurrentPersonID
        {
            get
            {
                var user = this.CurrentUser;
                return user == null ? 0 : user.PersonID;
            }
        }

        public CoreApiController()
        {

            //_log = LogManager.GetLogger(GetType().FullName);

        }

        protected HttpResponseMessage ReturnFile(FileData file)
        {
            if (file == null)
                return new HttpResponseMessage(HttpStatusCode.NoContent);

            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            result.Content = new ByteArrayContent(file.Data);
            result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
            result.Content.Headers.ContentDisposition.FileName = file.FileName;
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            result.Content.Headers.ContentLength = file.Size;
            return result;
        }

        //protected HttpResult FileDataResult(FileData file)
        //{
        //    if (file == null || file.Data == null)
        //        return null;

        //    this.Response.AddHeader("Content-Disposition", String.Format("filename=\"{0}\"", file.FileName));
        //    this.Response.AddHeader("X-FileName", file.FileName.Name);

        //    return new HttpResult(file.Data, file.FileName.MimeType);
        //}

        #region IDisposable
        //public override void Dispose()
        //{
        //    this.Dispose(true);
        //    GC.SuppressFinalize(this);
        //}

        //protected virtual void Dispose(bool disposing)
        //{
        //    if (disposing)
        //    {
        //        if (this.Cache != null)
        //        {
        //            this.Cache.Dispose();
        //            //this.Cache = null;
        //        }
        //    }
        //}
        #endregion
    }
}
