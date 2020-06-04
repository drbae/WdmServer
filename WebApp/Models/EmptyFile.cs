using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DrBAE.WdmServer.WebApp.Models
{
    public class EmptyFile : IFormFile
    {
        public readonly static IFormFile Default = new EmptyFile();

        public string ContentDisposition => throw new NotImplementedException();

        public string ContentType => throw new NotImplementedException();

        public string FileName => "(파일없음)";

        public IHeaderDictionary Headers => throw new NotImplementedException();

        public long Length => throw new NotImplementedException();

        public string Name => throw new NotImplementedException();

        public void CopyTo(Stream target)
        {

        }

        public Task CopyToAsync(Stream target, CancellationToken cancellationToken = default)
        {
            return new Task(() => { }, cancellationToken);
        }

        public Stream OpenReadStream()
        {
            throw new NotImplementedException();
        }
    }
}
