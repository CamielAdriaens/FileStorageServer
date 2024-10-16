using MongoDB.Bson;
using FileStorage.Repositories;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FileStorage.Services
{
    public class FileService : IFileService
    {
        private readonly IFileRepository _fileRepository;

        public FileService(IFileRepository fileRepository)
        {
            _fileRepository = fileRepository;
        }

        public async Task<List<object>> GetFilesAsync()
        {
            var files = await _fileRepository.GetFilesAsync();
            var fileInfos = new List<object>();

            foreach (var file in files)
            {
                fileInfos.Add(new
                {
                    Id = file.Id.ToString(),
                    FileName = file.Filename,
                    UploadDate = file.UploadDateTime,
                    Length = file.Length
                });
            }

            return fileInfos;
        }

        public async Task<ObjectId> UploadFileAsync(Stream sourceStream, string fileName)
        {
            return await _fileRepository.UploadFileAsync(sourceStream, fileName);
        }

        public async Task<Stream> DownloadFileAsync(ObjectId fileId)
        {
            return await _fileRepository.DownloadFileAsync(fileId);
        }

        public async Task DeleteFileAsync(ObjectId fileId)
        {
            await _fileRepository.DeleteFileAsync(fileId);
        }
    }
}
