using System;
using DS.Models;

namespace DS.Examples.Data
{
    [Serializable]
    public class LevelData:DataEntity
    {
        public string name;
        public int id;
        
        public override string ToDebugString()
        {
            return $"{base.ToDebugString()},ID: {id}, Name: {name}";
        }
    }
}