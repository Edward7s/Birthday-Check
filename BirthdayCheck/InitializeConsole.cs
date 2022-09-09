using BirthdayCheck.Utils;
using Microsoft.VisualBasic;
using Newtonsoft.Json;

namespace BirthdayCheck
{
    internal class InitializeConsole
    {
        private string _currentTime { get; set; }
        public static InitializeConsole? Instance { get; private set; }
        public string? LastText { get; set; }
        private Json.PersonInfo _person { get; set; } = new Json.PersonInfo();
        private bool _foundBday { get; set; } = false;
        private string _time { get; set; }
        public bool IsOpen { get; set; } = true;
        private int? _state { get; set; } = null;
        private int? _currentKey { get; set; } = null;
        private string _fileText { get; set; }
        private List<Json.PersonInfo> _personInfo { get; set; }
        public IntPtr ConsoleHwnd { get; set; }
        public InitializeConsole()
        {
            Instance = this;
            if (!File.Exists(Directory.GetCurrentDirectory() + "//BDays.json"))
            {
                File.Create(Directory.GetCurrentDirectory() + "//BDays.json").Close();
                SaveJson(new Json.PersonInfo() { Name = "Edward7.", Date = "09/09", Note = "Default" });
            }
            _personInfo = JsonConvert.DeserializeObject<List<Json.PersonInfo>>(File.ReadAllText(Directory.GetCurrentDirectory() + "//BDays.json"));
            Imports.AllocConsole();
            ConsoleHwnd = Imports.GetConsoleWindow();
            Imports.ShowWindow(ConsoleHwnd, 0);
            Console.Title = "Menu";
            Console.ForegroundColor = ConsoleColor.Gray;
            DefaultMenu();

            //Using Linq Cause This Is Getting Called Only One Time.
            if (_personInfo.FirstOrDefault(x => x.Date == DateAndTime.Now.ToString("dd/MM")) != null)
            {
                Imports.ShowWindow(ConsoleHwnd, 5);
                var persons = _personInfo.Where(x => x.Date == DateAndTime.Now.ToString("dd/MM")).ToArray();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Clear();
                for (int i = 0; i < persons.Length; i++)
                    Console.WriteLine($"Its {persons[i].Name} BIRTHDAY  {(persons[i].Note == null ? "" : " Note: " + persons[i].Note)}");
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine("------[Press Enter To Go Back To The Menu]------");
                Console.ReadLine();
                _currentTime = DateAndTime.Now.ToString("dd/MM");
                DefaultMenu();
                OpenedMenu();
            }
            Task.Run(() => Checker());

        }
        public void SaveJson(Json.PersonInfo person)
        {
            _fileText = File.ReadAllText(Directory.GetCurrentDirectory() + "//BDays.json");
            if (_fileText == String.Empty)
            {
                _personInfo = new List<Json.PersonInfo>();
                _personInfo.Add(new Json.PersonInfo()
                {
                    Name = person.Name,
                    Date = person.Date,
                    Note = person.Note
                });
                File.WriteAllText(Directory.GetCurrentDirectory() + "//BDays.json", JsonConvert.SerializeObject(_personInfo));
                Log("User", "Added User: " + person.Name + " On: " + person.Date);
                return;
            }
            _personInfo = JsonConvert.DeserializeObject<List<Json.PersonInfo>>(_fileText);
            _personInfo.Add(new Json.PersonInfo()
            {
                Name = person.Name,
                Date = person.Date,
                Note = person.Note
            });
            File.WriteAllText(Directory.GetCurrentDirectory() + "//BDays.json", JsonConvert.SerializeObject(_personInfo));
            Log("User", "Added User: " + person.Name + " On: " + person.Date);
        }
        private void DefaultMenu()
        {
            _state = 1;
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("------------------------------------------------");
            Console.WriteLine("1: Add New Person.");
            Console.WriteLine("2: Remove Person.");
            Console.WriteLine("3: Info.");
            Console.WriteLine("4: Hide Window.");
            Console.WriteLine("5: Check List.");
            Console.WriteLine("----------[Press The key For The Menu]----------");
        }
        private void RegisterNewPerson()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("------[Enter A Name]------");
            Console.ForegroundColor = ConsoleColor.Cyan;
            _person.Name = Console.ReadLine().Trim();
            if (_personInfo.FirstOrDefault(x => x.Name == _person.Name) != null)
            {
                Console.WriteLine("There is someone With that name");
                Console.ReadLine();
                DefaultMenu();
                return;
            }
            if (_person.Name.Length < 1)
            {
                Console.WriteLine("The Name Can Not Be Smaller Then 1C");
                Console.ReadLine();
                DefaultMenu();
                return;
            }
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine("------[Enter A Date (Day/Month(Digits Only))]------");
            _person.Date = Console.ReadLine().Trim();
            if (_person.Date.Length != 5)
            {
                Console.WriteLine("------[Something Went Wrong Try following this path > (09/09)]------");
                Console.ReadLine();
                DefaultMenu();
                return;
            }
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("------[Enter A Note Leave Empty If None]------");
            _person.Note = Console.ReadLine().Trim().Length > 1 ? Console.ReadLine().Trim() : null;
            Console.ForegroundColor = ConsoleColor.Magenta;
            SaveJson(_person);
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("------[Press Enter To Go Back To The Menu]------");
            Console.ReadLine();
            DefaultMenu();
        }

