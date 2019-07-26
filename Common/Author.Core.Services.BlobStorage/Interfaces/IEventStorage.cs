using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Author.Core.Services.BlobStorage.Interfaces
{
   public interface IEventStorage
    {
        Task<List<string>> saveFile(ImageData imageData);
    }
}
