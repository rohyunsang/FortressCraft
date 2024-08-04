using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections.ObjectModel;

namespace Agit.FortressCraft {

    public static class GoogleSheetManager
    {
        public static List<CommanderData> commanderDatas = new List<CommanderData>();
        public static List<UnitData> unitDatas = new List<UnitData>();

        // ???? ?? edit ~ ?????? ???? export?format=tsv ????????

        private static readonly List<string> _urls = new List<string>
        {
            "https://docs.google.com/spreadsheets/d/1Z_awcApNOODEqu---j7VFGDjjuyU5cl7IMinMXKHPII/export?format=tsv&gid=0",
            "https://docs.google.com/spreadsheets/d/1Z_awcApNOODEqu---j7VFGDjjuyU5cl7IMinMXKHPII/export?format=tsv&gid=1728502320"
        };

        public static readonly ReadOnlyCollection<string> Urls = new ReadOnlyCollection<string>(_urls);

        public static IEnumerator Loader()
        {
            List<string> data = new List<string>();
            foreach (var url in Urls)
            {
                UnityWebRequest www = UnityWebRequest.Get(url);
                yield return www.SendWebRequest();

                data.Add(www.downloadHandler.text);
                
            }

            if (data[0] != null) 
                ParseCommanderData(data[0]);
            if (data[1] != null)
                ParseUnitData(data[1]);
            foreach(string d in data)
            {
                Debug.Log(d);
            }
        }


        private static void ParseCommanderData(string data)
        {
            string[] lines = data.Split('\n');
            for (int i = 1; i < lines.Length; i++) // ?? ???? ???? ???? ?????? ????
            {
                string[] fields = lines[i].Split('\t');
                if (fields.Length < 5) // ?????? ???? ?? ???? ?? ???? ???? ?? ???? ????
                {
                    Debug.LogError($"Line {i} has insufficient fields: {fields.Length} fields found.");
                    continue;
                }
                try
                {
                    CommanderData commanderData = new CommanderData()
                    {
                        CommanderType = fields[0],
                        Level = ParseInt(fields[1]),
                        NeedExp = ParseInt(fields[2]),
                        HP = ParseFloat(fields[3]),
                        MP = ParseFloat(fields[4]),
                        Attack = ParseFloat(fields[5]),
                        AttackSpeed = ParseFloat(fields[6]),
                        AttackDelay = ParseFloat(fields[7]),
                        Defense = ParseFloat(fields[8]),
                        MoveSpeed = ParseFloat(fields[9]),
                        HealPerSecond = ParseFloat(fields[10]),
                    };
                    commanderDatas.Add(commanderData);
                }
                catch (FormatException e)
                {
                    Debug.LogError($"FormatException on line {i}: {e.Message}");
                }
                catch (OverflowException e)
                {
                    Debug.LogError($"OverflowException on line {i}: {e.Message}");
                }
            }
        }
        private static void ParseUnitData(string data)
        {
            string[] lines = data.Split('\n');
            for (int i = 1; i < lines.Length; i++) // ?? ???? ???? ???? ?????? ????
            {
                string[] fields = lines[i].Split('\t');
                if (fields.Length < 11) // ?????? ???? ?? ???? ?? ???? ???? ?? ???? ????
                {
                    Debug.LogError($"Line {i} has insufficient fields: {fields.Length} fields found.");
                    continue;
                }

                try
                {
                    UnitData unitData = new UnitData()
                    {
                        Level = ParseInt(fields[0]),
                        UpgradeCost = ParseInt(fields[1]),
                        HP = ParseFloat(fields[2]),
                        MP = ParseFloat(fields[3]),
                        Attack = ParseFloat(fields[4]),
                        AttackDelay = ParseFloat(fields[5]),
                        Defense = ParseFloat(fields[6]),
                        MoveSpeed = ParseFloat(fields[7]),
                        SpawnDelay = ParseFloat(fields[8]),
                        CostReward = ParseInt(fields[9]),
                        ExpReward = ParseInt(fields[10]),
                    };
                    unitDatas.Add(unitData);
                }
                catch (FormatException e)
                {
                    Debug.LogError($"FormatException on line {i}: {e.Message}");
                }
                catch (OverflowException e)
                {
                    Debug.LogError($"OverflowException on line {i}: {e.Message}");
                }
            }
        }
        private static int ParseInt(string input)
        {
                                    if (int.TryParse(input, out int result))
            {
                return result;
            }
            throw new FormatException($"Unable to parse '{input}' as integer.");
        }

        private static float ParseFloat(string input)
        {
            if (float.TryParse(input, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out float result))
            {
                return result;
            }
            throw new FormatException($"Unable to parse '{input}' as float.");
        }

        public static CommanderData GetCommanderData(int level, JobType jobType)
        {
            int offset = 0;

            switch (jobType)
            {
                case JobType.Warrior:
                    offset = -1;
                    break;
                case JobType.Magician:
                    offset = 14;
                    break;
                case JobType.Archer:
                    offset = 29;
                    break;
            }

            return GoogleSheetManager.commanderDatas[level + offset];
        }
    }

    [System.Serializable]
    public class CommanderData
    {
        public string CommanderType;
        public int Level;
        public int NeedExp;
        public float HP;
        public float MP;
        public float Attack;
        public float AttackSpeed;
        public float AttackDelay;
        public float Defense;
        public float MoveSpeed;
        public float HealPerSecond;
    }

    [System.Serializable]
    public class UnitData
    {
        public int Level;
        public int UpgradeCost;
        public float HP;
        public float MP;
        public float Attack;
        public float AttackDelay;
        public float Defense;
        public float MoveSpeed;
        public float SpawnDelay;
        public int CostReward;
        public int ExpReward;
    }
}

