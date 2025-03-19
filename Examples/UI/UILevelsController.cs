using System.Collections.Generic;
using DS.Examples;
using DS.Examples.Data;
using DS.Utils;
using UnityEngine;

public class UILevelsController : MonoBehaviour
{
    public GameManager gameManager;
    public GameObject Content;
    public GameObject LevelButtonPrefab;

    private readonly List<GameObject> _levelButtons = new();

    private void CreateLevelButton(LevelData levelData)
    {
        var button = Instantiate(LevelButtonPrefab, Content.transform);
        button.GetComponent<UILevelButton>().Init(levelData);
        _levelButtons.Add(button);
    }

    public void ClickShowLevels() //call from UI btn
    {
        ShowLevels();
    }

    public async void ShowLevels(string level = null) //call from UI btn
    {
        _levelButtons.ForEach(b => Destroy(b));
        _levelButtons.Clear();
        var result = await gameManager.Ds.LoadAllAsync<LevelData>(KeyNamingRules.KeyFor<LevelData>(level));

        if (result.IsSuccess)
        {
            foreach (var levelData in result.Data) CreateLevelButton(levelData);
        }
        else
        {
            Debug.Log(result.ErrorMessage);
            var levelData = new LevelData();
            levelData.id = 0;
            levelData.name = "Level example";
            levelData.grade = 1;
            gameManager.Ds.SaveAsync(KeyNamingRules.KeyFor<LevelData>("example"), levelData);
        }
    }
}