using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Xml.Linq;
using System.Xml.XmlConfiguration;
using System.IO;
using System.Net;
using System.Linq;
using System.Text;
using System.Xml;
using System.Threading;

namespace AOJCS
{
  public class AojSubmitException : Exception
  {
    public AojSubmitException()
    {
    }

    public AojSubmitException(string message) : base(message)
    {
    }

    public AojSubmitException(string message, Exception innerException) : base(message, innerException)
    {
    }
  }

  public class Util
  {
    private const string SolvedResponceUrl = @"http://judge.u-aizu.ac.jp/onlinejudge/webservice/user?id=";
    private static readonly Encoding encoding = Encoding.GetEncoding("Shift_JIS");
    private static readonly TimeSpan TimeOut = new TimeSpan(0, 0, 10);
    private static readonly TimeSpan MaxWatingJudgeTime = new TimeSpan(0, 0, 20);
    private const string GetLastRunIdUrl = @"http://judge.u-aizu.ac.jp/onlinejudge/webservice/status_log?user_id=";
    private const string SubmitEndpoint = @"http://judge.u-aizu.ac.jp/onlinejudge/servlet/Submit";

    private static string GetBeforeRunId(string UserName)
    {
      string id = "";
      using (XmlReader reader = XmlReader.Create(GetLastRunIdUrl + UserName))
      {
        if (reader.ReadToFollowing("run_id"))
        {
          id = reader.ReadString().Replace("\n", "");
        }
      }
      return id;
    }

    private static HttpWebRequest BuildRequest(byte[] data)
    {
      HttpWebRequest request = WebRequest.Create(SubmitEndpoint) as HttpWebRequest;

      request.Method = "POST";
      request.Timeout = (int)TimeOut.TotalMilliseconds;
      request.ContentType = "application/x-www-form-urlencoded";
      request.ContentLength = data.Length;

      return request;
    }

    private static string GetJudgeResult(string UserName, string BeforeRunId)
    {
      int challanged = 0;
      bool success = false;
      while (challanged <= (MaxWatingJudgeTime.TotalMilliseconds / 200))
      {
        if (BeforeRunId != GetBeforeRunId(UserName))
        {
          success = true;
          break;
        }
        Thread.Sleep(200);
        challanged++;
      }
      if (!success)
      {
        throw new AojSubmitException();
      }

      return GetResult(UserName);
    }

    private static string GetResult(string UserName)
    {
      string result = "";
      using (XmlReader reader = XmlReader.Create(GetLastRunIdUrl + UserName))
      {
        if (reader.ReadToFollowing("status") && reader.ReadToFollowing("status"))
        {
          result = reader.ReadString().Replace("\n", "");
        }
      }
      return result;
    }

    public static byte[] BuildSubmitData(string UserName, string PassWord, string ProblemNo, string Language,
      string SourceCode)
    {
      Hashtable submitConfig = new Hashtable();
      submitConfig["userID"] = WebUtility.UrlEncode(UserName);
      submitConfig["sourceCode"] = WebUtility.UrlEncode(SourceCode);
      submitConfig["problemNO"] = WebUtility.UrlEncode(ProblemNo);
      submitConfig["language"] = WebUtility.UrlEncode(Language);
      submitConfig["password"] = WebUtility.UrlEncode(PassWord);
      var submitParam = submitConfig.Keys.Cast<string>()
        .Aggregate("", (current, key) => current + String.Format("{0}={1}&", key, submitConfig[key]));
      return encoding.GetBytes(submitParam);
    }

    public static List<string> GetSolvedList(string UserName)
    {
      List<string> SolvedList = new List<string>();
      using (XmlReader reader = XmlReader.Create(SolvedResponceUrl + UserName))
      {
        reader.ReadToFollowing("problem");
        while (reader.Read())
        {
          if (reader.ReadToFollowing("id"))
          {
            SolvedList.Add(reader.ReadString().Replace("\n", ""));
          }
        }
      }
      return SolvedList;
    }

    public static List<string> GetDiffList(string UserName1, string UserName2)
    {
      List<string> User1List = GetSolvedList(UserName1);
      return GetSolvedList(UserName2).Where(id => !User1List.Contains(id)).ToList();
    }

    public static uint GetSolvedCount(string UserName)
    {
      uint SolvedCount = 0;
      using (XmlReader reader = XmlReader.Create(SolvedResponceUrl + UserName))
      {
        while (reader.Read())
        {
          if (reader.ReadToFollowing("solved"))
          {
            SolvedCount = uint.Parse(reader.ReadString());
          }
        }
      }
      return SolvedCount;

    }

    public static string Submit(string UserName, string PassWord, string ProblemNo, string Language, string SourceCode)
    {
      string BeforeRunId = GetBeforeRunId(UserName);
      var data = BuildSubmitData(UserName, PassWord, ProblemNo, Language, SourceCode);
      var submitRequest = BuildRequest(data);
      using (Stream submitReqStream = submitRequest.GetRequestStream())
      {
        submitReqStream.Write(data, 0, data.Length);
      }

      try
      {
        return GetJudgeResult(UserName, BeforeRunId);
      }
      catch (Exception e)
      {
        throw new AojSubmitException("", e);
      }
    }

    public static string Submit(AOJAccount Account, string ProblemNo, string Language, string SourceCode)
    {
      return Submit(Account.UserName, Account.PassWord, ProblemNo, Language, SourceCode);
    }

    public static string Submit(string UserName, string PassWord, AOJProblem Problem, string Language, string SourceCode)
    {
      return Submit(UserName, PassWord, Problem.Id, Language, SourceCode);
    }

    public static string Submit(AOJAccount Account, AOJProblem Problem, string Language, string SourceCode)
    {
      return Submit(Account.UserName, Account.PassWord, Problem.Id, Language, SourceCode);
    }
  }
}