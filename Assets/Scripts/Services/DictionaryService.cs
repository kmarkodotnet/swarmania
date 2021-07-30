using System.Collections.Generic;

public class DictionaryService
{
    List<BugTypeEnum> level1Bugs = new List<BugTypeEnum>()
        {
            BugTypeEnum.AntSlave,BugTypeEnum.Ladybug, BugTypeEnum.Termite1
        };
    List<BugTypeEnum> level2Bugs = new List<BugTypeEnum>()
        {BugTypeEnum.AntWarrior,BugTypeEnum.Scarab
        };
    List<BugTypeEnum> level3Bugs = new List<BugTypeEnum>()
        {
            BugTypeEnum.AntFlyer,BugTypeEnum.StagBeetle
        };
    public int GetBugLevel(BugTypeEnum bugType)
    {
        if (level1Bugs.Contains(bugType))
        {
            return 1;
        }else if (level2Bugs.Contains(bugType))
        {
            return 2;
        }
        else if (level3Bugs.Contains(bugType))
        {
            return 3;
        }
        else
        {
            throw new System.Exception($"No level for this bugtype: {bugType}");
        }
    }
}

