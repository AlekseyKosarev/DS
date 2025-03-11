using DS.Models;

namespace DS.Examples
{
    [global::System.Serializable]
    public class ExampleData: DataEntity
    {
        public string playerName;
        public int level;
        public float health;
        
        public override string ToDebugString() {
            return $"{base.ToDebugString()}, Name: {playerName}, Level: {level}, Health: {health}";
        }
    }
}