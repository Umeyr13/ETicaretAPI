using ETicaretAPI.Application.Abstractions.Stroge.Local;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Infrastructure.Services.Storage.Local
{
    public class LocalStorage :Storage, ILocalStorage
    {
        readonly IWebHostEnvironment _webHostEnvironment;

        public LocalStorage(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task DeleteAsync(string path, string fileName)
          =>  File.Delete($"{ path}\\{fileName}");
        

        public List<string> GetFiles(string path)
        {
            DirectoryInfo directory = new (path);
            return directory.GetFiles().Select(f=>f.Name).ToList();
        }

        public bool HasFile(string path, string fileName)
            =>File.Exists($"{path}\\{fileName}");

        private async Task<bool> CopyFileAsync(string path, IFormFile file)
        {
            try
            {
                await using FileStream fileStream = new(path, FileMode.Create, FileAccess.Write, FileShare.None, 1024 * 1024, useAsync: false); // buradaki using CopyFileAsync metodu bitene kadar ilgili nesneyi tutmamızı yanı dispose edilme mesini sağlar. ne zaman ki metot biter o zaman dispose edilir.
                await file.CopyToAsync(fileStream);
                await fileStream.FlushAsync();//temizleme
                return true;
            }
            catch (Exception ex)
            {
                //todo log!
                throw ex;
            }

        }

        public async Task<List<(string fileName, string pathOrContainerName)>> UploadAsync(string path, IFormFileCollection files)
        {

            string uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, path);//wwwroot/resource/product-images => wwwroot a kadar getirdi devamını biz ekledik
            if (!Directory.Exists(uploadPath))
                Directory.CreateDirectory(uploadPath);

            List<(string fileName, string path)> datas = new();
            

            foreach (IFormFile file in files)//Form data olarak gelen dosyaların her birini alıyoruz..(file ler parametre olarak gelimyor request in içindeki Formdata olarak geliyor)
            {
                //string fileNewName = await FileRenameAsync(uploadPath, file.FileName);
                string fileNewName = await FileRenameAsync(path, file.Name, HasFile);
                await CopyFileAsync($"{uploadPath}\\{fileNewName}", file);
                datas.Add((fileNewName, $"{path}\\{fileNewName}"));
                
            }

            //if (results.TrueForAll(r => r.Equals(true)))
            //{
            //    return datas;// hepsi doğruysa yani gönderilen bütün dosyalar yüklendi ise eklenenlerin dosya ismi ve adres bilgileri bir Liste olarak geri gönderildi.
            //}
            return datas;


            //todo Eğer ki yukarıdaki if geçerli değilse burada dosyaların sunucuda yüklenirken hata alındığına dair bir uyarıcı exception oluşturulup fırlatılması gerekiyor!
        }
    }
}
