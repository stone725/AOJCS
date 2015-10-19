using System;
using System.Xml;

namespace AOJCS
{
  public class AOJProblem
  {
    private const string GetProblemInfoUrl = @"http://judge.u-aizu.ac.jp/onlinejudge/webservice/problem?id=";
    private const string GetProblemCategoryUrl = @"http://judge.u-aizu.ac.jp/onlinejudge/webservice/problem_category?id=";

    public readonly string Id;
    public readonly string Name;
    public readonly string Category;
    public readonly uint TimeLimit;
    public readonly uint MemoryLimit;
    public readonly uint Submission;
    public readonly uint Accepted;
    public readonly uint WrongAnswer;
    public readonly uint TimeLimitExceeded;
    public readonly uint MemoryLimitExceeded;
    public readonly uint OutputLimitExceeded;
    public readonly uint RuntimeError;
    public readonly double score;

    public AOJProblem(string Id)
    {
      this.Id = Id;
      using (XmlReader ProblemInfoReader = XmlReader.Create(GetProblemInfoUrl + Id + @"&status=false"))
      {
        if (ProblemInfoReader.ReadToFollowing("name"))
          Name = ProblemInfoReader.ReadString().Replace("\n", "");

        if (ProblemInfoReader.ReadToFollowing("problemtimelimit"))
          TimeLimit = uint.Parse(ProblemInfoReader.ReadString().Replace("\n", ""));

        if (ProblemInfoReader.ReadToFollowing("problemmemorylimit"))
          MemoryLimit = uint.Parse(ProblemInfoReader.ReadString().Replace("\n", ""));

        if (ProblemInfoReader.ReadToFollowing("submission"))
          Submission = uint.Parse(ProblemInfoReader.ReadString().Replace("\n", ""));

        if (ProblemInfoReader.ReadToFollowing("accepted"))
          Accepted = uint.Parse(ProblemInfoReader.ReadString().Replace("\n", ""));

        if (ProblemInfoReader.ReadToFollowing("wronganswer"))
          WrongAnswer = uint.Parse(ProblemInfoReader.ReadString().Replace("\n", ""));

        if (ProblemInfoReader.ReadToFollowing("timelimit"))
          TimeLimitExceeded = uint.Parse(ProblemInfoReader.ReadString().Replace("\n", ""));

        if (ProblemInfoReader.ReadToFollowing("memorylimit"))
          MemoryLimitExceeded = uint.Parse(ProblemInfoReader.ReadString().Replace("\n", ""));

        if (ProblemInfoReader.ReadToFollowing("outputlimit"))
          OutputLimitExceeded = uint.Parse(ProblemInfoReader.ReadString().Replace("\n", ""));

        if (ProblemInfoReader.ReadToFollowing("runtimeerror"))
          RuntimeError = uint.Parse(ProblemInfoReader.ReadString().Replace("\n", ""));
      }
      using (XmlReader ProblemCategoryReader = XmlReader.Create(GetProblemCategoryUrl + Id))
      {
        if (ProblemCategoryReader.ReadToFollowing("category"))
          Category = ProblemCategoryReader.ReadString().Replace("\n", "");

        if (ProblemCategoryReader.ReadToFollowing("score"))
          score = double.Parse(ProblemCategoryReader.ReadString().Replace("\n", ""));
        else
          score = 0;
      }
    }

    public string Submit(string UserName, string PassWord, string Language, string SourceCode)
    {
      return Util.Submit(UserName, PassWord, this, Language, SourceCode);
    }

    public string Submit(AOJAccount Account, string Language, string SourceCode)
    {
      return Util.Submit(Account, this, Language, SourceCode);
    }
  }
}