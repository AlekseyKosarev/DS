using DS.Examples.Data;
using TMPro;
using UnityEngine;

public class UIExampleData : MonoBehaviour
{
    public TextMeshProUGUI text;
    public TextMeshProUGUI gradeText;

    public void Init(DataExampleDS dataExampleDS)
    {
        text.text = dataExampleDS.name;

        var strGrade = "\u2666";
        var finalGrade = "";
        for (var i = 0; i < dataExampleDS.grade; i++) finalGrade += strGrade;
        gradeText.text = finalGrade;
    }
}