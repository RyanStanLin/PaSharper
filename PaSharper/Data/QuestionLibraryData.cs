namespace PaSharper.Data;

/// <summary>
///     考试系统实体，例如 SAT, ALevel
/// </summary>
public class ExamSystemLibrary
{
    public int ExamSystemId { get; set; } // 主键
    public string Name { get; set; } // 考试系统的名称
    public string Description { get; set; } // 描述考试系统的功能或特点

    // 导航属性：与 Subject 表的一对多关系
    public List<Subject> Subjects { get; set; } = new();
}

/// <summary>
///     科目实体，例如 SAT 数学，ALevel 化学
/// </summary>
public class Subject
{
    public int SubjectId { get; set; } // 主键
    public int ExamSystemId { get; set; } // 外键，指向 ExamSystem
    public string Name { get; set; } // 科目名称
    public string Description { get; set; } // 科目描述

    // 导航属性：指向所属的考试系统
    public ExamSystemLibrary ExamSystemLibrary { get; set; }

    // 导航属性：与 ExamSession 表的一对多关系
    public List<ExamSession> ExamSessions { get; set; } = new();
}

/// <summary>
///     考试场次实体，例如 2024 年春季的 SAT 数学考试
/// </summary>
public class ExamSession
{
    public int ExamSessionId { get; set; } // 主键
    public int SubjectId { get; set; } // 外键，指向 Subject
    public int Year { get; set; } // 考试年份
    public string Season { get; set; } // 考试季节，例如 Spring, Winter
    public string AISummary { get; set; } // AI 自动总结字段
    public string Description { get; set; } // 对考试场次的描述

    // 导航属性：指向所属科目
    public Subject Subject { get; set; }

    // 导航属性：与 ExamPaper 表的一对多关系
    public List<ExamPaper> ExamPapers { get; set; } = new();
}

/// <summary>
///     试卷实体，例如 2024 春季 A 版试卷
/// </summary>
public class ExamPaper
{
    public int ExamPaperId { get; set; } // 主键
    public int ExamSessionId { get; set; } // 外键，指向 ExamSession
    public string PaperVersion { get; set; } // 试卷版本（例如 A, B, C）
    public string PaperCode { get; set; } // 试卷编号（例如 SAT2024A01）
    public string Description { get; set; } // 对试卷的额外描述
    public string PaperPdfUrl { get; set; } // 试卷 PDF 文件的下载链接
    public string AnswerPdfUrl { get; set; } // 答案 PDF 文件的下载链接

    // 导航属性：指向所属考试场次
    public ExamSession ExamSession { get; set; }

    // 导航属性：与 Question 表的一对多关系
    public List<Question> Questions { get; set; } = new();
}

/// <summary>
///     题目实体，描述具体的题目信息
/// </summary>
public class Question
{
    public int QuestionId { get; set; } // 主键
    public int ExamPaperId { get; set; } // 外键，指向 ExamPaper
    public string AISummary { get; set; } // AI 自动总结字段

    // 导航属性：指向所属的试卷
    public ExamPaper ExamPaper { get; set; }

    // 导航属性：与 QuestionImage 表的一对多关系
    public List<QuestionImage> QuestionImages { get; set; } = new();

    // 导航属性：与 AnswerImage 表的一对多关系
    public List<AnswerImage> AnswerImages { get; set; } = new();
}

/// <summary>
///     题目图片实体，存储题目所包含的图片
/// </summary>
public class QuestionImage
{
    public int ImageId { get; set; } // 主键
    public int QuestionId { get; set; } // 外键，指向 Question
    public string ImageUrl { get; set; } // 图片的 URL

    // 导航属性：指向所属题目
    public Question Question { get; set; }
}

/// <summary>
///     答案图片实体，存储答案所包含的图片
/// </summary>
public class AnswerImage
{
    public int ImageId { get; set; } // 主键
    public int QuestionId { get; set; } // 外键，指向 Question
    public string ImageUrl { get; set; } // 图片的 URL

    // 导航属性：指向所属题目
    public Question Question { get; set; }
}