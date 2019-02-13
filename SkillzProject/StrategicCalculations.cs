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
        protected int MinPortalBuildRadius { get; private set; }
        protected int MinFountainBuildRadius { get; private set; }
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
            CalculateMinPortalBuildRadius(game);
            CalculateMinFountainBuildRadius(game);
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
            try
            {
                IceTrollSummonRate = (int)System.Math.Floor((decimal)((game.IceTrollCost / game.GetMyself().ManaPerTurn) * 1.6));
            }
            catch
            {
                IceTrollSummonRate = 8;//(int)System.Math.Ceiling((decimal)((game.IceTrollCost / game.DefaultManaPerTurn) * 1.6));
            }
        }
        private void CalculateTheLongestDay(Game game)
        {
            TheLongestDay = (int)(game.MaxTurns * 0.8);
        }
        private void CalculateMaxPotentialMana(Game game)
        {
            MaxPotentialMana = TheLongestDay * game.GetMyself().ManaPerTurn;
        }
        private void CalculateMinPortalBuildRadius(Game game)
        {
            MinPortalBuildRadius = (int)(game.PortalSize * 2.25 + game.CastleSize + game.ManaFountainSize * 2);
        }
        private void CalculateMinFountainBuildRadius(Game game)
        {
            MinFountainBuildRadius = (int)(game.ManaFountainSize * 2.24 + game.CastleSize);
        }
        private void CalculateMaxBuildRadius(Game game)
        {
            MaxBuildRadius = 5000 + (int)(game.PortalSize * 2.25 + game.CastleSize);
        }
        private void CalculateMinManaForPortal(Game game)
        {
            if (game.GetMyself().ManaPerTurn <= 8)
            {
                MinManaForPortal = game.ManaFountainCost * ((game.ManaFountainManaPerTurn * (1 + (8 - game.DefaultManaPerTurn) / game.ManaFountainManaPerTurn) - (game.GetMyself().ManaPerTurn - game.DefaultManaPerTurn)) / game.ManaFountainManaPerTurn);
            }
            else
            {
                MinManaForPortal = game.PortalCost;
            }
        }
        private void CalculateDesiredPortalAmount(Game game)
        {
            DesiredPortalAmount = 4;
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
            //1500
            EnemyAggressiveElfRangeFromCastle = game.CastleSize + game.ElfAttackRange + game.ElfMaxSpeed * IceTrollSummonRate;
            //1600
        }
        private void CalculateEnemyAggressiveElfRangeFromElf(Game game)
        {
            EnemyAggressiveElfRangeFromElf = 500;
        }
        private void CalculateEnemyAggressiveElfRangeFromPortal(Game game)
        {
            //750
            EnemyAggressiveElfRangeFromPortal = game.PortalSize + game.ElfAttackRange + game.ElfMaxSpeed * IceTrollSummonRate;
            //1400
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