        private void InfoMenu()
        {
            _state = null;
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("------------------------------------------------");
            Console.WriteLine("A Small Little Project Made By Edward7.");
            Console.WriteLine("I am not good at remembering BirthDays So I Just Did This.");
            Console.WriteLine("Tip: Don't Close The App By The X Button Just Press 4 On The Menu.");
            Console.WriteLine("------[Press Enter To Go Back To The Menu]------");
            Console.ReadLine();
            DefaultMenu();
        }

        private void Checker()
        {
            //Waiting 30Min Before Collecting The Garbage and Checking For BirthDays.
            while (true)
            {
                _foundBday = false;
                _time = DateAndTime.Now.ToString("dd/MM");
                if (_currentTime == _time)
                {
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    Thread.Sleep(3600000);
                    continue;
                }
                /* I Chose To Use A for loop instead of linq cause I Can Freeze
                   The Thread More And Also Should Be Less Performance heavy (Even Tho A linq will be way more clean). */
                for (int i = 0; i < _personInfo.Count; i++)
                {
                    if (_personInfo[i].Date != _time) continue;
                    if (!_foundBday)
                    {
                        Imports.ShowWindow(ConsoleHwnd, 5);
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Clear();
                        _state = null;
                    }
                    _foundBday = true;
                    if (!IsOpen)
                        ToggleConsole();

                    Console.WriteLine($"Its {_personInfo[i].Name} BIRTHDAY  {(_personInfo[i].Note == null ? "" : " Note: " + _personInfo[i].Note)}");
                    Thread.Sleep(200);
                }
                if (_foundBday)
                {
                    Console.WriteLine("------[Press Enter To Go Back To The Menu]------");
                    Console.ReadLine();
                    _currentTime = _time;
                    DefaultMenu();
                }
                GC.Collect();
                GC.WaitForPendingFinalizers();
                Thread.Sleep(1800000);
            }

        }


        public void RemoveUser()
        {
            _state = null;
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Green;
            for (int i = 0; i < _personInfo.Count; i++)
                Console.WriteLine($"{_personInfo[i].Name} | {_personInfo[i].Date} {(_personInfo[i].Note == null ? "" : "| " + _personInfo[i].Note)}");

            Console.WriteLine("------[Please Type the name u want to remove]------");
            LastText = Console.ReadLine().Trim();
            if (LastText.Length < 1)
            {
                Console.WriteLine("The Name Can Not Be Smaller Then 1C");
                Console.ReadLine();
                DefaultMenu();
                return;
            }

            if (_personInfo.FirstOrDefault(x => x.Name == LastText) == null)
            {
                Console.WriteLine("------[There's No One With That Name]------");
                Console.ReadLine();
                DefaultMenu();
                return;
            }

            _personInfo = _personInfo.Where(x => x.Name != LastText).ToList();
            File.WriteAllText(Directory.GetCurrentDirectory() + "//BDays.json", JsonConvert.SerializeObject(_personInfo));
            Console.WriteLine("------[Deleted Users]------");
            Console.WriteLine("------[Press Enter To Go Back To The Menu]------");
            Console.ReadLine();
            DefaultMenu();
        }

        public void ListMenu()
        {
            _state = null;
            Console.Clear();
            var BirthdayUsers = _personInfo.Where(x => x.Date == DateAndTime.Now.ToString("dd/MM")).ToArray();
            Console.ForegroundColor = ConsoleColor.Red;
            for (int i = 0; i < BirthdayUsers.Length; i++)
                Console.WriteLine($">>>{BirthdayUsers[i].Name} | {BirthdayUsers[i].Date} {(BirthdayUsers[i].Note == null ? "" : "| " + BirthdayUsers[i].Note)}<<<");
            Console.ForegroundColor = ConsoleColor.Green;
            for (int i = 0; i < _personInfo.Count; i++)
                Console.WriteLine($"{_personInfo[i].Name} | {_personInfo[i].Date} {(_personInfo[i].Note == null ? "" : "| " + _personInfo[i].Note)}");
            Console.WriteLine("------[Press Enter To Go Back To The Menu]------");
            Console.ReadLine();
            DefaultMenu();
        }

        public void ToggleConsole()
        {
            //Toggles The Console On And Off.
            IsOpen = !IsOpen;
            Imports.ShowWindow(ConsoleHwnd, IsOpen ? 5 : 0);
            if (!IsOpen) return;
            DefaultMenu();
            OpenedMenu();
        }

        private void Log(object type, object message, ConsoleColor color = ConsoleColor.White)
        {
            Console.ForegroundColor = color;
            Console.Write($"{type} ");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("=> " + message);
        }

        private void OpenedMenu()
        {
            while (IsOpen)
            {
                LastText = string.Empty;
                if (_state != 1)
                {
                    Thread.Sleep(100);
                    continue;
                }
                LastText = Console.ReadKey().KeyChar.ToString();
                try
                {
                    _currentKey = int.Parse(LastText);
                    if (_currentKey < 1 || _currentKey > 3)
                        Log("ERROR", LastText + " Is not a matching any of the strings", ConsoleColor.Red);
                }
                catch
                {
                    Log("ERROR", LastText + " Can Not Be Parsed As A Int", ConsoleColor.Red);
                }
                switch (_currentKey)
                {
                    case 1:
                        RegisterNewPerson();
                        break;
                    case 2:
                        RemoveUser();
                        break;
                    case 3:
                        InfoMenu();
                        break;
                    case 4:
                        ToggleConsole();
                        break;
                    case 5:
                        ListMenu();
                        break;
                }
                _currentKey = null;

            }
        }
    }
}