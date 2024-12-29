using ImageMagick;
using PaSharper.Data;

namespace PaSharper.Utilities.IO;

public static class ImageResourceFinder
{
    public static List<string> SaveImages(Question question, List<IMagickImage<ushort>> images, string libraryRoot, bool isAnswerImage)
        {
            if (question == null) throw new ArgumentNullException(nameof(question));
            if (images == null) throw new ArgumentNullException(nameof(images));
            if (string.IsNullOrWhiteSpace(libraryRoot)) throw new ArgumentException("Library root cannot be null or empty.", nameof(libraryRoot));

            // 构建基本保存路径
            var examSystemName = question.ExamPaper.ExamSession.Subject.ExamSystemLibrary.Name;
            var subjectIndex = question.ExamPaper.ExamSession.Subject.SubjectIndex;
            var year = question.ExamPaper.ExamSession.Year;
            var season = question.ExamPaper.ExamSession.Season;
            var paperCode = question.ExamPaper.PaperCode;
            var paperVersion = question.ExamPaper.PaperVersion;

            var baseDirectoryPath = Path.Combine(libraryRoot, examSystemName, subjectIndex, year.ToString(), season, paperCode, paperVersion);
            Directory.CreateDirectory(baseDirectoryPath); // 创建基本目录

            // 查找当前目录下的最大数字
            int newFolderIndex = 1; // 默认从1开始
            var existingDirectories = Directory.GetDirectories(baseDirectoryPath);
            foreach (var dir in existingDirectories)
            {
                var dirName = Path.GetFileName(dir);
                if (int.TryParse(dirName, out int index) && index >= newFolderIndex)
                {
                    newFolderIndex = index + 1; // 更新为最大数字加1
                }
            }

            // 创建新的子目录
            var finalDirectoryPath = Path.Combine(baseDirectoryPath, newFolderIndex.ToString());
            Directory.CreateDirectory(finalDirectoryPath); // 创建新的子目录

            var savedFilePaths = new List<string>(); // 存储保存的文件路径

            // 保存每个图片
            for (int i = 0; i < images.Count; i++)
            {
                var imageTag = isAnswerImage ? "Answer" : "Question"; // 根据 isAnswerImage 确定标签
                var imagePath = Path.Combine(finalDirectoryPath, $"{i + 1}.{imageTag}.jpeg");
                images[i].Write(imagePath); // 保存图片
                savedFilePaths.Add(imagePath); // 添加到路径列表
            }

            return savedFilePaths; // 返回保存的文件路径列表
        }
}