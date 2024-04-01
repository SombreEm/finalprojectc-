using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.IO;
using System.Xml.Serialization;
using static finalprojectc_.Task;

namespace finalprojectc_
{

    public class Task
    {
        public string name { get; set; }
        public string description { get; set; }
        public DateTime deadline { get; set; }
        public Priority priority { get; set; }
        public Status status { get; set; }
        public enum Priority
        {
            Low,
            Medium,
            High
        }
        public enum Status
        {
            Done,
            notDone
        }
        public Task(){ }
        public Task(string name, string description, DateTime deadline, Priority priority)
        {
            this.name = name;
            this.description = description;
            this.deadline = deadline;
            this.priority = priority;
            status = Status.notDone;
        }

        public override string ToString()
        {
            return $"Назва: {name}\nОпис: {description}\nДедлайн: {deadline.ToString("yyyy.MM.dd")}\nПріоритет: {priority}\nСтатус: {status}";
        }
    }
    public class TaskPlanner
    {
        private List<Task> tasks = new List<Task>();
        public void addTask(Task task)
        {
            tasks.Add(task);
        }
        public void deleteTask(Task task)
        {
            tasks.Remove(task);
        }
        public void editTask(Task task, string newName, string newDescription, DateTime newDeadline, Priority newPriority, Status newStatus)
        {
            task.name = newName;
            task.description = newDescription;
            task.deadline = newDeadline;
            task.priority = newPriority;
            task.status = newStatus;
        }
        public void saveTask(string task)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<Task>));
            using (StreamWriter text = new StreamWriter(task))
            {
                serializer.Serialize(text, tasks);
            }
        }
        public List<Task> loadTask(string task)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<Task>));
            using (StreamReader text = new StreamReader(task))
            {
                return (List<Task>)serializer.Deserialize(text);
            }
        }
        public void printTasks()
        {
            foreach (Task task in tasks)
            {
                Console.WriteLine(task);
                Console.WriteLine();
            }
        }
    }
    internal class Program
    {
        static void Main(string[] args)
        {
            TaskPlanner exercise = new TaskPlanner();
            exercise.addTask(new Task("Написати курсову роботу", "Пишите від руки на аркуші паперу А4 формату, НЕ СПИСУВАТИ З ЧАТА GPT", new DateTime(2023, 01, 25), Task.Priority.High));
            exercise.addTask(new Task("Дз", "Доробити дз котре ще не доробили", new DateTime(2024, 04, 5), Task.Priority.Medium));
            exercise.saveTask("tasks.xml");
            exercise.printTasks();
            List<Task> tasks = exercise.loadTask("tasks.xml");
            exercise.editTask(tasks[0], "Написати курсову роботу", "Пишите від руки на аркуші паперу А4 формату, НЕ СПИСУВАТИ З ЧАТА GPT", new DateTime(2023, 01, 26), Task.Priority.Low, Task.Status.Done);
            exercise.deleteTask(tasks[1]);
        }
    }
}
