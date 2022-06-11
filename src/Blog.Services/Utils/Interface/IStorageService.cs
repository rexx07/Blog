using Microsoft.AspNetCore.Http;

namespace Blog.Services.Utils.Interface;

public interface IStorageService
{
    bool FileExists(string path);
    Task<bool> UploadFormFile(IFormFile file, string path = "");
    Task<string> UploadFromWeb(Uri requestUri, string root, string path = "");
    Task<string> UploadBase64Image(string baseImg, string root, string path = "");
}