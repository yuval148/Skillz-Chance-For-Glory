using ElfKingdom;

namespace MyBot
{
    abstract class StrategicCalculations : Math
    {
        public int ManaWasted { get; set; }
        protected double BaseDegree { get; private set; }
        protected int DefendRadius { get; private set; }
        protected int DesiredPortalAmount { get; private set; }
        protected int DesiredManaPerTurn { get; private set; }
        protected int IdealManaPerTurn { get; private set; }
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
        protected int EnemyAggressiveTornadoRangeFromPortal { get; private set; }
        protected int EnemyAggressiveLavaGiantRangeFromCastle { get; private set; }
        protected int EnemyAggressiveLavaGiantRangeFromElf { get; private set; }

        public abstract void DoTurn(Game game);
        public void CalculateConsts(Game game)
        {
            CalculateBaseDegree(game);
        }
        public void CalculateAll(Game game)
        {
            if (game.Turn <= 1)
            {
                CalculateConsts(game);
            }
            CalculateTheLongestDay(game);
            CalculateMinPortalBuildRadius(game);
            CalculateMinFountainBuildRadius(game);
            CalculateMaxBuildRadius(game);
            CalculateDefendRadius(game);
            CalculateDesiredPortalAmount(game);
            CalculateDesiredManaPerTurn(game);
            CalculateIdealManaPerTurn(game);
            CalculateIceTrollSummonRate(game);
            CalculateMinManaForPortal(game);
            CalculateMaxPotentialMana(game);
            CalculateEnemyAggressivePortalRangeFromCastle(game);
            CalculateEnemyAggressivePortalRangeFromElf(game);
            CalculateEnemyAggressiveElfRangeFromCastle(game);
            CalculateEnemyAggressiveElfRangeFromElf(game);
            CalculateEnemyAggressiveElfRangeFromPortal(game);
            CalculateEnemyAggressiveTornadoRangeFromPortal(game);
            CalculateEnemyAggressiveLavaGiantRangeFromCastle(game);
            CalculateEnemyAggressiveLavaGiantRangeFromElf(game);
        }
        protected Location Cis(double radius, double degree, Location baseLocation = null)
        {
            if (baseLocation == null)
            {
                return new Location((int)(radius * Sin(degree)), (int)(radius * Cos(degree)));
            }
            else
            {
                return new Location(baseLocation.Row + (int)(radius * Sin(degree)), baseLocation.Col + (int)(radius * Cos(degree)));
            }
        }
        protected double DegreeBetween(Location a, Location b)
        {
            double a1 = Sqrt(Pow(a.Col - b.Col, 2) + Pow(a.Row - b.Row, 2));
            double b1 = Abs(a.Col - b.Col);
            return (a.Col > b.Col ? 3 * PI / 2 : PI / 2) + Asin(b1 / a1);
        }
        private void CalculateBaseDegree(Game game)
        {
            Location baseLocation = game.GetMyCastle().Location;
            double a = Sqrt(Pow(baseLocation.Col - game.GetEnemyCastle().Location.Col, 2) + Pow(baseLocation.Row - game.GetEnemyCastle().Location.Row, 2));
            double b = Abs(baseLocation.Col - game.GetEnemyCastle().Location.Col);
            BaseDegree = Asin(b / a);
            BaseDegree = PI / 40;
            int min = int.MaxValue;
            for (int i = 0; i < 80; i++)
            {
                if (game.GetEnemyCastle().Distance(Cis(100, PI / 40 + PI * i / 40, game.GetMyCastle().Location)) < min)
                {
                    min = game.GetEnemyCastle().Distance(Cis(100, PI / 40 + PI * i / 40, game.GetMyCastle().Location));
                    BaseDegree = PI / 40 + PI * i / 40;
                }
            }
        }
        private void CalculateDefendRadius(Game game)
        {
            //DefendRadius = ((game.ElfMaxHealth / game.ElfAttackMultiplier) * game.ElfMaxSpeed / 2) + game.PortalSize + game.CastleSize;
            int maxDistance = -1;
            GameObject myCastle = game.GetMyCastle();
            foreach (Portal portal in game.GetMyPortals())
            {
                if (portal.Distance(game.GetMyCastle()) > maxDistance)
                {
                    maxDistance = portal.Distance(game.GetMyCastle());
                }
            }
            if (maxDistance != -1)
            {
                if (maxDistance < MaxBuildRadius)
                {
                    DefendRadius = maxDistance;
                }
                else
                {
                    DefendRadius = MaxBuildRadius;
                }
            }
            else
            {
                DefendRadius = ((game.ElfMaxHealth / game.ElfAttackMultiplier) * game.ElfMaxSpeed / 2) + game.PortalSize + game.CastleSize;
            }
        }
        private void CalculateIceTrollSummonRate(Game game)
        {
            try
            {
                IceTrollSummonRate = (int)Floor((decimal)((game.IceTrollCost / game.GetMyself().ManaPerTurn) * 1.6));
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
        private void CalculateDesiredManaPerTurn(Game game)
        {
            if (game.GetAllEnemyElves().Length == 3)
            {
                DesiredManaPerTurn = 13;
            }
            else
            {
                DesiredManaPerTurn = 8;
            }
        }
        private void CalculateIdealManaPerTurn(Game game)
        {
            if (game.GetAllEnemyElves().Length == 3)
            {
                IdealManaPerTurn = 16;
            }
            else
            {
                IdealManaPerTurn = 11;
            }
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
        private void CalculateEnemyAggressiveTornadoRangeFromPortal(Game game)
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
