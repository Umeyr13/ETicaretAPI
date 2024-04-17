using ETicaretAPI.Infrastructure.Operations;
using System.Text.RegularExpressions;

namespace ETicaretAPI.Infrastructure.Services.Storage
{
    public class Storage
    {
        protected delegate bool HasFile(string pathOrContainerName, string fileName);
        protected  Task<string> FileRenameAsync(string pathOrContainerName, string fileName,HasFile hasFileMethod)
        {
           return Task.Run(() => {

                string extension = Path.GetExtension(fileName);
                string oldName = Path.GetFileNameWithoutExtension(fileName);
                string newFileName = $"{NameOperation.CharacterRegulatory(oldName)}";

                int count = 1;

                while (hasFileMethod(pathOrContainerName, newFileName+extension))//File.Exists($"{pathOrContainerName}\\{newFileName}{extension}")
               {
                    var matches = Regex.Matches(newFileName, @"-(\d+)$");
                    if (matches.Count > 0)
                    {
                        count = int.Parse(matches[0].Groups[1].Value) + 1;
                        newFileName = Regex.Replace(newFileName, @"-\d+$", "");
                    }
                    newFileName = $"{newFileName}-{count}";

                }

                string newPath = Path.Combine(pathOrContainerName, newFileName);

                return newFileName + extension;

            });
       
        }
    }
}
