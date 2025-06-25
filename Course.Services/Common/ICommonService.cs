using Microsoft.AspNetCore.Http;

namespace Course.Services.Common;

public interface ICommonService
{
     Task<string> UploadImageAsync(IFormFile file, string folderName);
}