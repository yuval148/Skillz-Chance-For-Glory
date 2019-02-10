using ElfKingdom;

namespace MyBot
{
    abstract class StrategicCalculations
    {
        public int ManaWasted { get; set; }
        protected int DefendRadius { get; private set; }
        protected int DesiredPortalAmount { get; private set; }
        protected int IceTrollSummonRate { get; private set; }
        protected int TheLongestDay { get; private set; }
        protected int MinBuildRadius { get; private set; } 
        protected int MaxBuildRadius { get; private set; } 
        protected int MinManaForPortal { get; private set; }
        protected int MaxPotentialMana { get; private set; }
        protected int EnemyAggressivePortalRangeFromCastle { get; private set; }
        protected int EnemyAggressivePortalRangeFromElf { get; private set; }
        protected int EnemyAggressiveElfRangeFromCastle { get; private set; }
        protected int EnemyAggressiveElfRangeFromElf { get; private set; }
        protected int EnemyAggressiveElfRangeFromPortal { get; private set; }
        protected int EnemyAggressiveLavaGiantRangeFromCastle { get; private set; }
        protected int EnemyAggressiveLavaGiantRangeFromElf { get; private set; }
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
            DefendRadius = ((game.ElfMaxHealth / game.ElfAttackMultiplier) * game.ElfMaxSpeed / 2) + game.PortalSize + game.CastleSize;
        }
        private void CalculateIceTrollSummonRate(Game game)
        {
            IceTrollSummonRate = (int)System.Math.Ceiling((decimal)((game.IceTrollCost / game.GetMyself().ManaPerTurn) * 1.6));
        }
        private void CalculateTheLongestDay(Game game)
        {
            TheLongestDay = (int)(game.MaxTurns * 0.8);
        }
        private void CalculateMaxPotentialMana(Game game)
        {
            MaxPotentialMana = TheLongestDay * game.GetMyself().ManaPerTurn;
        }
        private void CalculateMinBuildRadius(Game game)
        {
            MinBuildRadius = (int)(game.PortalSize * 2.25 + game.CastleSize);
        }
        private void CalculateMaxBuildRadius(Game game)
        {
            MaxBuildRadius = 5000 + (int)(game.PortalSize * 2.25 + game.CastleSize);
        }
        private void CalculateMinManaForPortal(Game game)
        {
            MinManaForPortal = 100;
        }
        private void CalculateDesiredPortalAmount(Game game)
        {
            DesiredPortalAmount = 5;
        }
        private void CalculateEnemyAggressivePortalRangeFromCastle(Game game)
        {
            EnemyAggressivePortalRangeFromCastle = 3500;
        }
        private void CalculateEnemyAggressivePortalRangeFromElf(Game game)
        {
            EnemyAggressivePortalRangeFromElf = 700;
        }
        private void CalculateEnemyAggressiveElfRangeFromCastle(Game game)
        {
            EnemyAggressiveElfRangeFromCastle = 1500;
        }
        private void CalculateEnemyAggressiveElfRangeFromElf(Game game)
        {
            EnemyAggressiveElfRangeFromElf = 500;
        }
        private void CalculateEnemyAggressiveElfRangeFromPortal(Game game)
        {
            //750
            EnemyAggressiveElfRangeFromPortal = game.PortalSize + game.ElfAttackRange + game.ElfMaxSpeed * IceTrollSummonRate;
            game.Debug("EAERFP: " + EnemyAggressiveElfRangeFromPortal);
        }
        private void CalculateEnemyAggressiveLavaGiantRangeFromCastle(Game game)
        {
            EnemyAggressiveLavaGiantRangeFromCastle = 1500;
        }
        private void CalculateEnemyAggressiveLavaGiantRangeFromElf(Game game)
        {
            EnemyAggressiveLavaGiantRangeFromElf = 500;
        }
    }
}
