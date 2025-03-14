using System;
using _Project.System.DS.Models;

namespace _Project.System.DS.Examples
{
    [Serializable]
    public class PlayerData : DataEntity
    {
        public int id;
        public string playerName;
        public int level;

        public override string ToDebugString()
        {
            return $"{base.ToDebugString()},ID: {id}, Name: {playerName}, Level: {level}";
        }
    }
}