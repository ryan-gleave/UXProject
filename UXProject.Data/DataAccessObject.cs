using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UXProject.Data
{
    public class DataAccessObject
    {
        public static List<Test> GetAllTests()
        {
            using (var db = new UXProjectModelContext())
            {
                return db.Tests
                    .Include("Tasks")
                    .ToList();
            }
        }

        public static List<TaskResult> GetAllTaskResults(Task task)
        {
            using (var db = new UXProjectModelContext())
            {
                return db.TaskResults
                    .Where(i => i.TaskId == task.TaskId)
                    .ToList();
            }
        }

        public static List<Task> GetTaskResults(Test test)
        {
            using (var db = new UXProjectModelContext())
            {
                return db.Tasks
                    .Include("TaskResults")
                    .Where(i => i.TestId == test.TestId)
                    .ToList();
            }
        }



        public static bool TestConnection()
        {
            UXProjectModelContext db = null;
            try
            {
                db = new UXProjectModelContext();
                db.Participants.FirstOrDefault();
                db.Dispose();
                return true;
            }
            catch (Exception ex)
            {
                if (db != null) db.Dispose();
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public static Participant GetParticipant(string pass)
        {
            using (var db = new UXProjectModelContext())
            {
                return db.Participants
                    .Include(b => b.Test)
                    .Where(i => i.Password.Equals(pass))
                    .FirstOrDefault();
            }
        }

        public static List<Task> GetTasks(Test test)
        {
            using (var db = new UXProjectModelContext())
            {
                var t = db.Tests.Where(i => i.TestId == test.TestId).FirstOrDefault();
                return t.Tasks.ToList();
            }
        }

        public static void SaveTestResult(TestResult res)
        {
            using (var db = new UXProjectModelContext())
            {
                db.TestResults.Add(res);
                db.SaveChanges();
            }
        }

        public static Participant CreateParticipant(string pass)
        {
            using (var db = new UXProjectModelContext())
            {
                Participant s = new Participant() { Password = pass, };
                db.Participants.Add(s);
                db.SaveChanges();
                return s;
            }
        }

    }
}
