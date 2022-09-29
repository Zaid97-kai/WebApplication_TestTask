using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebApplication_TestTask.Models;
using System.Web;

namespace WebApplication_TestTask.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        public static int currentTime = 0;
        public static int setsFirstCount = 0;
        public static int setsSecondCount = 0;
        public static string tempData = "";
        public List<Queue<ServerTask>> serverTasksQueue;
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
            serverTasksQueue = new List<Queue<ServerTask>>();
        }

        public IActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// Post-метод (Обработчик нажатия на кнопку Вычислить)
        /// </summary>
        /// <param name="InputData">Входные данные</param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Index(string InputData)
        {
            if (InputData == null)
            {
                ViewBag.MessageBox = "Поле входных данных не заполнено.";
                return View();
            }

            bool flag = false;
            bool secondFlag = false;
            InputData = InputData.Replace("\r", "");
            string[] sets = InputData.Split(new char[] { '\n', '\n' });

            serverTasksQueue.Clear();
            secondFlag = ProcessingInputParameters(flag, sets);

            if (secondFlag == false)
            {
                return View();
            }

            TaskSolution();

            ViewBag.Out = tempData.Split("\n");

            return View();
        }
        /// <summary>
        /// Метод, обрабатывающий входную строку
        /// </summary>
        /// <param name="flag">Вспомогательная переменная</param>
        /// <param name="sets">Набор данных, подвергшийся разбиению</param>
        /// <returns></returns>
        private bool ProcessingInputParameters(bool flag, string[] sets)
        {
            try
            {
                for (int i = 0; i < sets.Length; i++)
                {
                    if (!flag)
                    {
                        setsFirstCount = Convert.ToInt32(sets[0]);
                        flag = true;
                        continue;
                    }
                    else if (sets[i] == " ")
                    {
                        continue;
                    }
                    else if (flag && !sets[i].Contains(" "))
                    {
                        if (sets[i] == "")
                        {
                            continue;
                        }
                        else
                        {
                            setsSecondCount = Convert.ToInt32(sets[i]);
                            serverTasksQueue.Add(new Queue<ServerTask>());
                            i++;
                        }
                    }

                    for (int j = 0; j < setsSecondCount; j++)
                    {
                        Queue<ServerTask> serverTasks = new Queue<ServerTask>();
                        if (sets[i + j].Contains(" "))
                        {
                            string[] tempSets = sets[i + j].Split(' ');

                            ServerTask serverTask = new ServerTask()
                            {
                                first = Convert.ToInt32(tempSets[0]),
                                second = Convert.ToInt32(tempSets[1])
                            };

                            serverTasksQueue[serverTasksQueue.Count - 1].Enqueue(serverTask);
                        }
                        if (j == setsSecondCount - 1)
                        {
                            i += j;
                        }
                    }
                }

                if (setsFirstCount != serverTasksQueue.Count())
                {
                    ViewBag.MessageBox = "Количество наборов входных данных в тесте не совпадает с введенным числом q.";
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                ViewBag.MessageBox = ex.Message + "\n" + "Проверьте корректность формата введенных данных.";
                return false;
            }
        }

        /// <summary>
        /// Решение задачи
        /// </summary>
        private void TaskSolution()
        {
            tempData = string.Empty;
            foreach (Queue<ServerTask> serverTaskQueue in serverTasksQueue)
            {
                foreach (ServerTask serverTask in serverTaskQueue)
                {
                    while (currentTime < serverTask.first)
                    {
                        currentTime++;
                    }
                    currentTime += serverTask.second;
                    tempData += Convert.ToString(currentTime) + " ";
                }
                currentTime = 0;
                tempData += "\n";
            }
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}