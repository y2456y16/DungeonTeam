using System.Collections.Concurrent;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SimpleRPG
{
    class GameCharacter //게임 캐릭터 설정
    {
        private Action<float> healthChangedCallback;
        public int level;
        public float hp;
        public float maxHP; //최대 체력값
        public string name;
        public string job;
        public float attack;
        public float defense;
        public int gold;

        //처음 캐릭터 생성시 스탯 설정
        public void SetStatus(string name, string job, float hp, float attack, float defense, int gold)
        {
            this.level = 1;
            this.name = name;
            this.job = job;
            this.hp = hp;
            this.maxHP = hp;
            this.attack = attack;
            this.defense = defense;
            this.gold = gold;
        }


        // 현재 표현된 텍스트 길이
        public static int GetPrintableLength(string str)
        {
            int length = 0;
            foreach (char c in str)
            {
                if (char.GetUnicodeCategory(c) == System.Globalization.UnicodeCategory.OtherLetter)
                {
                    length += 2;
                }
                else
                {
                    length += 1;
                }
            }
            return length;
        }

        //총 길이를 매개변수로 입력 받아서, 텍스트가 끝나면 우측에 빈칸을 배치
        public static string PadRightForMixedText(string str, int totalLength)
        {
            int currentLength = GetPrintableLength(str);
            int padding = totalLength - currentLength;
            return str.PadRight(str.Length + padding);
        }

        //상태 표시
        public void statUI()
        {
            Console.WriteLine("     상태창     ");
            Console.Write(PadRightForMixedText($"이름 ", 7)); //일정 텍스트 길이 적용
            Console.WriteLine($": {this.name}");
            Console.WriteLine($"Lv. {this.level}");
            Console.Write(PadRightForMixedText($"직업 ", 7));
            Console.WriteLine($": {this.job}");
            Console.Write(PadRightForMixedText($"HP ", 7));
            Console.WriteLine($": {this.hp}");
            Console.Write(PadRightForMixedText($"공격력 ", 7));
            Console.WriteLine($": {this.attack}");
            Console.Write(PadRightForMixedText($"방어력 ", 7));
            Console.WriteLine($": {this.defense}");
            Console.WriteLine();
            Console.WriteLine($"소지한 금액 : {this.gold}G");
            Console.WriteLine();
            Console.WriteLine("0. 나가기");
            string select = Console.ReadLine();
            int selection;
            if(int.TryParse(select, out selection))
            {
                if(selection == 0)
                    Console.Clear();
                else
                {
                    Console.Clear();
                    statUI();
                }
            }
            else
            {
                Console.Clear();
                statUI();
            }
        }

        //hp 변화
        public float HealthCall
        {
            get { return hp; }

            set
            {
                hp = value;
                healthChangedCallback?.Invoke(hp);
            }
        }

        public void SetHealthChangedCallback(Action<float> callback)
        {
            healthChangedCallback = callback;
        }
    }

    /* //인벤토리용 클래스
    class InvenList
    {
        private string name;
        private string type;
        private string info;
        private bool equip;
        private bool purchased;
        private float attack;
        private float defense;
        private float hp;

        public InvenList(string name, string type, string info, float attack, float defense, float hp, bool purchased)
        {
            this.name = name;
            this.type = type;
            this.info = info;
            this.equip = false;
            this.purchased = purchased;
            this.attack = attack;
            this.defense = defense;
            this.hp = hp;
        }
        
    }
    */

    /* 
    //아이템 리스트 클래스 (generic 문법 사용)
    class ItemList<T1, T2>
    {
        public T1 itemName;
        public T1 itemType;
        public T1 itemInfo;
        public T2 itemAttack;
        public T2 itemDefense;
        public T2 itemHealth;
        public bool Purchased;

        public ItemList(T1 name, T1 type, T1 info, T2 attack, T2 defense, T2 health, bool purchased)
        {
            itemName = name;
            itemType = type;
            itemInfo = info;
            itemAttack = attack;
            itemDefense = defense;
            itemHealth = health;
            Purchased = purchased;
        }

    }
    */


    // 던전 클래스
    class Dungeon
    {
        //몬스터 정보 데이터베이스
        public List<Monster> monsterList = new List<Monster>
        {
            //name, level, hp, attack
            new Monster("미니언", 1, 20, 5),
            new Monster("중급 미니언", 5, 30, 10),
            new Monster("대포 미니언", 10, 40, 15),
            new Monster("바론", 15, 100, 20)
        };

        //전투 입장시 랜덤하게 나오는 몬스터 리스트
        public List<Monster> randMonster = new List<Monster>();




        //던전 입장 시 난이도 선택 화면
        public int StageSelect()
        {
            Console.WriteLine("던전에 입장하셨습니다\n");
            Console.WriteLine("난이도 선택\n");
            Console.WriteLine("1. 이지 모드(레벨 1~5)");
            Console.WriteLine("2. 노말 모드(레벨 6~10)");
            Console.WriteLine("3. 하드 모드(레벨 11~15)");
            Console.WriteLine();
            Console.WriteLine("난이도를 입력하세요");
            string stage;
            int stageSelect;
        DunStart:
            stage = Console.ReadLine();
            if(int.TryParse(stage, out stageSelect))
            {
                if(stageSelect ==1 || stageSelect ==2 || stageSelect ==3)
                {
                    Console.Clear();
                    return MonsterRandom(stageSelect);
                }
                else
                {
                    Console.WriteLine("올바른 번호를 입력해주세요");
                    goto DunStart;
                }
            }
            else
            {
                Console.WriteLine("올바른 값을 입력해주세요");
                goto DunStart;
            }

        }

        //레벨에 따라 몬스터 정보를 나열
        public void LevelSort()
        {
            QuickSortMonster(0, monsterList.Count());
        }
        public void QuickSortMonster(int left, int right)
        {
            if(left>= right)
            {
                return;
            }

            int pivot = Partitioner(left, right);

            QuickSortMonster(left, pivot - 1);
            QuickSortMonster(pivot + 1, right);
        }
        private int Partitioner(int left, int right)
        {
            int pivot = monsterList[right].monsterLevel;
            int i = left - 1;

            for(int j=left; j<right; j++)
            {
                if (monsterList[j].monsterLevel < pivot)
                {
                    i++;
                    Swap(i, j);
                }
            }

            Swap(i + 1, right);
            return i + 1;

        }
        private void Swap(int i, int j)
        {
            string Name = monsterList[i].monsterName;
            int Level = monsterList[i].monsterLevel;
            float Hp = monsterList[i].monsterHp;
            float Attack = monsterList[i].monsterAttack; 

            monsterList[i].monsterName = monsterList[j].monsterName;
            monsterList[i].monsterLevel = monsterList[j].monsterLevel;
            monsterList[i].monsterHp = monsterList[j].monsterHp;
            monsterList[i].monsterAttack = monsterList[j].monsterAttack;

            monsterList[j].monsterName = Name;
            monsterList[j].monsterLevel=Level;
            monsterList[j].monsterHp = Hp;
            monsterList[j].monsterAttack = Attack;
        }

        // 랜덤 몬스터 생성
        public int MonsterRandom(int stage)
        {
            Random random = new Random();
            int monsterNumb = random.Next(1, 5);
            int[] monsterID = new int[monsterList.Count()];
            int count = 0;

            for(int i=0; i<monsterList.Count(); i++)
            {
                if (monsterList[i].monsterLevel > (stage-1) * 5 && monsterList[i].monsterLevel <= (stage) * 5)
                {
                    monsterID[count] = i;
                    count++;
                }
       
            }

            for(int j=0; j<monsterNumb; j++)
            {
                int ran = random.Next(0, count);
                randMonster.Add(monsterList[ran]);
            }

            return monsterNumb;
        }

        //던전 클리어 혹은 실패 시 랜덤 몬스터 데이터 삭제용
        /*
        public void Delete()
        {
            foreach(var everything in randMonster)
            {
                randMonster.Remove(everything);
            }
        }
        */
        
    }


    // 몬스터 정보 클래스
    class Monster
    {
        public string monsterName;
        public int monsterLevel;
        public float monsterHp;
        public float monsterAttack;

        public Monster(string name, int level, float hp, float attack)
        {
            monsterName = name;
            monsterLevel = level;
            monsterHp = hp;
            monsterAttack = attack;
        }
    }

   
    internal class Program
    {

        //제너릭 문법
        //static List<ItemList<string,float>> items = new List<ItemList<string,float>>();
        //static List<InvenList> Inven = new List<InvenList>();

        //캐릭터 클래스 생성(스탯 설정 포함)
        static GameCharacter gameCharacter = new GameCharacter();
        //던전 클래스 생성
        static Dungeon playDun = new Dungeon();

        //초기화면
        static void StartUI()
        {
            Console.WriteLine("스파르타 던전에 오신 여러분 환영합니다\n");
            Console.WriteLine(">>>>    <<<<");
            Console.WriteLine("            ");
            Console.WriteLine("    ^^^^    ");
            Console.WriteLine();
            Console.WriteLine(" Press Enter");
            
            Console.ReadLine();
            Console.Clear();
            SetCharacter(); //캐릭터 스탯 초기 설정
        }

        //메인화면
        static void MainUI()
        {
            string select1;
            int selection1;

            Console.WriteLine("이제 전투를 시작할 수 있습니다.\n");
            Console.WriteLine("1. 상태보기");
            Console.WriteLine("2. 전투 시작");
            Console.WriteLine("\n원하시는 행동을 입력해주세요.");
            Console.Write(">>");

            
        selectAgain1:

            //예외 처리
            try
            {
                select1 = Console.ReadLine();
                selection1 = int.Parse(select1);

            }catch
            {
                Console.WriteLine("잘못 입력하셨습니다. 다시 입력해주세요");
                goto selectAgain1;
            }

            if(selection1 <=0 || selection1 >2)
            {
                Console.WriteLine("올바른 번호를 입력해주세요");
                goto selectAgain1;
            }
            else
            {
                switch(selection1)
                {
                    case 1://상태보기 선택
                        Console.Clear();
                        gameCharacter.statUI();
                        MainUI();
                        break;
                    case 2://전투 시작 선택
                        Console.Clear();
                        int randNumber = playDun.StageSelect();
                        FightUI(randNumber);
                        //playDun.Delete();
                        MainUI();
                        break;
                }
            }
        }

        //전투 시작 선택 후 나오는 화면
        static void FightUI(int number)
        {
            int turn = 0; //화면 진행에 따른 페이지 표시
            string selection; //선택지 입력값
            int select=0; //선택지 int 변환 값
            double minValue, maxValue; //데미지 계산 시 필요한 최소, 최대 수치 변수
            float damage;
            float currentHP = gameCharacter.hp; //입장 초기, 캐릭터 체력
            Random rand1 = new Random();

            //배틀 첫화면
        BattleStart:
            Console.WriteLine("Battle!\n");
            for (int i = 0; i < number; i++) //랜덤 몬스터 표시
            {
                if(turn == 1)
                {
                    Console.Write($"{i+1}. ");
                }
                
                Console.Write(PadRightForMixedText($"Lv.{playDun.randMonster[i].monsterLevel} {playDun.randMonster[i].monsterName}", 15));
                
                if(playDun.randMonster[i].monsterHp>0)
                {
                    Console.WriteLine($"HP {playDun.randMonster[i].monsterHp}");
                }
                else
                {
                    Console.WriteLine($"Dead");
                }
                
            }

            //캐릭터 정보 표시
            Console.WriteLine("\n[내정보]");
            Console.WriteLine($"Lv.{gameCharacter.level} {gameCharacter.name} ({gameCharacter.job})");
            Console.WriteLine($"HP {gameCharacter.hp}/{gameCharacter.maxHP}");
            Console.WriteLine("");
            if(turn ==0)
            {
                Console.WriteLine("1. 공격");
                Console.WriteLine("0. 후퇴");
                Console.WriteLine("");
                Console.WriteLine("원하시는 행동을 입력해주세요");

            Battle:
                selection = Console.ReadLine();
                if (int.TryParse(selection, out select))
                {
                    if (select == 1)
                    {
                        turn = 1;
                        Console.Clear();
                        goto BattleStart;
                    }
                    else if(select == 0)
                    {
                        goto End;
                    }
                    else
                    {
                        Console.WriteLine("잘못된 숫자를 입력하셨습니다");
                        goto Battle;
                    }
                }
                else
                {
                    Console.WriteLine("잘못 입력하셨습니다");
                    goto Battle;
                }
            }
            else if(turn ==1)
            {
                Console.WriteLine("0. 취소");

                Console.WriteLine("");
                Console.WriteLine("대상을 선택해주세요");



            Battle2:
                selection = Console.ReadLine();
                if (int.TryParse(selection, out select))
                {
                    if (select == 0)
                    {
                        turn = 0;
                        Console.Clear();
                        goto BattleStart;
                    }
                    else if(select>0 && select<=number )
                    {
                        turn = 2;
                        goto DamagePage;
                    }
                    else
                    {
                        Console.WriteLine("잘못된 숫자를 입력하셨습니다");
                        goto Battle2;
                    }
                }
                else
                {
                    Console.WriteLine("잘못 입력하셨습니다");
                    goto Battle2;
                }
            }

        DamagePage://데미지 표시 및 적용 페이지
            Console.Clear();
            Console.WriteLine("Battle!\n");
            Console.WriteLine($"{gameCharacter.name}의 공격!");
            
            //데미지 계산
            minValue = (double)gameCharacter.attack*9/10;
            maxValue = (double)gameCharacter.attack * 11 / 10;
            damage = (float)(minValue + (maxValue - minValue) * rand1.NextDouble());

            Console.WriteLine($"Lv.{playDun.randMonster[select-1].monsterLevel} {playDun.randMonster[select - 1].monsterName}을(를) 맞췄습니다. [데미지 : {Math.Ceiling(damage)}]\n");
            Console.WriteLine($"");
            Console.WriteLine($"Lv.{playDun.randMonster[select - 1].monsterLevel} {playDun.randMonster[select - 1].monsterName}");
            Console.WriteLine($"HP {playDun.randMonster[select - 1].monsterHp} -> {playDun.randMonster[select - 1].monsterHp- (float)Math.Ceiling(damage)}");
            playDun.randMonster[select - 1].monsterHp = playDun.randMonster[select - 1].monsterHp - (float)Math.Ceiling(damage);
            Console.WriteLine("");
            Console.WriteLine("0. 다음");

            //몬스터의 체력이 0이하 조건
            if(playDun.randMonster[select - 1].monsterHp <=0)
            {
                //해당 몬스터 체력 0으로 설정
                playDun.randMonster[select - 1].monsterHp = 0;
                
                //모든 랜덤 몬스터 체력이 0이면 결과화면으로 이동
                bool end = checkDead(number);
                if(end == true)
                {
                    goto Result;
                }
            }

        DamagePageSelect:
            Console.Write("\n>>");
            selection = Console.ReadLine();
            if(int.TryParse(selection, out select))
            {
                if (select == 0)
                {
                    Console.Clear();
                    goto EnemyPhase;
                }
                else
                {
                    Console.WriteLine("잘못된 숫자를 입력하셨습니다");
                    goto DamagePageSelect;
                }
            }
            else
            {
                Console.WriteLine("잘못 입력하셨습니다");
                goto DamagePageSelect;
            }

        EnemyPhase://적의 공격 페이지
            Console.Clear();
            Console.WriteLine("Battle!\n");
            for(int i=0; i<number; i++)
            {
                if(playDun.randMonster[i].monsterHp <=0) //몬스터 체력이 0 이하면 해당 몬스터는 공격 못하고 스킵
                {
                    continue;
                }
                damage = playDun.randMonster[i].monsterAttack-gameCharacter.defense;
                if (damage < 0)
                    damage = 0;

                Console.WriteLine($"Lv.{playDun.randMonster[i].monsterLevel} {playDun.randMonster[i].monsterName}의 공격!");
                Console.WriteLine($"{gameCharacter.name}을(를) 맞췄습니다. [데미지 : {damage}]\n");

                Console.WriteLine($"Lv.{gameCharacter.level} {gameCharacter.name}");
                Console.WriteLine($"HP {gameCharacter.hp} -> {gameCharacter.hp - damage}\n");

                //데미지에 따른 체력 감소 적용
                gameCharacter.HealthCall = gameCharacter.hp - damage;

                //캐릭터의 체력이 0 이하로 내려가면 결과화면으로 이동
                if(gameCharacter.hp <=0)
                {
                    goto Result;
                }
            }

            Console.WriteLine($"0. 다음\n");
            Console.Write(">>");
            
    
        EnemyPhaseSelect:
            selection = Console.ReadLine();
            if(int.TryParse(selection, out select))
            {
                if (select == 0)
                {
                    Console.Clear();
                    turn = 0;
                    goto BattleStart;
                }
                else
                {
                    Console.WriteLine("잘못된 숫자를 입력하셨습니다");
                    goto EnemyPhaseSelect;
                }
            }
            else
            {
                Console.WriteLine("잘못 입력하셨습니다");
                goto EnemyPhaseSelect;
            }


        //결과 화면
        Result:
            Console.Clear();
            turn = 0;
            Console.WriteLine("Battle! - Result\n");

            if (gameCharacter.hp <= 0)
            {
                Console.WriteLine("You Lose\n");
                Console.WriteLine($"Lv.{gameCharacter.level} {gameCharacter.name}");
                Console.WriteLine($"HP {gameCharacter.maxHP} -> 0\n");
                
            }
            else
            {
                Console.WriteLine("Victory\n");
                Console.WriteLine($"던전에서 몬스터 {number}마리를 잡았습니다\n");
                Console.WriteLine($"{gameCharacter.name}");
                Console.WriteLine($"Lv.{gameCharacter.level} -> {gameCharacter.level+1}");  
                Console.WriteLine($"HP {gameCharacter.maxHP} -> {gameCharacter.hp}\n");
                gameCharacter.level++;
            }

            Console.WriteLine("0. 다음\n");          

        ResultSelect:
            Console.Write(">>");
            selection = Console.ReadLine();
            if (int.TryParse(selection, out select))
            {
                if (select == 0)
                {
                    goto End;
                }
                else
                {
                    Console.WriteLine("잘못된 숫자를 입력하셨습니다");
                    goto ResultSelect;
                }
            }
            else
            {
                Console.WriteLine("잘못 입력하셨습니다");
                goto ResultSelect;
            }

        End:
            turn = 0;
            Console.Clear();
        }

        //랜덤 몬스터들이 모두 죽었는지 확인하는 함수
        static bool checkDead(int randomMonsterCount)
        {
            int countDeath = 0;
            for(int i=0; i< randomMonsterCount; i++)
            {
                if (playDun.randMonster[i].monsterHp <=0)
                {
                    countDeath++;
                }
            }

            if (countDeath == randomMonsterCount)
                return true;
            else
                return false;
        }

        //캐릭터 이름, 직업 설정
        static void SetCharacter()
        {
            Console.WriteLine("사용하실 캐릭터 이름을 작성하세요");
            string name = Console.ReadLine();
            Console.Clear();

            Console.WriteLine("사용하실 캐릭터의 직업을 선택하세요");
            Console.WriteLine("1. 전사\n2. 도적\n3. 개발자");
            string Job;
            int JobSelect;

        selectAgainJob:
            //예외 처리
            try
            {
                Job = Console.ReadLine();
                JobSelect = int.Parse(Job);

            }
            catch
            {
                Console.WriteLine("잘못 입력하셨습니다. 다시 입력해주세요");
                goto selectAgainJob;
            }

            if (JobSelect <= 0 || JobSelect > 3)
            {
                Console.WriteLine("올바른 번호를 입력해주세요");
                goto selectAgainJob;
            }
            else
            {
                string JobName;
                switch(JobSelect)
                {
                    case 1:
                        JobName = "전사";
                        gameCharacter.SetStatus(name,JobName,120,10,5,1500);
                        break;

                    case 2:
                        JobName = "도적";
                        gameCharacter.SetStatus(name, JobName, 80, 15, 2, 1500);
                        break;
                    case 3:
                        JobName = "개발자";
                        gameCharacter.SetStatus(name, JobName, 1000, 1000, 0, 1500);
                        break;
                }
            }
            Console.Clear();
            Console.WriteLine("캐릭터가 생성되었습니다\n");
            gameCharacter.statUI(); //게임 캐릭터 상태 표시

        }

        public static int GetPrintableLength(string str)
        {
            int length = 0;
            foreach (char c in str)
            {
                if (char.GetUnicodeCategory(c) == System.Globalization.UnicodeCategory.OtherLetter)
                {
                    length += 2;
                }
                else
                {
                    length += 1;
                }
            }
            return length;
        }
        public static string PadRightForMixedText(string str, int totalLength)
        {
            int currentLength = GetPrintableLength(str);
            int padding = totalLength - currentLength;
            return str.PadRight(str.Length + padding);
        }

        static void Main(string[] args)
        {
            StartUI(); // 시작화면 실행
            MainUI(); //메인화면 실행
        }
    }
}
