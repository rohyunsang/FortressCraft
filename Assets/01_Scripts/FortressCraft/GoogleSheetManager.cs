using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public static class GoogleSheetManager
{
    public static List<UnitData> unitDatas = new List<UnitData>();

    // ��ũ �� edit ~ �κ��� ���� export?format=tsv �߰��ϱ�
    // origin url https://docs.google.com/spreadsheets/d/1Z_awcApNOODEqu---j7VFGDjjuyU5cl7IMinMXKHPII/edit?gid=0#gid=0
    private const string unitDataURL = "https://docs.google.com/spreadsheets/d/1Z_awcApNOODEqu---j7VFGDjjuyU5cl7IMinMXKHPII/export?format=tsv";
    public static IEnumerator Loader()
    {
        UnityWebRequest www = UnityWebRequest.Get(unitDataURL);
        yield return www.SendWebRequest();

        string data = www.downloadHandler.text;
        Debug.Log(data);

        ParseUnitData(data);
    }


    private static void ParseUnitData(string data)
    {
        string[] lines = data.Split('\n');
        for (int i = 1; i < lines.Length; i++) // ù ��° ���� ���� ������ ����
        {
            string[] fields = lines[i].Split('\t');
            if (fields.Length < 14) // �ʿ��� �ʵ� �� �̴� �� �α� ��� �� ó�� �ߴ�
            {
                Debug.LogError($"Line {i} has insufficient fields: {fields.Length} fields found.");
                continue;
            }

            try
            {
                UnitData unitData = new UnitData()
                {
                    Index = ParseInt(fields[0]),
                    Name = fields[1],
                    Grade = ParseInt(fields[2]),
                    Class = ParseInt(fields[3]),
                    HP = ParseFloat(fields[4]),
                    MP = ParseFloat(fields[5]),
                    AttackPower = ParseFloat(fields[6]),
                    Defense = ParseFloat(fields[7]),
                    AttackSpeed = ParseFloat(fields[8]),
                    MoveSpeed = ParseFloat(fields[9]),
                    AttackRange = ParseFloat(fields[10]),
                    Cost = ParseInt(fields[11]),
                    KRName = fields[12],
                    DesText = fields[13],
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
}

[System.Serializable]
public class UnitData
{
    public int Index;
    public string Name;
    public int Grade;
    public int Class;
    public int Cost;
    public float HP;
    public float MP;
    public float AttackPower;
    public float Defense;
    public float AttackSpeed;
    public float MoveSpeed;
    public float AttackRange;
    public string KRName;
    public string DesText;
}

[System.Serializable]
public class CommanderData
{

}