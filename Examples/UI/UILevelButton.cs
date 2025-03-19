using DS.Examples.Data;
using TMPro;
using UnityEngine;

public class UILevelButton : MonoBehaviour
{
    public TextMeshProUGUI text;
    public TextMeshProUGUI gradeText;

    public void Init(LevelData levelData)
    {
        text.text = levelData.name;

        var strGrade = "\u2666";
        var finalGrade = "";
        for (var i = 0; i < levelData.grade; i++) finalGrade += strGrade;
        gradeText.text = finalGrade;
    }
}