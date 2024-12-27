using System.ComponentModel;
using System.Linq.Expressions;
using PaSharper.Interfaces;

public class IGCSE_ExamItem : IFileMappable<IGCSE_ExamItem>
{
    public IGCSE_Subject Subject { get; set; }
    public IGCSE_PaperType File { get; set; }
    public IGCSE_ExamTime ExamTime { get; set; }
    public short PaperNumber { get; set; }
    public short PaperVersion { get; set; }

    public List<(string keyword, Expression<Func<IGCSE_ExamItem, object>> property, Func<string, object> transformer)> GetMapping()
    {
        return new List<(string, Expression<Func<IGCSE_ExamItem, object>>, Func<string, object>)>
        {
            ("SubjectID", item => item.Subject.SubjectID, x => x.ToString()), 
            ("Name", item => item.Subject.Name, x => x),
            ("Year", item => item.ExamTime.Year, x => (int.Parse(x) + 2000).ToString()), 
            ("Season", item => item.ExamTime.Season, x =>
            {
                return x switch
                {
                    "s" => IGCSE_ExamSeason.MayJun, // "s" -> MayJun
                    "m" => IGCSE_ExamSeason.FebMar, // "m" -> FebMar
                    "w" => IGCSE_ExamSeason.OctNov, // "w" -> OctNov
                };
            }), 
            ("PaperType", item => item.File, x =>
            {
                return x switch
                {
                    "ms" => IGCSE_PaperType.Answer, 
                    "qp" => IGCSE_PaperType.Question,
                };
            }), 
            ("PaperNumber", item => item.PaperNumber, x => x),
            ("PaperVersion", item => item.PaperVersion, x => x)
        };
    }
}

public class IGCSE_Subject
{
    public string Name { get; set; }
    public string SubjectID { get; set; }
}

public class IGCSE_FileItem
{
    public string Path { get; set; }
    public IGCSE_PaperType Type { get; set; }
}

public enum IGCSE_PaperType
{
    [Description("试题")]
    Question,
    [Description("答案")]
    Answer
}

public class IGCSE_ExamTime
{
    public string Year { get; set; }
    public IGCSE_ExamSeason Season { get; set; }
}

public enum IGCSE_ExamSeason
{
    [Description("冬考")]
    OctNov,
    [Description("春考")]
    FebMar,
    [Description("夏考")]
    MayJun
}
