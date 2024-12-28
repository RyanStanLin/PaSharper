using System.ComponentModel;
using System.Linq.Expressions;
using PaSharper.Extensions;
using PaSharper.Interfaces;

namespace PaSharper.Data.Base;

public class IgcseExamFileInfo : IFileMappable<IgcseExamFileInfo>, IFilePairable<IgcseExamFileInfo>
{
    public IgcseSubject Subject { get; set; }
    public IgcsePaperType FileType { get; set; }
    public FileInfo File { get; set; }
    public IgcseExamTime ExamTime { get; set; }
    public short PaperNumber { get; set; }
    public short PaperVersion { get; set; }

    public List<(string keyword, Expression<Func<IgcseExamFileInfo, object>> property, Func<string, object> transformer
        )> GetMapping()
    {
        return new List<(string, Expression<Func<IgcseExamFileInfo, object>>, Func<string, object>)>
        {
            ("SubjectID", item => item.Subject.SubjectID, x => x.ToString()),
            ("Name", item => item.Subject.Name, x => x),
            ("Year", item => item.ExamTime.Year, x => (int.Parse(x) + 2000).ToString()),
            ("Season", item => item.ExamTime.Season, x =>
            {
                return x switch
                {
                    "s" => IgcseExamSeason.MayJun, // "s" -> MayJun
                    "m" => IgcseExamSeason.FebMar, // "m" -> FebMar
                    "w" => IgcseExamSeason.OctNov // "w" -> OctNov
                };
            }),
            ("PaperType", item => item.FileType, x =>
            {
                return x switch
                {
                    "ms" => IgcsePaperType.Answer,
                    "qp" => IgcsePaperType.Question
                };
            }),
            ("PaperNumber", item => item.PaperNumber, x => x),
            ("PaperVersion", item => item.PaperVersion, x => x)
        };
    }

    public bool CanPairWith(IgcseExamFileInfo other)
    {
        return this.EqualsExcept(other, x => x.FileType, y => y.File) && FileType != other.FileType;
    }
}

public class IgcseSubject
{
    public string Name { get; set; }
    public string SubjectID { get; set; }
}

public enum IgcsePaperType
{
    [Description("试题")] Question,
    [Description("答案")] Answer
}

public class IgcseExamTime
{
    public string Year { get; set; }
    public IgcseExamSeason Season { get; set; }
}

public enum IgcseExamSeason
{
    [Description("冬考")] OctNov,
    [Description("春考")] FebMar,
    [Description("夏考")] MayJun
}