using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Xml.XmlConfiguration;
using System.IO;
using System.Net;
using System.Linq;
using System.Text;
using System.Xml;

namespace AOJCS
{
  public class AOJAccount
  {
    public readonly string UserName, PassWord;

    public AOJAccount(string UserName, string PassWord)
    {
      this.UserName = UserName;
      this.PassWord = PassWord;
    }

    public List<string> GetSolvedList()
    {
      return Util.GetSolvedList(UserName);
    }

    public List<string> GetDiffList(string RivalUserName)
    {
      return Util.GetDiffList(UserName, RivalUserName);
    }

    public List<string> GetDiffList(AOJAccount RivalUser)
    {
      return Util.GetDiffList(UserName, RivalUser.UserName);
    }

    public uint GetSolvedCount()
    {
      return Util.GetSolvedCount(UserName);
    }

    public string Submit(string ProblemNo, string Language, string SourceCode)
    {
      return Util.Submit(this, ProblemNo, Language, SourceCode);
    }

    public string Submit(AOJProblem Problem, string Language, string SourceCode)
    {
      return Util.Submit(UserName, PassWord, Problem, Language, SourceCode);
    }
  }
}
