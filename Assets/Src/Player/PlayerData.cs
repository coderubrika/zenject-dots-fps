namespace TestRPG.PlayerDir
{
    public class PlayerData
    {
        public FloatValue Health { get; }
        public FloatValue Speed { get; }
        public FloatValue Damage { get; }
        public FloatValue Score { get; }
        
        public PlayerData(
            FloatValue health, 
            FloatValue speed, 
            FloatValue damage, 
            FloatValue score)
        {
            Health = health;
            Speed = speed;
            Damage = damage;
            Score = score;
        }
    }
}