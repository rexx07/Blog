using Blog.Services.Extensions;
using Blog.Services.Utils.Interface;
using Microsoft.AspNetCore.Http;
using Serilog;

namespace Blog.Services.Utils.Service;

public class StorageService : IStorageService
{
    private readonly string _slash = Path.DirectorySeparatorChar.ToString();
    private readonly string _storageRoot;

    public StorageService()
    {
        _storageRoot = $"{ContentRoot}{_slash}wwwroot{_slash}data{_slash}";
    }

    private string ContentRoot
    {
        get
        {
            var path = Directory.GetCurrentDirectory();
            var testsDirectory = $"tests{_slash}Blog.Tests";
            var appDirectory = $"src{_slash}Blog.Web";

            Log.Information($"Current directory path: {path}");

            // development unit test run
            if (path.LastIndexOf(testsDirectory) > 0)
            {
                path = path.Substring(0, path.LastIndexOf(testsDirectory));
                Log.Information($"Unit test path: {path}src{_slash}Blog.Web");
                return $"{path}src{_slash}Blog.Web";
            }

            // this needed to make sure we have correct data directory
            // when running in debug mode in Visual Studio
            // so instead of debug (src/Blogifier/bin/Debug..)
            // will be used src/Blogifier/wwwroot/data
            // !! this can mess up installs that have "src/Blogifier" in the path !!
            if (path.LastIndexOf(appDirectory) > 0)
            {
                path = path.Substring(0, path.LastIndexOf(appDirectory));
                Log.Information($"Development debug path: {path}src{_slash}Blog.Web");
                return $"{path}src{_slash}Blog.Web";
            }

            Log.Information($"Final path: {path}");
            return path;
        }
    }

    public bool FileExists(string path)
    {
        Log.Information($"File exists: {Path.Combine(ContentRoot, path)}");
        return File.Exists(Path.Combine(ContentRoot, path));
    }

    public async Task<bool> UploadFormFile(IFormFile file, string path = "")
    {
        path = path.Replace("/", _slash);
        VerifyPath(path);

        var fileName = GetFileName(file.FileName);
        var filePath = string.IsNullOrEmpty(path)
            ? Path.Combine(_storageRoot, fileName)
            : Path.Combine(_storageRoot, path + _slash + fileName);

        Log.Information($"Storage root: {_storageRoot}");
        Log.Information($"Uploading file: {filePath}");
        try
        {
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
                Log.Information($"Uploaded file: {filePath}");
            }
        }
        catch (Exception ex)
        {
            Log.Error($"Error uploading file: {ex.Message}");
        }

        return true;
    }

    public async Task<string> UploadFromWeb(Uri requestUri, string root, string path = "")
    {
        path = path.Replace("/", _slash);
        VerifyPath(path);

        var fileName = TitleFromUri(requestUri);
        var filePath = string.IsNullOrEmpty(path)
            ? Path.Combine(_storageRoot, fileName)
            : Path.Combine(_storageRoot, path + _slash + fileName);

        var client = new HttpClient();
        var response = await client.GetAsync(requestUri);
        using (var fs = new FileStream(filePath, FileMode.CreateNew))
        {
            await response.Content.CopyToAsync(fs);
            return await Task.FromResult($"![{fileName}]({root}{PathToUrl(filePath)})");
        }
    }

    public async Task<string> UploadBase64Image(string baseImg, string root, string path = "")
    {
        path = path.Replace("/", _slash);
        var fileName = "";

        VerifyPath(path);
        var imgSrc = GetImgSrcValue(baseImg);

        var rnd = new Random();

        if (imgSrc.StartsWith("data:image/png;base64,"))
        {
            fileName = string.Format("{0}.png", rnd.Next(1000, 9999));
            imgSrc = imgSrc.Replace("data:image/png;base64,", "");
        }

        if (imgSrc.StartsWith("data:image/jpeg;base64,"))
        {
            fileName = string.Format("{0}.jpeg", rnd.Next(1000, 9999));
            imgSrc = imgSrc.Replace("data:image/jpeg;base64,", "");
        }

        if (imgSrc.StartsWith("data:image/gif;base64,"))
        {
            fileName = string.Format("{0}.gif", rnd.Next(1000, 9999));
            imgSrc = imgSrc.Replace("data:image/gif;base64,", "");
        }

        var filePath = string.IsNullOrEmpty(path)
            ? Path.Combine(_storageRoot, fileName)
            : Path.Combine(_storageRoot, path + _slash + fileName);

        await File.WriteAllBytesAsync(filePath, Convert.FromBase64String(imgSrc));

        return $"![{fileName}]({root}{PathToUrl(filePath)})";
    }

    private string GetFileName(string fileName)
    {
        // some browsers pass uploaded file name as short file name 
        // and others include the path; remove path part if needed
        if (fileName.Contains(_slash))
        {
            fileName = fileName.Substring(fileName.LastIndexOf(_slash));
            fileName = fileName.Replace(_slash, "");
        }

        // when drag-and-drop or copy image to TinyMce editor
        // it uses "mceclip0" as file name; randomize it for multiple uploads
        if (fileName.StartsWith("mceclip0"))
        {
            var rnd = new Random();
            fileName = fileName.Replace("mceclip0", rnd.Next(100000, 999999).ToString());
        }

        return fileName.SanitizePath();
    }

    private void VerifyPath(string path)
    {
        path = path.SanitizePath();

        if (!string.IsNullOrEmpty(path))
        {
            var dir = Path.Combine(_storageRoot, path);

            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
        }
    }

    private string TitleFromUri(Uri uri)
    {
        var title = uri.ToString().ToLower();
        title = title.Replace("%2f", "/");

        if (title.EndsWith(".axdx")) title = title.Replace(".axdx", "");

        if (title.Contains("image.axd?picture=")) title = title.Substring(title.IndexOf("image.axd?picture=") + 18);

        if (title.Contains("file.axd?file=")) title = title.Substring(title.IndexOf("file.axd?file=") + 14);

        if (title.Contains("encrypted-tbn") || title.Contains("base64,"))
        {
            var rnd = new Random();
            title = string.Format("{0}.png", rnd.Next(1000, 9999));
        }

        if (title.Contains("/")) title = title.Substring(title.LastIndexOf("/"));

        title = title.Replace(" ", "-");

        return title.Replace("/", "").SanitizeFileName();
    }

    private string PathToUrl(string path)
    {
        var url = path.ReplaceIgnoreCase(_storageRoot, "").Replace(_slash, "/");
        return $"data/{url}";
    }

    private string GetImgSrcValue(string imgTag)
    {
        if (!(imgTag.Contains("data:image") && imgTag.Contains("src=")))
            return imgTag;

        var start = imgTag.IndexOf("src=");
        var srcStart = imgTag.IndexOf("\"", start) + 1;

        if (srcStart < 2)
            return imgTag;

        var srcEnd = imgTag.IndexOf("\"", srcStart);

        if (srcEnd < 1 || srcEnd <= srcStart)
            return imgTag;

        return imgTag.Substring(srcStart, srcEnd - srcStart);
    }
}