using PaSharper.Data;

namespace PaSharper.Builders;

/// <summary>
/// 构建题库的核心类，负责管理 ExamSystemLibrary 结构
/// </summary>
public class QuestionLibraryBuilder
{
    private static readonly List<ExamSystemLibrary> ExistingLibraries = new List<ExamSystemLibrary>();
    private readonly ExamSystemLibrary _examSystemLibrary;

    /// <summary>
    /// 构造函数，初始化考试系统
    /// </summary>
    /// <param name="examSystemName">考试系统名称</param>
    /// <param name="examSystemDescription">考试系统描述</param>
    public QuestionLibraryBuilder(string examSystemName, string examSystemDescription)
    {
        if (string.IsNullOrWhiteSpace(examSystemName))
            throw new ArgumentException("考试系统名称不能为空", nameof(examSystemName));

        // 查找是否已存在相同名称的考试系统
        _examSystemLibrary = ExistingLibraries
            .FirstOrDefault(es => string.Equals(es.Name, examSystemName, StringComparison.OrdinalIgnoreCase));

        if (_examSystemLibrary == null)
        {
            // 不存在则新建
            _examSystemLibrary = new ExamSystemLibrary
            {
                Name = examSystemName.Trim(),
                Description = examSystemDescription?.Trim(),
                Subjects = new List<Subject>()
            };
            ExistingLibraries.Add(_examSystemLibrary);
        }
        else if (!string.IsNullOrWhiteSpace(examSystemDescription))
        {
            // 如果已有库存在但描述为空，则更新描述
            _examSystemLibrary.Description ??= examSystemDescription.Trim();
        }
    }

    /// <summary>
    /// 向题库中添加一道题目
    /// </summary>
    public void AddQuestion(Question question, string subjectIndex, int year, string season, string paperCode, string paperVersion)
    {
        if (question == null)
            throw new ArgumentNullException(nameof(question), "题目对象不能为空");
        if (string.IsNullOrWhiteSpace(subjectIndex))
            throw new ArgumentException("科目名称不能为空", nameof(subjectIndex));
        if (year <= 0)
            throw new ArgumentOutOfRangeException(nameof(year), "考试年份必须为正整数");
        if (string.IsNullOrWhiteSpace(season))
            throw new ArgumentException("考试季节不能为空", nameof(season));
        if (string.IsNullOrWhiteSpace(paperCode))
            throw new ArgumentException("试卷代码不能为空", nameof(paperCode));
        if (string.IsNullOrWhiteSpace(paperVersion))
            throw new ArgumentException("试卷版本不能为空", nameof(paperVersion));

        // 找到或创建科目
        var subject = FindOrCreateSubject(subjectIndex.Trim());
        // 找到或创建考试场次
        var examSession = FindOrCreateExamSession(subject, year, season.Trim());
        // 找到或创建试卷
        var examPaper = FindOrCreateExamPaper(examSession, paperCode.Trim(), paperVersion.Trim());
        // 添加题目
        AddQuestionToExamPaper(examPaper, question);
    }

    /// <summary>
    /// 获取最终构建的 ExamSystemLibrary
    /// </summary>
    public ExamSystemLibrary GetExamSystemLibrary()
    {
        return _examSystemLibrary;
    }

    // 私有方法 - 查找或创建科目
    private Subject FindOrCreateSubject(string subjectIndex)
    {
        var subject = _examSystemLibrary.Subjects
            .FirstOrDefault(s => string.Equals(s.Name, subjectIndex, StringComparison.OrdinalIgnoreCase));

        if (subject == null)
        {
            subject = new Subject
            {
                SubjectId = GenerateId(), // 自动生成唯一ID
                SubjectIndex = subjectIndex,
                //TODO:添加Index和Name对应
                ExamSystemLibrary = _examSystemLibrary,
                ExamSessions = new List<ExamSession>()
            };
            _examSystemLibrary.Subjects.Add(subject);
        }

        return subject;
    }

    // 私有方法 - 查找或创建考试场次
    private ExamSession FindOrCreateExamSession(Subject subject, int year, string season)
    {
        var examSession = subject.ExamSessions
            .FirstOrDefault(es => es.Year == year && string.Equals(es.Season, season, StringComparison.OrdinalIgnoreCase));

        if (examSession == null)
        {
            examSession = new ExamSession
            {
                ExamSessionId = GenerateId(), // 自动生成唯一ID
                Year = year,
                Season = season,
                Subject = subject,
                ExamPapers = new List<ExamPaper>()
            };
            subject.ExamSessions.Add(examSession);
        }

        return examSession;
    }

    // 私有方法 - 查找或创建试卷
    private ExamPaper FindOrCreateExamPaper(ExamSession examSession, string paperCode, string paperVersion)
    {
        var examPaper = examSession.ExamPapers
            .FirstOrDefault(ep => string.Equals(ep.PaperCode, paperCode, StringComparison.OrdinalIgnoreCase) &&
                                  string.Equals(ep.PaperVersion, paperVersion, StringComparison.OrdinalIgnoreCase));

        if (examPaper == null)
        {
            examPaper = new ExamPaper
            {
                ExamPaperId = GenerateId(), // 自动生成唯一ID
                PaperCode = paperCode,
                PaperVersion = paperVersion,
                ExamSession = examSession,
                Questions = new List<Question>()
            };
            examSession.ExamPapers.Add(examPaper);
        }

        return examPaper;
    }

    // 私有方法 - 将题目添加到试卷中
    private void AddQuestionToExamPaper(ExamPaper examPaper, Question question)
    {
        if (!examPaper.Questions.Contains(question))
        {
            question.QuestionId = GenerateId(); // 自动生成唯一ID
            examPaper.Questions.Add(question);
            question.ExamPaper = examPaper;
        }
    }

    /// <summary>
    /// 生成唯一 ID（模拟数据库自增或 GUID 模式）
    /// </summary>
    private static int _currentId;
    private static int GenerateId()
    {
        return Interlocked.Increment(ref _currentId); // 线程安全的递增 ID
    }
}