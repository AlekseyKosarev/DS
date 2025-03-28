using System;
using DS.Models;

namespace DS.Examples.Data
{
    [Serializable]
    public class DataExampleDS : DataEntity
    {
        public string name;
        public int id;

        public int grade;

        public override string ToDebugString()
        {
            return $"{base.ToDebugString()},ID: {id}, Name: {name}";
        }
    }
}