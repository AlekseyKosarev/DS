using System;
using DS.Models;

namespace DS.Examples.Data
{
    [Serializable]
    public class PlayerData : DataEntity
    {
        public int id;
        public string name;
        public int level;

        public override string ToDebugString()
        {
            return $"{base.ToDebugString()},ID: {id}, Name: {name}, Level: {level}";
        }
    }
}