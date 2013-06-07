using System.Collections.Generic;
using System.IO;

namespace ConsoleApplication1
{
    public class Exporter
    {
        private static FileStream fileStream;
        private static StreamWriter writer;
        private static string spath = @"H:\ConsoleApplication1\ConsoleApplication1\output\";
        private static PageObjectAction latestPageObject;
        private static int position;
        private static List<WriterAction> actions;

        public static void Export(string testName, List<WriterAction> actions)
        {
            position = 1;
            Exporter.actions = actions;
            openPageObject(actions[0] as PageObjectAction);
            while(position < actions.Count)
            {
                if (actions[position] is UserAction)
                {
                    if (position + 1 < actions.Count && actions[position + 1] is PageObjectAction)
                    {
                        writePageObjectItem(actions[position] as UserAction, actions[position + 1] as PageObjectAction);
                    }
                    else
                    {
                        writePageObjectItem(actions[position] as UserAction, latestPageObject);
                    }
                }else if (actions[position] is PageObjectAction)
                {
                    closePageObject();
                    openPageObject(actions[position] as PageObjectAction);
                }
                position++;
            }
            closePageObject();
            exportTest(testName);
        }

        private static UserAction getNextUserAction(int pos)
        {
            while (pos < actions.Count)
            {
                if (actions[pos] is UserAction)
                {
                    return actions[pos] as UserAction;
                }
                pos++;
            }
            return null;
        }

        private static void exportTest(string classname)
        {
            string path = spath + classname + "Test.cs";

            fileStream = File.Create(path);
            var writer = new StreamWriter(fileStream);

            writer.WriteLine("using System;");
            writer.WriteLine("using MBRegressionLibrary.Base;");
            writer.WriteLine("using MBRegressionLibrary.JohnHayes.GeneratedJunk4;");
            writer.WriteLine("using MbUnit.Framework;");
            writer.WriteLine("");
            writer.WriteLine("namespace MBRegressionLibrary.Tests.Tests.BusinessMode.JohnHayes");
            writer.WriteLine("{");
            writer.WriteLine("\tinternal class " + classname + "Test : AbstractBusinessModeTestSuite");
            writer.WriteLine("\t{");
            writer.WriteLine("\t\t[Test]");
            writer.WriteLine("\t\tpublic void Run" + classname + "Test()");
            writer.WriteLine("\t\t{");
            writer.WriteLine(
                "\t\t\tSession.NavigateTo<LoggedOutPage>(\"https://dev7.mindbodyonline.com/ASP/adm/home.asp?studioid=-40000\");");

            for (int i = 0; i < actions.Count; i++)
            {
                if (actions[i] is UserAction)
                {
                    UserAction act = actions[i] as UserAction;
                    if (act.Node.ToLower() == "input" && act.Type.ToLower() == "checkbox")
                    {
                        writer.WriteLine("\t\t\tSession.CurrentBlock<" + latestPageObject.Name + ">()." + act.Name +
                                         ".Toggle();");
                    }
                    else if (act.Node.ToLower() == "input" && act.Type.ToLower() != "button")
                    {
                        writer.WriteLine("\t\t\tSession.CurrentBlock<" + latestPageObject.Name + ">()." + act.Name +
                                         ".EnterText(\"" + getNextUserAction(i + 1).Text + "\");");
                    }
                    else
                    {
                        writer.WriteLine("\t\t\tSession.CurrentBlock<" + latestPageObject.Name + ">()." + act.Name +
                                         ".Click();");
                    }

                    writer.WriteLine("\t\t\tSession.Driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(1));");
                }else if (actions[i] is PageObjectAction)
                {
                    latestPageObject = actions[i] as PageObjectAction;
                }
            }

            writer.WriteLine("\t\t}");
            writer.WriteLine("\t}");
            writer.WriteLine("}");

            writer.Close();
            fileStream.Close();
        }

        private static void openPageObject(PageObjectAction newPage)
        {
            string path = spath + newPage.Name + ".cs";
            fileStream = File.Create(path);
            writer = new StreamWriter(fileStream);

            writer.WriteLine("using Bumblebee.Implementation;");
            writer.WriteLine("using Bumblebee.Interfaces;");
            writer.WriteLine("using Bumblebee.Setup;");
            writer.WriteLine("using MBRegressionLibrary.Base;");
            writer.WriteLine("using OpenQA.Selenium;");
            writer.WriteLine("");
            writer.WriteLine("namespace MBRegressionLibrary.JohnHayes.GeneratedJunk4");
            writer.WriteLine("{");
            writer.WriteLine("\tpublic class " + newPage.Name + " : BusinessModePage");
            writer.WriteLine("\t{");
            writer.WriteLine("\t\tpublic " + newPage.Name + "(Session session)");
            writer.WriteLine("\t\t\t: base(session)");
            writer.WriteLine("\t\t{");
            writer.WriteLine("\t\t}");

            latestPageObject = newPage;
        }

        private static void closePageObject()
        {
            writer.WriteLine("\t}");
            writer.WriteLine("}");

            writer.Close();
            fileStream.Close();
        }

        private static void writePageObjectItem(UserAction act, PageObjectAction to)
        {
            writer.WriteLine("");

            if (act.Node.ToLower() == "input" && act.Type.ToLower() == "checkbox")
            {
                writer.WriteLine("\t\tpublic ICheckbox<" + to.Name + "> " + act.Name);
                writer.WriteLine("\t\t{");
                writer.WriteLine("\t\t\tget { return new Checkbox<" + to.Name + ">(this, By.XPath(\"" +
                                 act.Path.Replace("\"", "\\\"") + "\")); }");
            }
            else if (act.Node.ToLower() == "input" && act.Type.ToLower() != "button")
            {
                writer.WriteLine("\t\tpublic ITextField<" + to.Name + "> " + act.Name);
                writer.WriteLine("\t\t{");
                writer.WriteLine("\t\t\tget { return new TextField<" + to.Name + ">(this, By.XPath(\"" +
                                 act.Path.Replace("\"", "\\\"") + "\")); }");
            }
            else
            {
                writer.WriteLine("\t\tpublic IClickable<" + to.Name + "> " + act.Name);
                writer.WriteLine("\t\t{");
                writer.WriteLine("\t\t\tget { return new Clickable<" + to.Name + ">(this, By.XPath(\"" +
                                 act.Path.Replace("\"", "\\\"") + "\")); }");
            }

            writer.WriteLine("\t\t}");
        }
    }
}