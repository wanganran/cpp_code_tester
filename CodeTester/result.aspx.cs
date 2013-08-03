using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Diagnostics;

namespace CodeTester
{
    public class gcccompiler
    {
        public string filepath;
        public string filename;
        public string filetype;
        public string input;
        public string gccpath;
        public int timelimit;
        public string compile(ref string errors)
        {
            Process p = new Process();
            string gccexe = (filetype == "c" ? "gcc" : "g++");
            p.StartInfo.FileName = "\""+gccpath + "\\"+gccexe+"\" -o \"" + filepath + "\\" + filename + ".exe" + "\" \"" + filepath + "\\" + filename + "."+filetype+"\"";
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.CreateNoWindow = true;
            p.Start();
            System.IO.StreamReader sr = p.StandardOutput;
            System.IO.StreamReader sr2 = p.StandardError;
            p.WaitForExit(5000);
            if (System.IO.File.Exists(filepath + "\\" + filename + ".exe"))
            {
                return filepath + "\\" + filename + ".exe";
            }
            else
            {
                errors = sr2.ReadToEnd();
                return null;
            }
        }
        public string runforresult(string exefile,ref int time)
        {
            Process p = new Process();
            p.StartInfo.FileName = exefile;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.CreateNoWindow = true;
            p.Start();

            int t = System.Environment.TickCount;
            System.IO.StreamWriter sw = p.StandardInput;
            System.IO.StreamReader sr = p.StandardOutput;
            sw.Write(input);
            sw.Close();
            bool exited = false;
            System.Threading.Thread th = new System.Threading.Thread(new System.Threading.ThreadStart(delegate()
            {
                p.WaitForExit();
                exited = true;
                t = System.Environment.TickCount - t;
            }));
            th.Start();
            int restlimit = timelimit;
            while (!exited && timelimit > 0)
            {
                System.Threading.Thread.Sleep(100);
                timelimit -= 100;
            }
            if (!exited)
            {
                p.Kill();
                return null;
            }
            else
            {
                time = t;
                return sr.ReadToEnd();
            }
        }
    }
    public partial class result : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (Request["lang"] == null) Response.Redirect("~/default.aspx");
                else
                {
                    //Response.Buffer = Response.BufferOutput = false;
                    Application.Lock();
                    outputheader();
                    Response.Write(new string(' ', 200)+"<h2>Waiting..");
                    Response.Flush();
                    string lang = Request["lang"];
                    string timelimit = Request["timelimit"];
                    string code = Request["code"];
                    string input = Request["input"] + "\r\n";
                    string guid = Guid.NewGuid().ToString();
                    string path = Server.MapPath("/tmp") + "\\" + guid + "." + lang.ToLower();
                    System.IO.StreamWriter sw = new System.IO.StreamWriter(path);
                    sw.Write(code);
                    sw.Close();
                    gcccompiler c = new gcccompiler();
                    c.filename = guid;
                    c.filetype = lang.ToLower();
                    c.filepath = Server.MapPath("/tmp");
                    c.gccpath = @"D:\MinGW-4.4.1\bin";
                    c.input = input;
                    c.timelimit = (timelimit == "1s" ? 1000 : (timelimit == "5s" ? 5000 : 200));
                    string errors = "";
                    string exefile = c.compile(ref errors);
                    Response.Write(".</h2>"+new string(' ', 255));
                    Response.Flush();
                    if (exefile == null)
                    {
                        outputerror(errors);
                    }
                    else
                    {
                        int t = 0;
                        string res = c.runforresult(exefile,ref t);
                        outputresult(res,t);
                    }
                    outputfoot(c.filename, c.filetype);
                }
            }
            finally
            {
                Response.End();
                Application.UnLock();
            }
        }

        private void outputfoot(string guid,string ext)
        {
            Response.Write("<h2><a href=\"tmp/" + guid + "." + ext + "\">Download source</a>&nbsp;<a href=\"tmp/" + guid + ".exe\">Download execute file</a>&nbsp;");
            Response.Write("</body></html>");
        }

        private void outputheader()
        {
            Response.Write(@"<html><head><title>Result</title><link href=""Style.css"" type=""text/css"" rel=""Stylesheet"" /><body>");
        }

        private void outputerror(string errors)
        {
            Response.Write("<h2>There're some errors when compiling:</h2>");
            Response.Write("<p>");
            Response.Write(Server.HtmlEncode(errors).Replace("\r", "").Replace("\n", "<br/>"));
            Response.Write("</p>");
        }

        private void outputresult(string res,int t)
        {
            if (res == null)
                Response.Write("<h2>Time limit excceed.</h2>");
            else
            {
                Response.Write("<h2>Compile OK. Program output:</h2><p>");
                Response.Write(Server.HtmlEncode(res).Replace("\r", "").Replace("\n", "<br/>").Replace(" ","&nbsp;"));
                Response.Write("</p><h2>in "+t.ToString()+" ms.</h2>");
            }
        }
    }
}
