using ElfKingdom;

namespace SkillzProject
{
    abstract class StrategicCalculations
    {
        public int manaWasted { get; set; }
        protected int defendRadius { get; private set; }
        protected int desiredPortalAmount { get; private set; }
        protected int iceTrollSummonRate { get; private set; }
        protected int theLongestDay { get; private set; }
        protected int minBuildRadius { get; private set; } 
        protected int maxBuildRadius { get; private set; } 
        protected int minManaForPortal { get; private set; }
        protected int maxPotentialMana { get; private set; }
        protected int enemyAggressivePortalRangeFromCastle { get; private set; }
        protected int enemyAggressivePortalRangeFromElf { get; private set; }
        protected int enemyAggressiveElfRangeFromCastle { get; private set; }
        protected int enemyAggressiveElfRangeFromElf { get; private set; }
        protected int enemyAggressiveElfRangeFromPortal { get; private set; }
        protected int enemyAggressiveLavaGiantRangeFromCastle { get; private set; }
        protected int enemyAggressiveLavaGiantRangeFromElf { get; private set; }
        public abstract void DoTurn(Game game);
        public void CalculateAll(Game game)
        {
            CalculateTheLongestDay(game);
            CalculateMinBuildRadius(game);
            CalculateMaxBuildRadius(game);
            CalculateDefendRadius(game);
            CalculateDesiredPortalAmount(game);
            CalculateIceTrollSummonRate(game);
            CalculateMinManaForPortal(game);
            CalculateMaxPotentialMana(game);
            CalculateEnemyAggressivePortalRangeFromCastle(game);
            CalculateEnemyAggressivePortalRangeFromElf(game);
            CalculateEnemyAggressiveElfRangeFromCastle(game);
            CalculateEnemyAggressiveElfRangeFromElf(game);
            CalculateEnemyAggressiveElfRangeFromPortal(game);
            CalculateEnemyAggressiveLavaGiantRangeFromCastle(game);
            CalculateEnemyAggressiveLavaGiantRangeFromElf(game);
        }
        protected Location Cis(double radius, double degree, Location baseLocation = null)
        {
            if (baseLocation == null)
            {
                return new Location((int)(radius * System.Math.Sin(degree)), (int)(radius * System.Math.Cos(degree)));
            }
            else
            {
                return new Location(baseLocation.Row + (int)(radius * System.Math.Sin(degree)), baseLocation.Col + (int)(radius * System.Math.Cos(degree)));
            }
        }
        protected double DegreeBetween(Location a, Location b)
        {
            double a1 = System.Math.Sqrt(System.Math.Pow(a.Col - b.Col, 2) + System.Math.Pow(a.Row - b.Row, 2));
            double b1 = System.Math.Abs(a.Col - b.Col);
            return (a.Col > b.Col ? 3 * System.Math.PI / 2 : System.Math.PI / 2) + System.Math.Asin(b1 / a1);
        }
        private void CalculateDefendRadius(Game game)
        {
            defendRadius = ((game.ElfMaxHealth / game.ElfAttackMultiplier) * game.ElfMaxSpeed / 2) + game.PortalSize + game.CastleSize;
        }
        private void CalculateIceTrollSummonRate(Game game)
        {
            iceTrollSummonRate = (int)System.Math.Ceiling((decimal)((game.IceTrollCost / game.GetMyself().ManaPerTurn) * 1.6));
        }
        private void CalculateTheLongestDay(Game game)
        {
            theLongestDay = (int)(game.MaxTurns * 0.8);
        }
        private void CalculateMaxPotentialMana(Game game)
        {
            maxPotentialMana = theLongestDay * game.GetMyself().ManaPerTurn;
        }
        private void CalculateMinBuildRadius(Game game)
        {
            minBuildRadius = (int)(game.PortalSize * 2.25 + game.CastleSize);
        }
        private void CalculateMaxBuildRadius(Game game)
        {
            maxBuildRadius = (int)(game.PortalSize * 2.25 + game.CastleSize);
        }
        private void CalculateMinManaForPortal(Game game)
        {
            minManaForPortal = 100;
        }
        private void CalculateDesiredPortalAmount(Game game)
        {
            desiredPortalAmount = 5;
        }
        private void CalculateEnemyAggressivePortalRangeFromCastle(Game game)
        {
            enemyAggressivePortalRangeFromCastle = 3500;
        }
        private void CalculateEnemyAggressivePortalRangeFromElf(Game game)
        {
            enemyAggressivePortalRangeFromElf = 700;
        }
        private void CalculateEnemyAggressiveElfRangeFromCastle(Game game)
        {
            enemyAggressiveElfRangeFromCastle = 1500;
        }
        private void CalculateEnemyAggressiveElfRangeFromElf(Game game)
        {
            enemyAggressiveElfRangeFromElf = 500;
        }
        private void CalculateEnemyAggressiveElfRangeFromPortal(Game game)
        {
            enemyAggressiveElfRangeFromPortal = 750;
        }
        private void CalculateEnemyAggressiveLavaGiantRangeFromCastle(Game game)
        {
            enemyAggressiveLavaGiantRangeFromCastle = 1500;
        }
        private void CalculateEnemyAggressiveLavaGiantRangeFromElf(Game game)
        {
            enemyAggressiveLavaGiantRangeFromElf = 500;
        }
    }
}
