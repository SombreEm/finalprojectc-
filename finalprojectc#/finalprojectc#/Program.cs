using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.IO;
using System.Xml.Serialization;
using static finalprojectc_.Task;
using System.Xml.Linq;

namespace finalprojectc_
{

    public class Task
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime Deadline { get; set; }
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
            New,
            InProgress,
            Completed
        }

        public Task(string name, string description, DateTime deadline, Priority priority, Status status)
        {
            this.ID = IdGenerator.GenerateId();
            this.Name = name;
            this.Description = description;
            this.Deadline = deadline;
            this.priority = priority;
            this.status = status;
        }
        public override string ToString()
        {
            return $"\nID: {ID},\n Назва: {Name},\n Опис: {Description},\n Дедлайн: {Deadline:yyyy-MM-dd},\n Пріоритет: {priority},\n Статус: {status}";
        }
    }

    public class IdGenerator
    {
        private static long lastId = 0; 
        public static string GenerateId()
        {
            long id = lastId;
            if (id <= lastId)
            {
                id = lastId + 1;
            }
            lastId = id;
            return id.ToString();
        }
    }

    public class TaskPlanner
    {
        private List<Task> tasks;
        private string xmlFilePath;

        public TaskPlanner(string filePath)
        {
            xmlFilePath = filePath;
            tasks = new List<Task>();
        }

        public void AddTask()
        {
            Console.WriteLine("Введіть дані завдання:");
            Console.Write("Назва завдання: ");
            string name = Console.ReadLine();
            Console.Write("Опис завдання: ");
            string description = Console.ReadLine();
            Console.Write("Дедлайн (у форматі yyyy-MM-dd): ");
            DateTime deadline = DateTime.Parse(Console.ReadLine());
            Console.Write("Пріоритет (0 - Low, 1 - Medium, 2 - High): ");
            int priority = int.Parse(Console.ReadLine());
            Console.Write("Статус (0 - New, 1 - InProgress, 2 - Completed): ");
            int status = int.Parse(Console.ReadLine());

            tasks.Add(new Task(
                name,
                description,
                deadline,
                (Task.Priority)priority,
                (Task.Status)status
            ));

            SaveTasks();
            Console.WriteLine("Новий запис успішно створено.");
        }

        public void DeleteTask(string id)
        {
            Task taskToRemove = tasks.Find(t => t.ID == id);
            if (taskToRemove != null)
            {
                tasks.Remove(taskToRemove);
                SaveTasks();
                Console.WriteLine("Запис успішно видалено.");
            }
        }

        public void EditTask(string id, Task newTaskData)
        {
            Task taskToEdit = tasks.Find(t => t.ID == id);
            if (taskToEdit != null)
            {
                taskToEdit.Name = newTaskData.Name;
                taskToEdit.Description = newTaskData.Description;
                taskToEdit.Deadline = newTaskData.Deadline;
                taskToEdit.priority = newTaskData.priority;
                taskToEdit.status = newTaskData.status;
                SaveTasks();
                Console.WriteLine("Запис успішно відредаговано.");
            }
        }

        private void SaveTasks()
        {
            XDocument doc = new XDocument(
                new XElement("Tasks",
                    from task in tasks
                    select new XElement("Task",
                        new XElement("ID", task.ID),
                        new XElement("Name", task.Name),
                        new XElement("Description", task.Description),
                        new XElement("Deadline", task.Deadline.ToString("yyyy-MM-dd")),
                        new XElement("Priority", task.priority),
                        new XElement("Status", task.status)
                    )
                )
            );
            doc.Save(xmlFilePath);
        }

        public void LoadTasks()
        {
            if (File.Exists(xmlFilePath))
            {
                XDocument doc = XDocument.Load(xmlFilePath);
                tasks = (from taskElement in doc.Descendants("Task")
                         select taskElement.Element("ID") != null ?
                             new Task(
                                 taskElement.Element("Name").Value,
                                 taskElement.Element("Description").Value,
                                 DateTime.Parse(taskElement.Element("Deadline").Value),
                                 (Task.Priority)Enum.Parse(typeof(Task.Priority), taskElement.Element("Priority").Value),
                                 (Task.Status)Enum.Parse(typeof(Task.Status), taskElement.Element("Status").Value)
                             ) : null
                         )
                .ToList();
            }
        }

        public void DisplayTasks()
        {
            foreach (var task in tasks)
            {
                Console.WriteLine(task);
            }
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            string xmlFilePath = "tasks.xml";
            Console.WriteLine("Файл знайдено. Зчитування даних...");

            TaskPlanner taskPlanner = new TaskPlanner(xmlFilePath);
            taskPlanner.LoadTasks();

            while (true)
            {
                Console.WriteLine("\nОберіть опцію:");
                Console.WriteLine("1. Додати завдання");
                Console.WriteLine("2. Редагувати завдання");
                Console.WriteLine("3. Видалити завдання");
                Console.WriteLine("4. Показати всі завдання");
                Console.WriteLine("5. Вийти");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        Console.WriteLine("\nДодавання нового завдання...");
                        taskPlanner.AddTask();
                        break;
                    case "2":
                        Console.WriteLine("\nРедагування завдання...");
                        Console.Write("Введіть ID завдання для редагування: ");
                        string taskIdToEdit = Console.ReadLine();
                        Task editedTask = CreateTask();
                        taskPlanner.EditTask(taskIdToEdit, editedTask);
                        break;
                    case "3":
                        Console.WriteLine("\nВидалення завдання...");
                        Console.Write("Введіть ID завдання для видалення: ");
                        string taskIdToDelete = Console.ReadLine();
                        taskPlanner.DeleteTask(taskIdToDelete);
                        break;
                    case "4":
                        Console.WriteLine("\nПоказ усіх завдань:");
                        taskPlanner.DisplayTasks();
                        break;
                    case "5":
                        Console.WriteLine("\nЗавершення програми.");
                        return;
                    default:
                        Console.WriteLine("\nНевірний вибір. Будь ласка, виберіть опцію знову.");
                        break;
                }
            }
        }

        static Task CreateTask()
        {
            Console.WriteLine("Введіть дані завдання:");
            Console.Write("Назва завдання: ");
            string name = Console.ReadLine();
            Console.Write("Опис завдання: ");
            string description = Console.ReadLine();
            Console.Write("Дедлайн (у форматі yyyy-MM-dd): ");
            DateTime deadline = DateTime.Parse(Console.ReadLine());
            Console.Write("Пріоритет (0 - Low, 1 - Medium, 2 - High): ");
            int priority = int.Parse(Console.ReadLine());
            Console.Write("Статус (0 - New, 1 - InProgress, 2 - Completed): ");
            int status = int.Parse(Console.ReadLine());
            return new Task(
                name,
                description,
                deadline,
                (Task.Priority)priority,
                (Task.Status)status
            );
        }
    }
}
