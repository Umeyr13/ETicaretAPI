
using ETicaretAPI.Infrastructure.Operations;
using System.Text.RegularExpressions;

namespace ETicaretAPI.Infrastructure.Services
{
    public class FileService
    {
        public async Task<string> FileRenameAsync(string path,string fileName)
        {
            string NewName = await Task.Run<string>(async () => {

                string extension = Path.GetExtension(fileName);
                string oldName = Path.GetFileNameWithoutExtension(fileName);
                string newFileName = $"{NameOperation.CharacterRegulatory(oldName)}";

                int count = 1;
                
                 while (File.Exists($"{path}\\{newFileName}{extension}"))
                 {
                    var matches = Regex.Matches(newFileName, @"-(\d+)$");
                    if (matches.Count > 0)
                    {
                        count = int.Parse(matches[0].Groups[1].Value) + 1;
                        newFileName = Regex.Replace(newFileName, @"-\d+$", "");
                    }
                    newFileName = $"{newFileName}-{count}";
                    
                 }

                string newPath = Path.Combine(path,newFileName);
                            
              
              return newFileName+extension;
                       
            });

         return NewName;
        }

    }
}
